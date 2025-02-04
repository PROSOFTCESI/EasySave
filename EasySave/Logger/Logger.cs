using Newtonsoft.Json;

namespace LoggerLib;


/// <summary>
/// Class to write dated json logs in the Application Data Foldor Repertory custom Repertory 
/// Depend of Newtonsoft.Json
/// </summary>
///
public class Logger
{
    private string LogDirectory;

    private static Logger? Instance = null;

    private Logger() 
    {
        LogDirectory = GetLogDirectory();
    }

    /// <summary>
    /// Singleton of the Logger.
    /// Plese use Initialize before using this instance.
    /// </summary>
    /// <returns>
    /// Unique Instance of the logger.
    /// </returns>
    public static Logger GetInstance()
    {
        Instance ??= new Logger();
        return Instance;
    }

    /// <summary>
    /// Initialisze the Logs Repos
    /// You can choose between initialization by project name (saving Logs in the OS's application Data folder : AppData\Roaming\[projectName] for windows
    /// Or initialize directly with projectsPath and a Project Name with will create 
    /// </summary>
    /// <param name="projectName">Project Name for the Sub directory</param>
    /// <param name="projectsPath">Custom parth for all your application folder</param>
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

    ///<summary>
    ///Log any Object on a Dated log file 
    ///</summary>
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

