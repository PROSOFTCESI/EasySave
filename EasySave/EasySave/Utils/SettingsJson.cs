using EasySave.Utils.JobStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoSoftLib;
using System.Text.Json;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace EasySave.Utils
{
    public class SettingsJson
    {
        private static readonly string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasySave");
        private static readonly string FilePath = Path.Combine(FolderPath, "settings.json");

        private static SettingsJson? instance;

        private static SettingsJsonDefinition content = new SettingsJsonDefinition
        {
            Name = "EasySave",
            EncryptionKey = CryptoSoft.GenerateKey(),
            selectedCulture = "FR",
            extensionsToEncrypt = "*",
            logFormat = "json",
        };
        public SettingsJsonDefinition GetContent() { return content; }

        public static SettingsJson GetInstance()
        {
            instance ??= new SettingsJson();
            return instance;
        }
      
        public void Initialize()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            if (!File.Exists(FilePath))
            {
                File.WriteAllText(FilePath, "[]");
            }

            string json = JsonConvert.SerializeObject(content, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public void Update(SettingsJsonDefinition newContent)
        {
            content = newContent;
        } 
    }

    public class SettingsJsonDefinition
    {
        public string Name { get; set; }
        public string EncryptionKey { get; set; }
        public string selectedCulture { get; set; }
        public string extensionsToEncrypt { get; set; }
        public string logFormat { get; set; }
    }
}

