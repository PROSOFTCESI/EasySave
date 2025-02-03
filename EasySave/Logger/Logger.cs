using Newtonsoft.Json;

namespace LoggerLib;

public  class Logger
{

    private static string LogDirectory = "EasySave";

    private static Logger? Instance = null;

    private Logger() {}

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

    private static string GetLogPath()
    {
        return Path.Combine(LogDirectory,DateTime.Now.Date.ToString("yyyy-MM-dd") + ".json");
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

    public bool Log(Object toWrite)
    {
        try
        {
            string jsonToAdd = JsonConvert.SerializeObject(toWrite, Formatting.Indented) + ",\n";
            return WriteFile(jsonToAdd);
        }
        catch 
        { 
            return false; 
        }
    }
}
