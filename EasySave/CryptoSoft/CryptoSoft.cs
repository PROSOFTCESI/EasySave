using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography;

namespace CryptoSoftLib
{
    public static class CryptoSoft
    {
        public static string currentDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        public static string exePath = Path.Combine(currentDir, "CryptoSoft/CryptoSoft.exe");
        public static void EncryptDecrypt(string filePath, string key)
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
