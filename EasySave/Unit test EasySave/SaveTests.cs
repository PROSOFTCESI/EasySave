
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
    private string CreateFakeInformationFolder(string folderName = "Fake Folder for test")
    {
        string path = Path.Combine(Path.GetTempPath(), "EasySaveTestTemp", folderName);
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

    private void Cleanup()
    {
        string SourcePath = CreateFakeInformationFolder();
        string TargetPathFull = Path.Combine(Path.GetTempPath(), "EasySaveFullTestTemp");
        string TargetPathDiff = Path.Combine(Path.GetTempPath(), "EasySaveDiffTestTemp");

        List<string> pathsToDelete = [SourcePath, TargetPathFull, TargetPathDiff];

        foreach (string path in pathsToDelete){
            if (IsPathExist(path))
            {
                Directory.Delete(path, true);
            }
        }
       

    }


    [Fact]
    public void FullSave_CreateSave()
    {
        Cleanup();
        string SourcePath = CreateFakeInformationFolder();
        string TargetPath = Path.Combine(Path.GetTempPath(), "EasySaveFullTestTemp");

        var FullSave = new FullSave("TestSave", SourcePath, TargetPath);


        var result = FullSave.CreateSave();


        Assert.True(result);
        Assert.True(IsPathExist(TargetPath));
        Assert.Equal(GetDirSize(TargetPath), GetDirSize(SourcePath));
        
    }

    [Fact]
    public void FullSave_Save()
    {
        Cleanup();
        string SourcePath = CreateFakeInformationFolder();
        string TargetPath = Path.Combine(Path.GetTempPath(), "EasySaveFullTestTemp");

        var FullSave = new FullSave("TestSave", SourcePath, TargetPath);
        FullSave.CreateSave();

        long initalSaveSize = GetDirSize(SourcePath);


        CreateFakeFile(Path.Combine(SourcePath, "NewFileToSee.data"));
        Thread.Sleep(2000);
        var result = FullSave.Save();


        Assert.True(result);
        Assert.True(IsPathExist(TargetPath));
        Assert.Equal(GetDirSize(TargetPath), GetDirSize(SourcePath)+ initalSaveSize);
        Cleanup();
    }

    [Fact]
    public void FullSave_Delete()
    {
        Cleanup();
        string SourcePath = CreateFakeInformationFolder();
        string TargetPath = Path.Combine(Path.GetTempPath(), "EasySaveFullTestTemp");

        var FullSave = new FullSave("TestSave", SourcePath, TargetPath);
        FullSave.CreateSave();
        var result = FullSave.DeleteSave();

        Assert.True(result);
        Assert.False(IsPathExist(TargetPath));
        Cleanup();
    }

    [Fact]
    public void DifferentialSave_CreateSave()
    {
        Cleanup();
        string SourcePath = CreateFakeInformationFolder();
        string TargetPath = Path.Combine(Path.GetTempPath(), "EasySaveDiffTestTemp");

        var DifferentialSave = new DifferentialSave("TestSave", SourcePath, TargetPath);


        var result = DifferentialSave.CreateSave();


        Assert.True(result);
        Assert.True(IsPathExist(TargetPath));
        Assert.Equal(GetDirSize(TargetPath), GetDirSize(SourcePath));
        Cleanup();
    }

    [Fact]
    public void DifferentialSave_Save()
    {
        Cleanup();
        string SourcePath = CreateFakeInformationFolder();
        string TargetPath = Path.Combine(Path.GetTempPath(), "EasySaveDiffTestTemp");

        var DifferentialSave = new DifferentialSave("TestSave", SourcePath, TargetPath);
        DifferentialSave.CreateSave();
        CreateFakeFile(Path.Combine(SourcePath, "NewFileToSee.data"));
        var result = DifferentialSave.Save();

        Assert.True(result);
        Assert.True(IsPathExist(TargetPath));
        Assert.Equal(GetDirSize(TargetPath), GetDirSize(SourcePath));
        Cleanup();
    }

    [Fact]
    public void DifferentialSave_Delete()
    {
        Cleanup();
        string SourcePath = CreateFakeInformationFolder();
        string TargetPath = Path.Combine(Path.GetTempPath(), "EasySaveDiffTestTemp");

        var DifferentialSave = new DifferentialSave("TestSave", SourcePath, TargetPath);
        DifferentialSave.CreateSave();
        CreateFakeFile(Path.Combine(SourcePath, "NewFileToSee.data"));
        var result = DifferentialSave.DeleteSave();

        Assert.True(result);
        Assert.False(IsPathExist(TargetPath));
        Cleanup();
    }

    [Fact]
    public void MultriprocessingDifferentialSave_Save()
    {
        Cleanup();
        string SourcePath = CreateFakeInformationFolder();
        string TargetPath = Path.Combine(Path.GetTempPath(), "EasySaveDiffTestTemp");
        

        var DifferentialSave = new DifferentialSave("TestSave", SourcePath, TargetPath);
        JobManager.Instance.NewProcess(DifferentialSave,saveAction.Create);
        JobManager.Instance.NewProcess(DifferentialSave, saveAction.Save);
        JobManager.Instance.NewProcess(DifferentialSave, saveAction.Delete);
        CreateFakeFile(Path.Combine(SourcePath, "NewFileToSee.data"));
        JobManager.Instance.WaitAllTaskFinished();
        Cleanup();
    }


}