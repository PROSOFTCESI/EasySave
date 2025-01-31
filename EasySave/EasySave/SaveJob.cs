using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave
{
    public abstract class SaveJob
    {
        // ATTRIBUTES
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
        public DateTime CreationDate { get; set; }
        
        //CONSTRUCTOR
        protected SaveJob(string name, string sourcePath, string targetPath)
        {
            Name = name;
            SourcePath = sourcePath;
            TargetPath = targetPath;
            CreationDate = DateTime.Now;
        }



        //METHODS
        public bool CreateSave()
        {
            // Create target directory       
            string type = "";
            if (this is FullSave)
                type = "FullSave";
            else if(this is DifferentialSave)
                type = "DifferentialSave";

            string repJobName = Name + "_" + type;

            TargetPath = Path.Combine(TargetPath, repJobName);

            Directory.CreateDirectory(TargetPath);

            // Create a default FULL SAVE
            FullSave(SourcePath, TargetPath, true);
            
            return true;
        }

        private bool FullSave(string sourcePath, string targetPath, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourcePath);

            // Check if the source directory exists
            // TODO We need to andle the case of an inexistant directory. Currently, it crashes
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(targetPath);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(targetPath, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(targetPath, subDir.Name);
                    FullSave(subDir.FullName, newDestinationDir, true);
                }
            }
            return true;
        }

        public abstract bool UpdateSave(string name);

        public abstract bool RestoreSave(string name);

        public abstract bool DeleteSave(string name);

        public abstract override string ToString();
    }
}
