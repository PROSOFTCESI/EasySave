using EasySave.Utils.JobStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    //CONSTRUCTOR
    protected SaveJob(string name, string sourcePath, string targetPath)
    {
        Name = name;
        SourcePath = sourcePath;
        TargetPath = targetPath;
        CreationDate = DateTime.Now;
        LastUpdate = DateTime.Now;
        State = StateJsonReader.SavedState;
    }

    //METHODS

    public bool CreateSave()
    {
        // Create Target Directory
        Directory.CreateDirectory(TargetPath);

        // Create a default FULL SAVE
        FullSave(SourcePath, TargetPath);
        
        return true;
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
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(saveTargetPath);

        CreateFullSave(SourcePath, saveTargetPath);

        return true;
    }

    protected bool CreateFullSave(string sourcePath, string saveTargetPath)
    {

        // Get information about the source directory
        var dir = new DirectoryInfo(sourcePath);

        // Check if the source directory exists
        // TODO We need to andle the case of an inexistant directory. Currently, it crashes
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(saveTargetPath);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(saveTargetPath, file.Name);
            file.CopyTo(targetFilePath);
        }

        // RECURSIVITY : Copy the files from the sub directories
        foreach (DirectoryInfo subDir in dirs)
        {
            string newDestinationDir = Path.Combine(saveTargetPath, subDir.Name);
            CreateFullSave(subDir.FullName, newDestinationDir);
        }

        return true;
    }

    public abstract bool Save();

    public abstract bool RestoreSave();

    public bool DeleteSave()
    {
        // Delete directory and all its contents
        // TODO : call logs
        Directory.Delete(TargetPath, true);
        return true;
    }

    public override string ToString()
    {
        return $"'{Name}' | Source : '{SourcePath}', Destination : '{TargetPath}'";
    }
}
