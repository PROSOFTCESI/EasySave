using LoggerLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CryptoSoftLib;
using EasySave.CustomExceptions;
using EasySave.Utils;
using Newtonsoft.Json;
using EasySave.Utils.JobStates;

namespace EasySave
{
    public class DifferentialSave : SaveJob
    {
        //ATTRIBUTES


        //CONTRUCTOR
        public DifferentialSave(string name, string sourcePath, string targetPath, bool checkBusinessSoftwares = false) : base(name, sourcePath, targetPath, checkBusinessSoftwares)
        {
        }

        //METHODS

        
       

        public override bool Save()
        {
            NameLastSave = "DifferentialSave_" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss");

            CheckIfCanRun();

            if (SourcePath == TargetPath)
            {
                Logger.GetInstance().Log(
                       new
                       {
                           Statue = "Error",
                           Message = "Source path and Target path can't be equal"
                       });
                throw new ArgumentException("Source path and Target path can't be equal");
            }

            // Create Target Directory
            Directory.CreateDirectory(TargetPath);

            StateJsonReader.GetInstance().AddJob(this);

            var state = StateJsonReader.GetInstance().GetJob(Name);
            state.NameLastSave = NameLastSave;
            StateJsonReader.GetInstance().UpdateJob(Name, state);
            string saveTargetPath = Path.Combine(TargetPath, NameLastSave);

            // Get information about the source directory
            var dir = new DirectoryInfo(SourcePath);

            // Check if the source directory exists
            if (!dir.Exists)
            {
                Logger.GetInstance().Log(
                     new
                     {
                         Statue = "Error",
                         Message = $"Source directory not found: {dir.FullName}"
                     });
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            }

            // Create the destination directory
            string jsonSaved = Path.Combine(TargetPath, Path.Combine(GetLastSavePath(), ".fileStructure.json"));

            
            Directory.CreateDirectory(saveTargetPath);

            string jsonPath = FileStructureJson.GetInstance().CreateDiffenretialFileStructure(SourcePath, saveTargetPath, jsonSaved);

            FileStructureJson.GetInstance().GetAdvancement(jsonPath);
            // Copy Files
            CopyFiles(jsonPath, saveTargetPath);
            //EncryptFiles
            EncryptFiles(jsonPath, saveTargetPath, true);

            return true;
        }


        public override bool RestoreSave()
        {
            return true;
        }

        public override string ToString()
        {
            return base.ToString() + ", Differential Save";
        }
    }
}
