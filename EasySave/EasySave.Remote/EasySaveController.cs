

using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using EasySave;
using EasySave.Graphic3._0.ViewModel;
using EasySave.Utils.JobStates;
using EasySave.ViewModel.ViewModel;


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
                    return await DeleteBackup(jobName);
                case "create_backup":
                    return await CreateBackup(command);
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
        {
            UserResponse response = await UpdateJobViewModel.Update(saveJobName);
            return response.Success ? "OK" : "NOK";
        }

        private string StartBackup(string job) => $"Pause de {job}";
        private string PauseBackup(string job) => $"Pause de {job}";
        private async Task<string> DeleteBackup(string saveJobName)
        {
            UserResponse response = await DeleteJobViewModel.Delete(saveJobName);
            return response.Success ? "OK" : "NOK";
        }
        private async Task<string> CreateBackup(string command)
        {
            Regex regex = new Regex("\"([^\"]+)\"");
            MatchCollection matches = regex.Matches(command);

            if (matches.Count < 4)
            {
                return "Paramètres insuffisants pour créer un job.";
            }

            string jobName = matches[0].Groups[1].Value;
            string sourcePath = matches[1].Groups[1].Value;
            string targetPath = matches[2].Groups[1].Value;
            string saveType = matches[3].Groups[1].Value.ToUpper();

            UserResponse response = await CreateJobViewModel.Create(jobName, sourcePath, targetPath, saveType);
            return response.Success ? "Job créé avec succès." : "Échec de la création du job.";
        }

    }
}
