using EasySave.Utils.JobStates;
using LoggerLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoSoftLib;
using EasySave.Utils;
using EasySave.CustomExceptions;

namespace EasySave;

public abstract class SaveJob
{
    // ATTRIBUTES
    public string Name { get; set; }
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastUpdate { get; set; }
    public string State { get; set; }
    private bool CanRun { get; set; } = true;

    private readonly ProcessObserver _businessSoftwaresObserver;

    //CONSTRUCTOR
    protected SaveJob(string name, string sourcePath, string targetPath, bool checkBusinessSoftwares = false)
    {
        Name = name;
        SourcePath = sourcePath;
        TargetPath = targetPath;
        CreationDate = DateTime.Now;
        LastUpdate = DateTime.Now;
        State = StateJsonReader.SavedState;

        if (checkBusinessSoftwares)
        {
            _businessSoftwaresObserver = new ProcessObserver(1000);
            _businessSoftwaresObserver.OnProcessStateChanged += isRunning =>
            {
                CanRun = !isRunning;
            };
        }
    }

    //METHODS

    public bool CreateSave()
    {
        CheckIfCanRun();

        if (SourcePath == TargetPath)
        {
            Logger.GetInstance().Log(
                   new
                   {
                       Statue = "Error",
                       Message = "Source path and Target path can't be equal"
                   });
            throw new ArgumentException("Source path and Target path can't be equal");
        }

        // Create Target Directory
        Directory.CreateDirectory(TargetPath);

        // Create a default FULL SAVE
        FullSave(SourcePath, TargetPath);

        return StateJsonReader.GetInstance().AddJob(this);
    }

    protected bool FullSave(string sourcePath, string targetPath)
    {
        // Create save path with type and date
        string saveTargetPath = Path.Combine(targetPath, ("FullSave_" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss")));

        // Get information about the source directory
        var dir = new DirectoryInfo(sourcePath);

        // Check if the source directory exists
        // TODO We need to andle the case of an inexistant directory. Currently, it crashes
        if (!dir.Exists)
        {
            Logger.GetInstance().Log(
                 new
                 {
                     Statue = "Error",
                     Message = $"Source directory not found: {dir.FullName}"
                 });
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
        }

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(saveTargetPath);

        CreateFullSave(SourcePath, saveTargetPath);

        return true;
    }

    protected bool CreateFullSave(string sourcePath, string saveTargetPath, long? leftSizeToCopy = null, long? leftFilesToCopy = null, long? totalSizeToCopy = null)
    {
        CheckIfCanRun();
        // Get information about the source directory
        var dir = new DirectoryInfo(sourcePath);

        // Check if the source directory exists
        // TODO We need to andle the case of an inexistant directory. Currently, it crashes
        if (!dir.Exists)
        {
            Logger.GetInstance().Log(
               new
               {
                   Statue = "Error",
                   Message = $"Source directory not found: {dir.FullName}"
               });
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
        }
          

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        if (leftSizeToCopy == null && leftFilesToCopy == null)
        {
            // Compute the total size of the root directory
            leftSizeToCopy = dir.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);

            // Compute the total number of files to copy
            leftFilesToCopy = dir.EnumerateFiles("*", SearchOption.AllDirectories).Count();

            totalSizeToCopy = leftSizeToCopy;

            StateJsonReader.GetInstance().UpdateJob(Name, new JobStateJsonDefinition
            {
                State = StateJsonReader.SavingState,
                LastUpdate = DateTime.Now,
                TotalFilesToCopy = leftFilesToCopy,
                TotalFilesSize = totalSizeToCopy
            });
        }

        // Create the destination directory
        Directory.CreateDirectory(saveTargetPath);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            CheckIfCanRun();
            string targetFilePath = Path.Combine(saveTargetPath, file.Name);

            Stopwatch stopwatch = Stopwatch.StartNew();
            file.CopyTo(targetFilePath);
            stopwatch.Stop();

            Stopwatch stopwatchCrypt = Stopwatch.StartNew();            
            bool isEncrypted = CryptoSoft.EncryptDecryptFile(targetFilePath);
            stopwatchCrypt.Stop();

            Logger.GetInstance().Log(
                new
                {
                    type = "Info",
                    SaveJobName = Name,
                    FileSource = Path.Combine(SourcePath, file.Name),
                    FileTarget = targetFilePath,
                    FileSize = file.Length,
                    Time = DateTime.Now,
                    FileCryptTime = isEncrypted ? stopwatchCrypt.ElapsedMilliseconds : 0,
                    FileTransferTime = stopwatch.ElapsedMilliseconds
                });
            leftFilesToCopy--;
            leftSizeToCopy -= file.Length;
            leftSizeToCopy = leftSizeToCopy <= 1 ? 1 : leftSizeToCopy;
            StateJsonReader.GetInstance().UpdateJob(Name, new JobStateJsonDefinition
            {
                State = StateJsonReader.SavingState,
                LastUpdate = DateTime.Now,
                Progression = 100 - (leftSizeToCopy * 100) / totalSizeToCopy,
                NbFilesLeftToDo = leftFilesToCopy,
                TotalSizeLeftToDo = leftSizeToCopy,
                SourceFilePath = Path.Combine(SourcePath, file.Name),
                TargetFilePath = targetFilePath
            });
           
        }

        // RECURSIVITY : Copy the files from the sub directories
        foreach (DirectoryInfo subDir in dirs)
        {
            string newDestinationDir = Path.Combine(saveTargetPath, subDir.Name);
            CreateFullSave(subDir.FullName, newDestinationDir, leftSizeToCopy, leftFilesToCopy, totalSizeToCopy);
        }


        StateJsonReader.GetInstance().UpdateJob(Name, new JobStateJsonDefinition
        {
            State = StateJsonReader.SavedState,
            LastUpdate = DateTime.Now,
            TotalFilesToCopy = null,
            TotalFilesSize = null,
            Progression = null,
            NbFilesLeftToDo = null,
            TotalSizeLeftToDo = null,
            SourceFilePath = null,
            TargetFilePath = null
        });
        return true;
    }

    public abstract bool Save();

    public abstract bool RestoreSave();

    public bool DeleteSave()
    {
        try
        {
            // Supprimer le dossier de sauvegarde s'il existe
            if (Directory.Exists(TargetPath))
            {
                Directory.Delete(TargetPath, true);
            }

            // Supprime le job de la bdd
            StateJsonReader stateDB = StateJsonReader.GetInstance();
            stateDB.DeleteJob(this);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors de la suppression : " + ex.Message);
            return false;
        }
    }

    protected void CheckIfCanRun()
    {
        if (!CanRun)
        {
            throw new BusinessSoftwareRunningException();
        }
    }

    public override string ToString()
    {
        return $"'{Name}' | Source : '{SourcePath}', Destination : '{TargetPath}'";
    }
}
