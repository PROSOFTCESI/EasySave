using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography;
using EasySave.Utils;

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
            // read settings.json dans appdata
            // recup encrypotionKey
            
            return "1245124585";
        }
        public static void EncryptDecrypt(string filePath)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"\"{filePath}\" \"{Key()}\"", // Passer les arguments en les entourant de guillemets
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
    }
}
