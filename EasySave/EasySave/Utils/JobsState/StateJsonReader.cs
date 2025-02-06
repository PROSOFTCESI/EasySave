using System.IO.Enumeration;
using System.Net.Http.Json;
using System.Text.Json;

namespace EasySave.Utils.JobStates;

internal class StateJsonReader
{
    private static readonly string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasySave");
    private static readonly string FilePath = Path.Combine(FolderPath, "state.json");

    private static StateJsonReader? instance;

    public const string FullSaveType = "FullSave";
    public const string DifferentialSaveType = "DifferentialSave";

    public const string SavedState = "SAVED";
    public const string SavingState = "SAVING";
    public const string DeletedState = "DELETED";

    private StateJsonReader() { }

    public static StateJsonReader GetInstance()
    {
        instance ??= new StateJsonReader();
        return instance;
    }
    private List<JobStateJsonDefinition> ReadJson()
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

    public List<SaveJob> GetJobs()
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
                    jobsList.Add(new FullSave(job.Name, job.SourcePath, job.TargetPath));
                    break;
                case DifferentialSaveType:
                    jobsList.Add(new DifferentialSave(job.Name, job.SourcePath, job.TargetPath));
                    break;
            }
        }

        return jobsList;
    }

    public bool UpdateJob(string jobName, JobStateJsonDefinition infos)
    {
        JobStateJsonDefinition jobToUpdate = GetJob(jobName);

        jobToUpdate.LastUpdate = DateTime.Now;
        jobToUpdate.State = infos.State ?? jobToUpdate.State;
        jobToUpdate.TotalFilesToCopy = infos.TotalFilesToCopy == null && jobToUpdate.State.Equals(SavingState) ? jobToUpdate.TotalFilesToCopy : infos.TotalFilesToCopy;
        jobToUpdate.TotalFilesSize = infos.TotalFilesSize == null && jobToUpdate.State.Equals(SavingState) ? jobToUpdate.TotalFilesSize : infos.TotalFilesSize;
        jobToUpdate.Progression = infos.Progression == null && jobToUpdate.State.Equals(SavingState) ? jobToUpdate.Progression : infos.Progression;
        jobToUpdate.NbFilesLeftToDo = infos.NbFilesLeftToDo == null && jobToUpdate.State.Equals(SavingState) ? jobToUpdate.NbFilesLeftToDo : infos.NbFilesLeftToDo;
        jobToUpdate.TotalSizeLeftToDo = infos.TotalSizeLeftToDo == null && jobToUpdate.State.Equals(SavingState) ? jobToUpdate.TotalSizeLeftToDo : infos.TotalSizeLeftToDo;
        jobToUpdate.SourceFilePath = infos.SourceFilePath == null && jobToUpdate.State.Equals(SavingState) ? jobToUpdate.SourceFilePath : infos.SourceFilePath;
        jobToUpdate.TargetFilePath = infos.TargetFilePath == null && jobToUpdate.State.Equals(SavingState) ? jobToUpdate.TargetFilePath : infos.TargetFilePath;

        return UpdateJob(jobToUpdate);
        
    }

    public bool AddJob(SaveJob job)
    {
        try
        {
            // Limit of 5 save jobs
            if (GetJobs().Count >= 5)
            {
                return false;
            }

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

            return SaveJob(jobJson);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool DeleteJob(SaveJob job)
    {
        try
        {
            JobStateJsonDefinition jobToDelete = GetJob(job.Name);
            jobToDelete.State = DeletedState;
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
            File.WriteAllText(FilePath, json);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private JobStateJsonDefinition GetJob(string jobName)
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
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(jobsJson, options);
            File.WriteAllText(FilePath, json);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        
    }
}

public class JobStateJsonDefinition
{
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime LastUpdate { get; set; }
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }
    public string State { get; set; }
    public long? TotalFilesToCopy { get; set; } = null;
    public long? TotalFilesSize { get; set; } = null;
    public long? Progression { get; set; } = null;
    public long? NbFilesLeftToDo { get; set; } = null;
    public long? TotalSizeLeftToDo { get; set; } = null;
    public string? SourceFilePath { get; set; } = null;
    public string? TargetFilePath { get; set; } = null;
}
