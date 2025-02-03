using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json.Nodes;

namespace EasySave
{
    internal  class Logger
    {
        private static string LogDirectory = "EasySave";

        private static Logger? Instance = null;
        
        private Logger()
        {
             }
        public static Logger GetInstance()
        {
            Instance ??= new Logger();
            return Instance;
        }
        public static void Initialize(string projectName = "EasySave", string? projectsPath= null)
        {

            LogDirectory = Path.Combine(string.IsNullOrWhiteSpace(projectsPath) ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) : projectsPath, projectName, "logs");
            if (!Path.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
           
        }

        private string GetLogPath()
        {
            return Path.Combine(LogDirectory,DateTime.Now.Date.ToString("yyyy-MM-dd") + ".json");
        }

        private JArray ReadLog()
        {
            try {
                return JArray.Parse(File.ReadAllText(GetLogPath()));
            }catch (Exception)
            {
                return [];
            }
            

            
        }

        private bool WriteFile(string text)
        {
          
            File.WriteAllText(GetLogPath(), text);
            return true;
        }


        public void Log(Object toWrite)
        {
            string jsonToAdd = JsonConvert.SerializeObject(toWrite, Formatting.Indented);

            JArray jsonObject = ReadLog();
            
            jsonObject.Add(JObject.Parse(jsonToAdd));
            WriteFile(jsonObject.ToString(Formatting.Indented));
        }

      
    }
}
