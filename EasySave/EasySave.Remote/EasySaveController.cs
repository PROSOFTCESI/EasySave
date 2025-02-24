using System;
using System.Diagnostics;

namespace EasySaveRemote
{
    public class EasySaveController
    {
        private const string EasySavePath = @"C:\Users\Poirr\source\repos\PROSOFTCESI\EasySave\EasySave\EasySave.Graphic\bin\Release\net8.0-windows\EasySave.Graphic.exe"; // Chemin d'EasySave

        public string ExecuteCommand(string command)
        {
            switch (command.ToLower())
            {
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
    }
}
