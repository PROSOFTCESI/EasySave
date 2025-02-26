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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace EasySave;

public abstract class SaveJob : INotifyPropertyChanged
{
    // ATTRIBUTES
    public string Name { get; set; }
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastUpdate { get; set; }
    public string State { get; set; }
    public string NameLastSave { get; set; } = "";


    private long? _progression;
    public long? Progression
    {
        get => _progression;
        set
        {
            if (_progression != value)
            {
                _progression = value;
                OnPropertyChanged();
            }
        }
    }
    private bool _disposed = false;
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private bool CanRun { get; set; } = true;
    private bool Paused { get; set; } = false;

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
            _businessSoftwaresObserver = ProcessObserver.GetInstance(1000);
            _businessSoftwaresObserver.OnProcessStateChanged += isRunning =>
            {
                CanRun = !isRunning;
            };
        }
    }

    //METHODS

    public bool CreateSave()
    {
        NameLastSave = "FullSave_" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss");

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

        string saveTargetPath = Path.Combine(TargetPath, NameLastSave);

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
        StateJsonReader.GetInstance().UpdateJob(Name, new JobStateJsonDefinition
        {
            State = StateJsonReader.SavingState,
            LastUpdate = DateTime.Now,
        });
        
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
        if (Paused)
        {
            StateJsonReader.GetInstance().UpdateJob(Name, new JobStateJsonDefinition
            {
                State = StateJsonReader.PausedState,
                LastUpdate = DateTime.Now,
            });
            throw new PlayPauseStopException("PAUSED");
        }
    }

    public void Pause()
    {
        Paused = true;
        Logger.GetInstance().Log(new
        {
            type = "Info",
            SaveJobName = Name,
            Time = DateTime.Now,
            action = "Pause"
        });
        CheckIfCanRun();
    }

    public void Play()
    {
        if (!StateJsonReader.GetInstance().GetJob(Name).State.Equals(StateJsonReader.PausedState))
        {
            return;
        }
        Paused = false;
        Logger.GetInstance().Log(new
        {
            type = "Info",
            SaveJobName = Name,
            Time = DateTime.Now,
            action = "Play"
        });
        NameLastSave = StateJsonReader.GetInstance().GetJob(Name).NameLastSave;
        string saveTargetPath = Path.Combine(TargetPath, NameLastSave);
        string jsonFilePath = Path.Combine(TargetPath, Path.Combine(NameLastSave, ".fileStructure.json"));
        if (!File.Exists(jsonFilePath))
        {
            ResetState();
            StateJsonReader.GetInstance().UpdateJob(Name, new JobStateJsonDefinition
            {
                State = StateJsonReader.SavingState,
                LastUpdate = DateTime.Now,
            });
            string jsonPath = FileStructureJson.GetInstance().CreateFileStructure(SourcePath, saveTargetPath);
            FileStructureJson.GetInstance().GetAdvancement(jsonPath);
            CopyFiles(jsonPath, saveTargetPath);
            EncryptFiles(jsonPath, saveTargetPath, true);
        }
        else
        {
            string jsonContent = File.ReadAllText(jsonFilePath);
            var jsonStructure = JsonConvert.DeserializeObject<JsonStructure>(jsonContent);
            if (jsonStructure.Status.Equals("set"))
            {
                ResetState();
                long[] advancement = FileStructureJson.GetInstance().GetAdvancement(jsonFilePath);
                if (advancement[0] != advancement[2]  && advancement[0] != 0) // If saving not completed
                {
                    CopyFiles(jsonFilePath, saveTargetPath);
                    EncryptFiles(jsonFilePath, saveTargetPath, true);
                }else if(advancement[0] != advancement[4]) // If encrypting not completed
                {
                    EncryptFiles(jsonFilePath, saveTargetPath, true);
                }
            }
        }

    public void Stop()
    {
        string dir = Path.Combine(TargetPath, NameLastSave);
        if (Directory.Exists(dir))
        {
            Directory.Delete(dir, true);
        }
        ResetState();

        NameLastSave = GetLastSavePath((this is FullSave) ? "full" : "Diffenretial");
        if(NameLastSave is null && this is DifferentialSave)
        {
            NameLastSave = GetLastSavePath();
        }

        var state = StateJsonReader.GetInstance().GetJob(Name);
        state.NameLastSave = NameLastSave;
        StateJsonReader.GetInstance().UpdateJob(Name, state);
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
                new SmartFileCopier().CopyFile(source, newFile);
                stopwatch.Stop();

                // Save Status
                file.Status = "saved";
                string updatedJson = JsonConvert.SerializeObject(jsonStructure, Formatting.Indented);
                File.WriteAllText(jsonFilePath, updatedJson);


                // Log
                Logger.GetInstance().Log(new
                {
                    type = "Info",
                    SaveJobName = Name,
                    FileSource = source,
                    FileTarget = newFile,
                    FileSize = file.Size,
                    Time = DateTime.Now,
                    action = "save",
                    FileTransferTime = stopwatch.ElapsedMilliseconds
                });

                long[] advancement = FileStructureJson.GetInstance().GetAdvancement(jsonFilePath);
                int progress = 0;
                if (advancement[1] != 0)
                {
                    progress = (int)Math.Round(((double)advancement[3] / advancement[1]) * 100);
                }

                Progression = progress;

                SetAvancement("saving", jsonFilePath, file.Name);
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
            if (File.Exists(filePath))
            {

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

                long[] advancement = FileStructureJson.GetInstance().GetAdvancement(jsonFilePath);
                int progress = 0;
                if (advancement[1] != 0)
                {
                    progress = (int)Math.Round(((double)advancement[5] / advancement[1]) * 100);
                }

                SetAvancement("encrypt", jsonFilePath, file.Name);
            }
        }
        StateJsonReader.GetInstance().UpdateJob(Name, new JobStateJsonDefinition
        {
            //State = isEncrypted ? StateJsonReader.EncryptedState : StateJsonReader.DecryptedState,
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
        return isEncrypted;
    }

    public void ResetState()
    {
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

    private void SetAvancement(string origin, string jsonFilePath, string fileName = "")
    {

        long[] advancement = FileStructureJson.GetInstance().GetAdvancement(jsonFilePath);

        string state = "";
        long progress = 0;
        long advanceFiles = 0;
        long advanceBytes = 0;

        if (origin == "saving")
        {
            state = StateJsonReader.SavingState;
            advanceFiles = advancement[2];
            advanceBytes = advancement[3];
        }
        if (origin == "encrypt")
        {
            state = StateJsonReader.EncryptingState;
            advanceFiles = advancement[4];
            advanceBytes = advancement[5];
        }

        if (advancement[1] != 0)
        {
            progress = (int)Math.Round(((double)advanceBytes / advancement[1]) * 100);
        }

        StateJsonReader.GetInstance().UpdateJob(Name, new JobStateJsonDefinition
        {
            State = Paused ? StateJsonReader.PausedState : state,
            LastUpdate = DateTime.Now,
            TotalFilesToCopy = advancement[0],
            TotalFilesSize = advancement[1],
            Progression = progress,
            NbFilesLeftToDo = advancement[0] - advanceFiles,
            TotalSizeLeftToDo = advancement[1] - advanceBytes,
            SourceFilePath = Path.Combine(SourcePath, fileName),
            TargetFilePath = Path.Combine(TargetPath, fileName)
        });

    public string GetLastSavePath(string type = "full")
    {

        Regex regex = new Regex(@"^FullSave_(\d{2}_\d{2}_\d{4}-\d{2}_\d{2}_\d{2})$");
        if (type == "Diffenretial")
        {
            regex = new Regex(@"^DifferentialSave_(\d{2}_\d{2}_\d{4}-\d{2}_\d{2}_\d{2})$");
        }
        var latestSave = Directory.GetDirectories(TargetPath)
            .Select(Path.GetFileName)
            .Where(name => regex.IsMatch(name))
            .Select(name => new { Name = name, Date = DateTime.ParseExact(name.Substring(9), "dd_MM_yyyy-HH_mm_ss", null) })
            .OrderByDescending(entry => entry.Date)
            .FirstOrDefault();

        return latestSave?.Name;
    }
}
