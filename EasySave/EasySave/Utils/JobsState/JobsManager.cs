using System.IO.Enumeration;
using System.Net.Http.Json;
using System.Text.Json;

namespace EasySave.Utils.JobStates;

internal class JobsManager
{
    private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"EasySave","state.json");

    private static JobsManager? instance;

    public const string FullSaveType = "FullSave";
    public const string DifferentialSaveType = "DifferentialSave";

    public const string SavedState = "SAVED";
    public const string SavingState = "SAVING";
    public const string DeletedState = "DELETED";

    private JobsManager() { }

    public static JobsManager GetInstance()
    {
        instance ??= new JobsManager();
        return instance;
    }
    private List<JobsJson> ReadJson()
    {
        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, "[]");
        }

        string jsonContent = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<JobsJson>>(jsonContent)
                       ?? throw new Exception("Le fichier JSON est vide ou invalide.");
    }

    public List<SaveJob> GetJobs()
    {

        List<SaveJob> jobsList = [];
        List<JobsJson> jobs = ReadJson();

        foreach (JobsJson job in jobs)
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

    public bool UpdateJob(string jobName, JobsJson infos)
    {
        JobsJson jobToUpdate = GetJob(jobName);

        jobToUpdate.LastUpdate = DateTime.Now;
        jobToUpdate.State = infos.State;
        jobToUpdate.TotalFilesToCopy = infos.TotalFilesToCopy;
        jobToUpdate.TotalFilesSize = infos.TotalFilesSize;
        jobToUpdate.Progression = infos.Progression;
        jobToUpdate.NbFilesLeftToDo = infos.NbFilesLeftToDo;
        jobToUpdate.SourceFilePath = infos.SourceFilePath;
        jobToUpdate.TargetFilePath = infos.TargetFilePath;

        return UpdateJob(jobToUpdate);
        
    }

    public bool SaveJob(SaveJob job)
    {
        try
        {
            // Limit of 5 save jobs
            if (GetJobs().Count >= 5)
            {
                return false;
            }

            JobsJson jobJson = new();
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
            JobsJson jobToDelete = GetJob(job.Name);
            jobToDelete.State = DeletedState;
            return UpdateJob(jobToDelete);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool SaveJob(JobsJson job)
    {
        try
        {
            List<JobsJson> jobsJson = ReadJson();
            if (jobsJson.Any(job => job.Name == job.Name && job.State != DeletedState))
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

    private JobsJson GetJob(string jobName)
    {
        List<JobsJson> jobsJson = ReadJson();
        JobsJson job = jobsJson.Find(job => job.Name == jobName) ?? throw new KeyNotFoundException($"Job {jobName} not found");
        return job;
    }

    private bool UpdateJob(JobsJson job)
    {
        try
        {
            List<JobsJson> jobsJson = ReadJson();
            jobsJson[jobsJson.FindIndex(job => job.Name == job.Name && job.State != DeletedState)] = job;
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

public class JobsJson
{
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime LastUpdate { get; set; }
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }
    public string State { get; set; }
    public int? TotalFilesToCopy { get; set; } = null;
    public int? TotalFilesSize { get; set; } = null;
    public int? Progression { get; set; } = null;
    public int? NbFilesLeftToDo { get; set; } = null;
    public int? TotalSizeLeftToDo { get; set; } = null;
    public string? SourceFilePath { get; set; } = null;
    public string? TargetFilePath { get; set; } = null;

}
