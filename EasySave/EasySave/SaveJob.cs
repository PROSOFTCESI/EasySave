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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;

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

        StateJsonReader.GetInstance().AddJob(this);

        string saveTargetPath = Path.Combine(TargetPath, ("FullSave_" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss")));

        // Get information about the source directory
        var dir = new DirectoryInfo(SourcePath);

        // Check if the source directory exists
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

        // Create the destination directory
        Directory.CreateDirectory(saveTargetPath);
        //Create Json with file structure
        string jsonPath = FileStructureJson.GetInstance().CreateFileStructure(SourcePath, saveTargetPath);
        FileStructureJson.GetInstance().GetAdvancement(jsonPath);
        // Copy Files
        CopyFiles(jsonPath, saveTargetPath);
        //EncryptFiles
        EncryptFiles(jsonPath, saveTargetPath, true);

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

    //Copy files 
    //TODO ADD logger and size files
    public void CopyFiles(string jsonFilePath, string saveTargetPath)
    {
        CheckIfCanRun();
        if (!File.Exists(jsonFilePath))
        {
            Logger.GetInstance().Log(
               new
               {
                   Statue = "Error",
                   Message = $"Json file does not exists: {jsonFilePath}"
               });

            StateJsonReader.GetInstance().UpdateJob(Name, new JobStateJsonDefinition
            {
                State = StateJsonReader.ErrorState,
                LastUpdate = DateTime.Now,
                TotalFilesToCopy = null,
                TotalFilesSize = null,
                Progression = null,
                NbFilesLeftToDo = null,
                TotalSizeLeftToDo = null,
                SourceFilePath = null,
                TargetFilePath = null
            });

            throw new Exception("Json file does not exists");
        }

        string jsonContent = File.ReadAllText(jsonFilePath);            
        var jsonStructure = JsonConvert.DeserializeObject<JsonStructure>(jsonContent);
        foreach (var file in jsonStructure.Files)
        {
            CheckIfCanRun();
            if (file.Status.Equals("set"))
            {
                string newFile = Path.Combine(saveTargetPath, file.Name);
                string dir = Path.GetDirectoryName(newFile);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                // Copy
                Stopwatch stopwatch = Stopwatch.StartNew();
                string source = Path.Combine(SourcePath, file.Name);                 
                File.Copy(source, newFile, true);
                stopwatch.Stop();

                // Save Status
                file.Status = "saved";
                string updatedJson = JsonConvert.SerializeObject(jsonStructure, Formatting.Indented);
                File.WriteAllText(jsonFilePath, updatedJson);

                FileStructureJson.GetInstance().GetAdvancement(jsonFilePath);

                // Log
                Logger.GetInstance().Log(new {
                    type = "Info",
                    SaveJobName = Name,
                    FileSource = source,
                    FileTarget = newFile,
                    FileSize = file.Size,
                    Time = DateTime.Now,
                    action = "save",
                    FileTransferTime = stopwatch.ElapsedMilliseconds
                }) ;

                // Update Statejson
                StateJsonReader.GetInstance().UpdateJob(Name, new JobStateJsonDefinition
                {
                    State = StateJsonReader.SavingState,
                    LastUpdate = DateTime.Now,
                    //Progression = long.Parse(jsonStructure.EncryptedFiles),
                    SourceFilePath = source,
                    TargetFilePath = newFile
                });
            }
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

    }

    public bool EncryptFiles(string jsonFilePath, string saveTargetPath, bool encrypt = true)
    {
        CheckIfCanRun();
        bool isEncrypted = false;
        if (!File.Exists(jsonFilePath))
        {
            Logger.GetInstance().Log(
               new
               {
                   Statue = "Error",
                   Message = $"Json file does not exists: {jsonFilePath}"
               });

            StateJsonReader.GetInstance().UpdateJob(Name, new JobStateJsonDefinition
            {
                State = StateJsonReader.ErrorState,
                LastUpdate = DateTime.Now,
                TotalFilesToCopy = null,
                TotalFilesSize = null,
                Progression = null,
                NbFilesLeftToDo = null,
                TotalSizeLeftToDo = null,
                SourceFilePath = null,
                TargetFilePath = null
            });
            throw new Exception("Json file does not exists");
        }

        string jsonContent = File.ReadAllText(jsonFilePath);
        var jsonStructure = JsonConvert.DeserializeObject<JsonStructure>(jsonContent);
        foreach (var file in jsonStructure.Files)
        {
            CheckIfCanRun();
            string filePath = Path.Combine(saveTargetPath, file.Name);
            if (File.Exists(filePath)) {

                Stopwatch stopwatch = Stopwatch.StartNew();
                if (file.Status.Equals("saved") && encrypt) // if saved and want to encrypt
                {
                    CryptoSoft.EncryptDecryptFile(filePath);
                    isEncrypted = true;
                    file.Status = "encrypted";  
                }
                if (file.Status.Equals("encrypted") && !encrypt) // if encrypted and want to decrypt
                {
                    CryptoSoft.EncryptDecryptFile(filePath);
                    isEncrypted = false;
                    file.Status = "decrypted";
                }
                if (file.Status.Equals("decrypted") && encrypt) // if decrypted and want to encrypt
                {
                    CryptoSoft.EncryptDecryptFile(filePath);
                    isEncrypted = true;
                    file.Status = "encrypted";
                }
                stopwatch.Stop();

                // Save Status
                string updatedJson = JsonConvert.SerializeObject(jsonStructure, Formatting.Indented);
                File.WriteAllText(jsonFilePath, updatedJson);

                FileStructureJson.GetInstance().GetAdvancement(jsonFilePath);

                Logger.GetInstance().Log(new
                {
                    type = "Info",
                    SaveJobName = Name,
                    FileSource = Path.Combine(SourcePath, file.Name),
                    FileTarget = Path.Combine(TargetPath, file.Name),
                    FileSize = file.Size,
                    Time = DateTime.Now,
                    action = isEncrypted ? "encrypt" : "decrypt",
                    FileCryptTime = isEncrypted ? stopwatch.ElapsedMilliseconds : 0
                });

                StateJsonReader.GetInstance().UpdateJob(Name, new JobStateJsonDefinition
                {
                    State = isEncrypted ? StateJsonReader.EncryptingState : StateJsonReader.DecryptingState,
                    LastUpdate = DateTime.Now,
                    //Progression = long.Parse(jsonStructure.SaveAdvancement),
                    SourceFilePath = Path.Combine(SourcePath, file.Name),
                    TargetFilePath = Path.Combine(TargetPath, file.Name)
                });
            }            
        }
        StateJsonReader.GetInstance().UpdateJob(Name, new JobStateJsonDefinition
        {
            State = isEncrypted ? StateJsonReader.EncryptedState : StateJsonReader.DecryptedState,
            LastUpdate = DateTime.Now,
            TotalFilesToCopy = null,
            TotalFilesSize = null,
            Progression = null,
            NbFilesLeftToDo = null,
            TotalSizeLeftToDo = null,
            SourceFilePath = null,
            TargetFilePath = null
        });
        return isEncrypted;
    }

}
