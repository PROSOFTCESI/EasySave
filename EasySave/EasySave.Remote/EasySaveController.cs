

using EasySave.Graphic3._0.ViewModel;

namespace EasySaveRemote
{
    public class EasySaveController
    {
        public string ExecuteCommand(string command)
        {
            string[] parts = command.Split(' ');
            string action = parts[0];
            string jobName = parts.Length > 1 ? parts[1] : "";

            switch (action)
            {
                case "list_jobs":
                    return GetSaveJobs();
                case "start_backup":
                    return StartBackup(jobName);
                case "pause_backup":
                    return PauseBackup(jobName);
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
            string str = "";
            var jobs = new MainMenuViewModel().GetJobs();
            foreach (var job in jobs)
            {
                str += job.ToString();
            }
            return str;
        }
        private string StartBackup(string job) => $"Démarrage de {job}";
        private string PauseBackup(string job) => $"Pause de {job}";
        private string DeleteBackup(string job) => $"Suppression de {job}";
        private string CreateBackup(string job) => $"Création de {job}";
    }
}
