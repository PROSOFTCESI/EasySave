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
        public abstract bool CreateSave(string name, string sourcePath, string targetPath);

        public abstract bool UpdateSave(string name);

        public abstract bool RestoreSave(string name);

        public abstract bool DeleteSave(string name);

        public abstract override string ToString();
    }
}
