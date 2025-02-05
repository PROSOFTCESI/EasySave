using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasySave
{
    internal class DifferentialSave : SaveJob
    {
        //ATTRIBUTES


        //CONTRUCTOR
        public DifferentialSave(string name, string sourcePath, string targetPath) : base(name, sourcePath, targetPath)
        {
        }

        //METHODS

        public string GetLastFullSavePath()
        {
            if (!Directory.Exists(TargetPath))
            {
                Console.WriteLine("Le répertoire spécifié n'existe pas.");
                return null;
            }

            Regex regex = new Regex(@"^FullSave_(\d{2}_\d{2}_\d{4}-\d{2}_\d{2}_\d{2})$");

            var latestSave = Directory.GetDirectories(TargetPath)
                .Select(Path.GetFileName)
                .Where(name => regex.IsMatch(name))
                .Select(name => new { Name = name, Date = DateTime.ParseExact(name.Substring(9), "dd_MM_yyyy-HH_mm_ss", null) })
                .OrderByDescending(entry => entry.Date)
                .FirstOrDefault();

            return latestSave?.Name;
        }
        

        private void CreateDifferentialSave(string source, string fullsave, string diffsave)
        {
            Directory.CreateDirectory(diffsave);

            DirectoryInfo sourceDir = new DirectoryInfo(source);
            DirectoryInfo fullsaveDir = new DirectoryInfo(fullsave);

            FileInfo[] sourceFiles = sourceDir.GetFiles();
            FileInfo[] fullsaveFiles = fullsaveDir.GetFiles();

            //Copy New and modified Files
            foreach(FileInfo sFile in sourceFiles)
            {
                string savedFile = Path.Combine(fullsave, sFile.Name);
                if(!File.Exists(savedFile) || File.GetLastWriteTime(sFile.FullName) > File.GetLastWriteTime(savedFile))
                {
                    Console.WriteLine(sFile.Name);
                    sFile.CopyTo(Path.Combine(diffsave, sFile.Name));
                }                    
            }

            // Recursivity
            foreach(DirectoryInfo dir in sourceDir.GetDirectories())
            {                
                CreateDifferentialSave(Path.Combine(source, dir.Name), Path.Combine(fullsave, dir.Name), Path.Combine(diffsave, dir.Name));
            }    
        }

        public override bool Save()
        {
            string fullSave = Path.Combine(TargetPath, GetLastFullSavePath());
            string diffsave = Path.Combine(TargetPath, "DiffenrentialSave_" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss"));
            CreateDifferentialSave(SourcePath, fullSave, diffsave);
            return true;
        }


        public override bool RestoreSave()
        {
            return true;
        }

        public override string ToString()
        {
            string str = "";

            return str;
        }
    }
}
