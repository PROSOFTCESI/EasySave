using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography;
using System.Xml.Linq;
using EasySave.Utils;
using EasySave.Utils.JobStates;
using Newtonsoft.Json.Linq;

namespace CryptoSoftLib
{
    public static class CryptoSoft
    {
        private static string currentDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        private static string exePath = Path.Combine(currentDir, "CryptoSoft/CryptoSoft.exe");   
        
        private static string Key()
        {
            return SettingsJson.GetInstance().GetContent().EncryptionKey;
        }

        public static string GenerateKey()
        {              
            return Guid.NewGuid().ToString();
        }
        public static void EncryptDecryptFile(string filePath, string key = null)
        {
            if(key is null){
                key = Key();
            }

            if (ExtentionToEncrypt()[0] != "*")
            {
                if (!ExtentionToEncrypt().Contains(new FileInfo(filePath).Extension))
                {
                    Console.WriteLine("non");
                    return;
                }
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"\"{filePath}\" \"{key}\"", // Passer les arguments en les entourant de guillemets
                RedirectStandardOutput = true, // Pour récupérer la sortie si nécessaire
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'exécution : " + ex.Message);
            }
        }

        public static void EncryptDecryptFolder(string folder, string key = null)
        {
            if(key is null)
            {
                key = Key();
            }

            // Get information about the source directory
            var dir = new DirectoryInfo(folder);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {                
                CryptoSoft.EncryptDecryptFile(file.FullName, key);                
            }

            // RECURSIVITY : Copy the files from the sub directories
            foreach (DirectoryInfo subDir in dirs)
            {
                EncryptDecryptFolder(subDir.FullName, key);
            }
        }

        public static string[] ExtentionToEncrypt()
        {
           return SettingsJson.GetInstance().GetContent().extensionsToEncrypt.Split(' ');
        }
    }
}
