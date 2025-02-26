using System.IO.Enumeration;
using System.Net.Http.Json;
using System.Text.Json;

namespace EasySave.Utils.JobStates;

public class StateJsonReader
{
    private static readonly string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasySave");
    private static readonly string FilePath = Path.Combine(FolderPath, "state.json");

    private static StateJsonReader? instance;
    private static readonly object _lock = new(); // 🔒 Verrou pour l’accès concurrent

    public const string FullSaveType = "FullSave";
    public const string DifferentialSaveType = "DifferentialSave";

    public const string SavedState = "SAVED";
    public const string Saved2State = "Saved";
    public const string SavingState = "SAVING";
    public const string EncryptingState = "ENCRYPTING";
    public const string DecryptingState = "DECRYPTING";
    public const string EncryptedState = "ENCRYPTED";
    public const string DecryptedState = "DECRYPTED";
    public const string DeletedState = "DELETED";

    public const string PausedState = "PAUSED";
    public const string ErrorState = "ERROR";

    private StateJsonReader() { }

    public static StateJsonReader GetInstance()
    {
        instance ??= new StateJsonReader();
        return instance;
    }

    private List<JobStateJsonDefinition> ReadJson()
    {
        lock (_lock) // 🔒 Verrou
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            if (!File.Exists(FilePath))
            {
                File.WriteAllText(FilePath, "[]");
            }

            string jsonContent = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<JobStateJsonDefinition>>(jsonContent)
                           ?? throw new Exception("Le fichier JSON est vide ou invalide.");
        }
    }

    /// <summary>
    /// Get the list of actives jobs from the state.json file
    /// </summary>
    /// <returns>The list of active jobs</returns>
    public List<SaveJob> GetJobs(bool checkBusinessSoftwares = false)
    {

        List<SaveJob> jobsList = [];
        List<JobStateJsonDefinition> jobs = ReadJson();

        foreach (JobStateJsonDefinition job in jobs)
        {
            // Ignore the deleted jobs
            if (job.State == DeletedState)
                continue;

            switch (job.Type)
            {
                case FullSaveType:
                    jobsList.Add(new FullSave(job.Name, job.SourcePath, job.TargetPath, checkBusinessSoftwares)
                    {
                        State = job.State,
                        Progression = job.Progression,
                    });
                    break;
                case DifferentialSaveType:
                    jobsList.Add(new DifferentialSave(job.Name, job.SourcePath, job.TargetPath, checkBusinessSoftwares)
                    {
                        State = job.State,
                        Progression = job.Progression,
                    });
                    break;
            }
        }

        return jobsList;
    }

    /// <summary>
    /// Update a job in the state.json file
    /// </summary>
    /// <param name="jobName"></param>
    /// <param name="infos"></param>
    /// <returns>True if the job was successfully updated in the state.json file, false otherwise</returns>
    public bool UpdateJob(string jobName, JobStateJsonDefinition infos)
    {
        JobStateJsonDefinition jobToUpdate;
        try
        {
            jobToUpdate = GetJob(jobName);
        }
        catch (KeyNotFoundException)
        {
            return false;
        }

        jobToUpdate.LastUpdate = DateTime.Now;
        jobToUpdate.State = infos.State ?? jobToUpdate.State;
        jobToUpdate.NameLastSave = infos.NameLastSave ?? jobToUpdate.NameLastSave;
        jobToUpdate.TotalFilesToCopy = infos.TotalFilesToCopy == null && jobToUpdate.State.Equals(SavingState) ? jobToUpdate.TotalFilesToCopy : infos.TotalFilesToCopy;
        jobToUpdate.TotalFilesSize = infos.TotalFilesSize == null && jobToUpdate.State.Equals(SavingState) ? jobToUpdate.TotalFilesSize : infos.TotalFilesSize;
        jobToUpdate.Progression = infos.Progression == null && jobToUpdate.State.Equals(SavingState) ? jobToUpdate.Progression : infos.Progression;
        jobToUpdate.NbFilesLeftToDo = infos.NbFilesLeftToDo == null && jobToUpdate.State.Equals(SavingState) ? jobToUpdate.NbFilesLeftToDo : infos.NbFilesLeftToDo;
        jobToUpdate.TotalSizeLeftToDo = infos.TotalSizeLeftToDo == null && jobToUpdate.State.Equals(SavingState) ? jobToUpdate.TotalSizeLeftToDo : infos.TotalSizeLeftToDo;
        jobToUpdate.SourceFilePath = infos.SourceFilePath == null && jobToUpdate.State.Equals(SavingState) ? jobToUpdate.SourceFilePath : infos.SourceFilePath;
        jobToUpdate.TargetFilePath = infos.TargetFilePath == null && jobToUpdate.State.Equals(SavingState) ? jobToUpdate.TargetFilePath : infos.TargetFilePath;

        return UpdateJob(jobToUpdate);
        
    }

    /// <summary>
    /// Add a new job in the state.json file
    /// </summary>
    /// <param name="job"></param>
    /// <returns>True if the job was created, false otherwise</returns>
    public bool AddJob(SaveJob job)
    {
        try
        {
            JobStateJsonDefinition jobJson = new();
            jobJson.Name = job.Name;
            switch (job)
            {
                case FullSave _:
                    jobJson.Type = FullSaveType;
                    break;
                case DifferentialSave _:
                    jobJson.Type = DifferentialSaveType;
                    break;
            }
            jobJson.LastUpdate = job.LastUpdate;
            jobJson.SourcePath = job.SourcePath;
            jobJson.TargetPath = job.TargetPath;
            jobJson.State = job.State;
            jobJson.NameLastSave = job.NameLastSave;

            bool saved = SaveJob(jobJson);

            if (saved)
            {
                EasySave.SaveJob.Instances.Add(job);
            }

            return saved;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Delete the save job in the state.json file
    /// </summary>
    /// <param name="job"></param>
    /// <returns>True if the job was deleted, false otherwise</returns>
    public bool DeleteJob(SaveJob job)
    {
        try
        {
            JobStateJsonDefinition jobToDelete = GetJob(job.Name);
            jobToDelete.State = DeletedState;
            EasySave.SaveJob.Instances.Remove(job);
            return UpdateJob(jobToDelete);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool SaveJob(JobStateJsonDefinition job)
    {
        try
        {
            List<JobStateJsonDefinition> jobsJson = ReadJson();
            if (jobsJson.Any(j => j.Name == job.Name && j.State != DeletedState))
            {
                return false;
            }

            jobsJson.Add(job);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(jobsJson, options);
            lock (_lock) // 🔒 Verrou
            {
                File.WriteAllText(FilePath, json);
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    public JobStateJsonDefinition GetJob(string jobName)
    {
        List<JobStateJsonDefinition> jobsJson = ReadJson();
        JobStateJsonDefinition job = jobsJson.Find(j => j.Name == jobName && j.State != DeletedState) ?? throw new KeyNotFoundException($"Job {jobName} not found");
        return job;
    }

    private bool UpdateJob(JobStateJsonDefinition job)
    {
        try
        {
            List<JobStateJsonDefinition> jobsJson = ReadJson();
            jobsJson[jobsJson.FindIndex(j => j.Name == job.Name && j.State != DeletedState)] = job;
            var jobToUpdate = EasySave.SaveJob.Instances.Where(j => j.Name.Equals(job.Name)).FirstOrDefault();
            jobToUpdate.State = job.State;
            jobToUpdate.Progression = job.Progression;
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(jobsJson, options);
            lock (_lock) // 🔒 Verrou
            {
                File.WriteAllText(FilePath, json);
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
        
    }
}

/// <summary>
/// The json definition of a save job in the state.json file
/// </summary>
public class JobStateJsonDefinition
{
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime LastUpdate { get; set; }
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }
    public string State { get; set; }
    public string NameLastSave { get; set; }
    public long? TotalFilesToCopy { get; set; } = null;
    public long? TotalFilesSize { get; set; } = null;
    public long? Progression { get; set; } = null;
    public long? NbFilesLeftToDo { get; set; } = null;
    public long? TotalSizeLeftToDo { get; set; } = null;
    public string? SourceFilePath { get; set; } = null;
    public string? TargetFilePath { get; set; } = null;
}
