using EasySave.Utils.JobStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoSoftLib;
using System.Text.Json;
using System.Net.Http.Json;
using System.IO.Enumeration;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using static System.Net.Mime.MediaTypeNames;

namespace EasySave.Utils
{
    public class SettingsJson
    {
        private static readonly string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasySave");
        private static readonly string FilePath = Path.Combine(FolderPath, "settings.json");

        private static SettingsJson? instance;

        public static SettingsJson GetInstance()
        {
            instance ??= new SettingsJson();
            return instance;
        }
      
        public SettingsJsonDefinition GetContent()
        {
            Initialize();
            SettingsJsonDefinition content = JsonSerializer.Deserialize<SettingsJsonDefinition>(File.ReadAllText(FilePath));
            return content;
        }

        public void Initialize()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            if (!File.Exists(FilePath))
            {
                InitContent();
            }
            
        }

        private void InitContent()
        {
            SettingsJsonDefinition newContent = new SettingsJsonDefinition();

            newContent.Name = "EasySave";
            newContent.EncryptionKey = CryptoSoft.GenerateKey();
            newContent.extensionsToEncrypt = "*";
            newContent.selectedCulture = "fr-FR";
            newContent.logFormat = "json";
            newContent.businessSoftwares = "CalculatorApp";

            string json = JsonConvert.SerializeObject(newContent, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public void Update(SettingsJsonDefinition newContent)
        {
            string json = JsonConvert.SerializeObject(newContent, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        } 
    }

    public class SettingsJsonDefinition
    {
        public string Name { get; set; }
        public string EncryptionKey { get; set; }
        public string selectedCulture { get; set; }
        public string extensionsToEncrypt { get; set; }
        public string logFormat { get; set; }
        public string businessSoftwares { get; set; }
    }
}
