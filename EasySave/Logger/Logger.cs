using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Xml.Serialization;

using System.Reflection;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace LoggerLib;

/// <summary>
/// Class to write dated JSON logs in the Application Data folder or a custom directory.
/// Depends on Newtonsoft.Json.
/// </summary>

public class Logger
{
    public enum LogExportType
    {
        json,
        xml,
    };

    private string LogDirectory;
    private LogExportType ExportType;
    private static Logger? Instance = null;

    private Logger() 
    {
        LogDirectory = GetLogDirectory();
    }

    /// <summary>
    /// Singleton instance of the Logger.
    /// Please use Initialize before using this instance.
    /// </summary>
    /// <returns>
    /// Unique instance of the Logger.
    /// </returns>
    public static Logger GetInstance()
    {
        Instance ??= new Logger();
        return Instance;
    }

    /// <summary>
    /// Initializes the logs repository.
    /// You can choose between initialization by project name (saving logs in the OS's application data folder: AppData\Roaming\[projectName] for Windows)  
    /// or initializing directly with a custom path and a project name, which will create the logs in the specified directory.
    /// </summary>
    /// <param name="projectName">Project name for the subdirectory.</param>
    /// <param name="exportType">Custom path for the application folder.</param>
    /// <param name="projectsPath">Custom path for the application folder.</param>
    public void Initialize(string projectName = "LogLib", LogExportType exportType = LogExportType.json, string? projectsPath = null)
    {
        ExportType = exportType;
        LogDirectory = GetLogDirectory(projectName, projectsPath);
        if (!Path.Exists(LogDirectory))
        {
            Directory.CreateDirectory(LogDirectory);
        }
    }
  
    private string GetLogDirectory(string projectName = "LogLib", string? projectsPath = null)
    {

        return Path.Combine(string.IsNullOrWhiteSpace(projectsPath) ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) : projectsPath, projectName, "logs");
    }

    private string GetLogPath()
    {
        return Path.Combine(LogDirectory,DateTime.Now.Date.ToString("yyyy-MM-dd") +"." + ExportType.ToString());
    }

    public class LogClass{
        public object Details;
        public LogClass()
        {
            Details = null;
        }

        public LogClass(object details) 
        {
            Details = details; 
        }
    };

    public static LogClass ConvertTo<LogClass>(object source) where LogClass : new()
    {
        LogClass result = new LogClass();
        Type sourceType = source.GetType();
        Type targetType = typeof(LogClass);

        foreach (PropertyInfo sourceProp in sourceType.GetProperties())
        {
            PropertyInfo targetProp = targetType.GetProperty(sourceProp.Name);
            if (targetProp != null && targetProp.CanWrite)
            {
                object value = sourceProp.GetValue(source);
                targetProp.SetValue(result, value);
            }
        }
        return result;

    }

    private bool WriteFile(string text)
    {
        try 
        {
            File.AppendAllText(GetLogPath(), text);
            return true;
        }
        catch 
        {
            return false;
        }
    }

    /// <summary>
    /// Logs any object in a dated log file.
    /// </summary>
    /// <param name="toWrite">Object to log.</param>
    /// <returns>True if the log was successfully written, otherwise false.</returns>
    public bool Log(object toWrite)
    {
        switch (ExportType) {
            case LogExportType.json:
                string jsonToAdd = JsonConvert.SerializeObject(toWrite, (Newtonsoft.Json.Formatting)Formatting.Indented) + ",\n";
                return WriteFile(jsonToAdd);
            case LogExportType.xml:
                var xmlToWrite = new
                {
                    Log = toWrite
                };
                var jsonText = JsonConvert.SerializeObject(xmlToWrite);           // convert to JSON
                XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText); // convert JSON to XML Document
                return WriteFile(doc.OuterXml + "\n");
            default:
                return false;
        };     
    }
}
