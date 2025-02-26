

using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using EasySave;
using EasySave.Graphic3._0.ViewModel;
using EasySave.Utils.JobStates;


namespace EasySaveRemote
{
    public class EasySaveController
    {
        public async Task<string> ExecuteCommand(string command)
        {
            string[] parts = command.Split(' ');
            string action = parts[0];
            string jobName = parts.Length > 1 ? parts[1] : "";

            switch (action)
            {
                case "list_jobs":
                    return GetSaveJobs();
                case "start_backup":
                    return  StartBackup(jobName);
                case "pause_backup":
                    return PauseBackup(jobName);
                case "update_backup":
                    return await UpdateBackup(jobName);
                case "delete_backup":
                    return DeleteBackup(jobName);
                case "create_backup":
                    return CreateBackup(jobName);
                default:
                    return "Commande inconnue.";
            }
        }

        private string GetSaveJobs()
        {
            var jobs = new MainMenuViewModel().GetJobs();

            if (jobs == null || jobs.Count == 0)
            {
                return "Aucun job disponible.";
            }

            return JsonSerializer.Serialize(jobs);
        }
        private async Task<string> UpdateBackup(string saveJobName)
        { SaveJob saveJob = StateJsonReader.GetInstance().GetJobs().Where(job => job.Name == saveJobName).First();
            bool response = await JobManager.instance.Value.NewProcess(saveJob, saveAction.Save);
            return response ? "OK" : "NOK";
        }
        private string StartBackup(string job) => $"Pause de {job}";
        private string PauseBackup(string job) => $"Pause de {job}";
        private string DeleteBackup(string job) => $"Suppression de {job}";
        private string CreateBackup(string job) => $"Création de {job}";
       
    }
}
