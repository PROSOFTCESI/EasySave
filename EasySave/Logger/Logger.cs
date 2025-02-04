using Newtonsoft.Json;

namespace LoggerLib;


/// <summary>
/// Class to write dated JSON logs in the Application Data folder or a custom directory.
/// Depends on Newtonsoft.Json.
/// </summary>
public class Logger
{
    private string LogDirectory;

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
    /// <param name="projectsPath">Custom path for the application folder.</param>
    public void Initialize(string projectName = "LogLib", string? projectsPath= null)
    {
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

    /// <summary>
    /// Logs any object in a dated log file.
    /// </summary>
    /// <param name="toWrite">Object to log.</param>
    /// <returns>True if the log was successfully written, otherwise false.</returns>
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

