using EasySave.Utils;
using EasySave;
using EasySave.Utils.JobStates;
using LoggerLib;

Console.WriteLine("Hello, World!");
//FullSave save = new FullSave("TEST1", "C:\\Users\\33641\\Documents\\TestSource", "C:\\Users\\33641\\Documents\\TestDestination");
//save.CreateSave();

//JobsJson infos = new()
//{
//    State = JobsManager.SavingState,
//    TotalFilesToCopy = 3200,
//    TotalFilesSize = 1240312777,
//    Progression = 32,
//    NbFilesLeftToDo = 1438,
//    TotalSizeLeftToDo = 640312777,
//    SourceFilePath = "C:\\progSys\\Projet\\gr6_save\\EasySave_Group6_1.1\\api-ms-win-core-errorhandling-l1-1-0.dll",
//    TargetFilePath = "D:\\save\\Projet\\gr6_save\\EasySave_Group6_1.1\\api-ms-win-core-errorhandling-l1-1-0.dll"
//};

//JobsManager.GetInstance().UpdateJob("Save2", infos);
//var test = JobsManager.GetInstance().GetJobs();

Logger.GetInstance().Initialize("EasySave");
new ConsoleManager().Launch();