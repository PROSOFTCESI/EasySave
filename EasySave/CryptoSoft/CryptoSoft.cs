using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography;

namespace CryptoSoftLib
{
    public static class CryptoSoft
    {
        private static string currentDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        private static string exePath = Path.Combine(currentDir, "CryptoSoft/CryptoSoft.exe");

        private static string key = GetKey();


        private static string GetKey()
        {
            // read settings.json dans appdata
            // recup encrypotionKey
            key = "1245124585";
            return key;
        }
        public static void EncryptDecrypt(string filePath)
        {           
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
    }
}
