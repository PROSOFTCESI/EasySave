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

FullSave save = new FullSave("TEST", "C:\\Users\\Milan\\Desktop\\projetCESI\\Tests\\TestDiff", "C:\\Users\\Milan\\Desktop\\projetCESI\\Saves\\NewSave");

save.CreateSave();
//save.Save();

//FileStructureJson.GetAdvancement("C:\\Users\\Milan\\Desktop\\projetCESI\\Saves\\NewSave\\FullSave_20_02_2025-13_32_47\\.fileStructure.json");

//Directory.CreateDirectory("C:\\Users\\Milan\\Desktop\\projetCESI\\Saves\\NewSave\\Test\\test")*/
