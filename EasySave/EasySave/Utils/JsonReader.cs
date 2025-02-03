using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EasySave.Utils
{
    public class JsonReader
    {
        private string basePath;
        private Dictionary<string, string>? messages;

        public JsonReader(string basePath, string fileName)
        {
            this.basePath = basePath;
            LoadMessages(fileName);
        }

        private void LoadMessages(string fileName)
        {
            string filePath = Path.Combine(basePath, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Le fichier {filePath} est introuvable.");
            }

            string jsonContent = File.ReadAllText(filePath);
            messages = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent)
                       ?? throw new Exception("Le fichier JSON est vide ou invalide.");
        }
        public string? GetMessage(string key)
        {
            if (messages != null && messages.TryGetValue(key, out string? value))
            {
                return value;
            }
            return null;
        }
    }
}
