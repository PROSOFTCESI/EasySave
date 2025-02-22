using EasySave.Utils;
using EasySave;
using EasySave.Utils.JobStates;
using LoggerLib;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
Console.WriteLine("Hello, World!");

SettingsJson.GetInstance().Initialize();
Logger.GetInstance().Initialize("EasySave", Logger.LogExportType.json);
new ConsoleManager().Launch();