
using EasySave;
using System.IO;
using System;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
namespace Unit_test_EasySave;

public class SaveTests
{
    const string KILLINGNAME = "Test_Repertoi re!@#$% ^&(){}[]+=~;,'.txt";

    private bool IsPathExist(string path)
    {
        return Path.Exists(path);
    }

    private void CreateFakeFile(string filePath)
    {
        long fileSize = RandomNumberGenerator.GetInt32(30000); // Taille en octets (ex: 1 Mo)

        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            fs.SetLength(fileSize); // Définit la taille du fichier
        }


    }
    private string CreateFakeInformationFolder()
    {
        string path = Path.Combine(Path.GetTempPath(), "EasySaveTestTemp", "Fake Folder for test");
        if (IsPathExist(path)) { Directory.Delete(path,true); }
        Directory.CreateDirectory(path);
        CreateFakeFile(Path.Combine(path, "Temporary.txt"));
        CreateFakeFile(Path.Combine(path, "Temporary.png"));
        CreateFakeFile(Path.Combine(path, "nothing.dll"));
        string SubDirPath = Path.Combine(path, KILLINGNAME);
        Directory.CreateDirectory(SubDirPath);
        CreateFakeFile(Path.Combine(SubDirPath, KILLINGNAME));

        return path;
    }

    private long GetDirSize(string path)
    {
        return Directory.EnumerateFiles(path, "*", System.IO.SearchOption.AllDirectories)
                                     .Sum(file => new FileInfo(file).Length);
    }

    [Fact]
    public void Save_ShouldReturnTrue()
    {   string SourcePath = CreateFakeInformationFolder();
        string TargetPath = Path.Combine(Path.GetTempPath(), "EasySaveTestTemp");
        
        var differentialSave = new DifferentialSave("TestSave", SourcePath, TargetPath);

        
        var result = differentialSave.Save();


        Assert.True(result);
        Console.WriteLine(TargetPath);
        Assert.True(IsPathExist(TargetPath));
        Assert.Equal(GetDirSize(TargetPath), GetDirSize(SourcePath));
    }


    [Fact]
    public void Test1()
    {
            
    }
}