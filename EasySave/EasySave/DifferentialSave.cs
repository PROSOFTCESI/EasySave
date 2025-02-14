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

        public string GetLastFullSavePath()
        {           

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
            CheckIfCanRun();            

            DirectoryInfo sourceDir = new DirectoryInfo(source);

            FileInfo[] sourceFiles = sourceDir.GetFiles();

            //Copy New and modified Files
            foreach(FileInfo sFile in sourceFiles)
            {
                CheckIfCanRun();
                if (Directory.Exists(fullsave))
                {
                    DirectoryInfo fullsaveDir = new DirectoryInfo(fullsave);
                    FileInfo[] fullsaveFiles = fullsaveDir.GetFiles();
                    string savedFile = Path.Combine(fullsave, sFile.Name);

                    if (!File.Exists(savedFile) || File.GetLastWriteTime(sFile.FullName) > File.GetLastWriteTime(savedFile))
                    {
                        if (!Directory.Exists(diffsave))
                            Directory.CreateDirectory(diffsave);
                        
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        string newFile = Path.Combine(diffsave, sFile.Name);
                        sFile.CopyTo(newFile);
                        CryptoSoft.EncryptDecryptFile(newFile);
                        stopwatch.Stop();
                        var test = Logger.GetInstance();
                        Logger.GetInstance().Log(
                        new
                        {
                            SaveJobName = Name,
                            FileSource = SourcePath + "/" + sFile.Name,
                            FileTarget = TargetPath + "/" + sFile.Name,
                            FileSize = sFile.Length,
                            Time = DateTime.Now,
                            FileTransferTime = stopwatch.ElapsedMilliseconds
                        });
                    }
                }
                else //directory is new
                {
                    Directory.CreateDirectory(diffsave);
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    string newFile = Path.Combine(diffsave, sFile.Name);
                    sFile.CopyTo(newFile);
                    CryptoSoft.EncryptDecryptFile(newFile);
                    stopwatch.Stop();
                    var test = Logger.GetInstance();
                    Logger.GetInstance().Log(
                    new
                    {
                        SaveJobName = Name,
                        FileSource = SourcePath + "/" + sFile.Name,
                        FileTarget = TargetPath + "/" + sFile.Name,
                        FileSize = sFile.Length,
                        Time = DateTime.Now,
                        FileTransferTime = stopwatch.ElapsedMilliseconds
                    });
                }
            }
            // new Dir DDD not in FS. 

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
            Directory.CreateDirectory(diffsave);
            CreateDifferentialSave(SourcePath, fullSave, diffsave);
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
