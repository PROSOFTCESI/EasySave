using System;
using System.Diagnostics;


using EasySave.Graphic3._0.ViewModel;

namespace EasySaveRemote
{
    public class EasySaveController
    {
        private const string EasySavePath = @"C:\Users\Poirr\source\repos\PROSOFTCESI\EasySave\EasySave\EasySave.Graphic\bin\Release\net8.0-windows\EasySave.Graphic.exe"; // Chemin d'EasySave

        public string ExecuteCommand(string command)
        {
            switch (command.ToLower())
            {
                case "list_jobs":
                    return GetSaveJobs();
                case "start_backup":
                    return StartBackup();
                case "stop_backup":
                    return StopBackup();
                case "status":
                    return "EasySave opérationnel.";
                default:
                    return "Commande inconnue.";
            }
        }

        private string StartBackup()
        {
            try
            {
                Process.Start(EasySavePath); // Lancer EasySave
                return "Backup démarré.";
            }
            catch (Exception ex)
            {
                return $"Erreur : {ex.Message}";
            }
        }

        private string StopBackup()
        {
            // Logique pour arrêter un processus spécifique d'EasySave
            return "Arrêt du backup non implémenté.";
        }
        private string StartBackup(string job) => $"Démarrage de {job}";
        private string PauseBackup(string job) => $"Pause de {job}";
        private string DeleteBackup(string job) => $"Suppression de {job}";
        private string CreateBackup(string job) => $"Création de {job}";
    }
}
