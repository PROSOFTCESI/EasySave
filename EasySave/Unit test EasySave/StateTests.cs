using System.Security.Cryptography;
using EasySave;
using EasySave.Utils.JobStates;


namespace Unit_test_EasySave;

using Xunit;

public class StateTests
{
    private readonly string StateJsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasySave","state.json");

    private readonly string SourcePath = Path.Combine(Path.GetTempPath(), "SourceStateTest");
    private readonly string TargetPathFull = Path.Combine(Path.GetTempPath(), "EasySaveFullTestTemp");
    private readonly string TargetPathDiff = Path.Combine(Path.GetTempPath(), "EasySaveDiffTestTemp");



    public StateTests()
    { 
        if(!Path.Exists(SourcePath))
        {
            Directory.CreateDirectory(SourcePath);
        }
    }

    private static void CreateFakeFile(string filePath)
    {
        long fileSize = RandomNumberGenerator.GetInt32(30000); 
        using (FileStream fs = new(filePath, FileMode.Create, FileAccess.Write))
        {
            fs.SetLength(fileSize); // Définit la taille du fichier
        }
    }

    private void Cleanup()
    {
        string fakeFilePath = Path.Combine(SourcePath, "testFile.data");

        //Delete State file
        if (File.Exists(StateJsonPath))
        {
            File.Delete(StateJsonPath);
        }
        if (File.Exists(fakeFilePath))
        {
            File.Delete(fakeFilePath);
        }
        CreateFakeFile(fakeFilePath);

    }

    [Fact]
    public void GetJobs_AddJobs_JsonStateReader()
    {
        Cleanup();
        FullSave FullsaveJob = new FullSave("testeSave",SourcePath,TargetPathFull);
        bool result = StateJsonReader.GetInstance().AddJob(FullsaveJob);
        var allJobs = SaveJob.Instances;

        Assert.True(result);
        Assert.True(allJobs[0].ToString() == FullsaveJob.ToString());
    }
    
    //Deprecated
    //[Fact]
    //public void Verify5JobsMax_JsonStateReader()
    //{
    //    Cleanup();
    //    List<string> jobsName = ["TesteSave1", "TesteSave2", "TesteSave3", "TesteSave4", "TesteSave5", "TesteSave6",];
    //    foreach (string jobName in jobsName)
    //    {
    //        FullSave FullsaveJob = new(jobName, SourcePath, TargetPathFull);
    //        StateJsonReader.GetInstance().AddJob(FullsaveJob);
    //    }
    //    Assert.Equal(5,StateJsonReader.GetInstance().GetJobs().Count);
    //}

    [Fact]
    public void Delete_JsonStateReader()
    {
        Cleanup();
        FullSave FullsaveJob = new FullSave("testeSave", SourcePath, TargetPathFull);
       StateJsonReader.GetInstance().AddJob(FullsaveJob);
        bool result = StateJsonReader.GetInstance().DeleteJob(FullsaveJob);

        Assert.True(result);
        Assert.Empty(SaveJob.Instances);
    }

    [Fact]
    public void UpdateJob_JsonStateReader()
    {
        Cleanup();
        FullSave FullsaveJob = new FullSave("testeSave", SourcePath, TargetPathFull);

        StateJsonReader.GetInstance().AddJob(FullsaveJob);
        JobStateJsonDefinition updatedDef = new()
        {
            State = StateJsonReader.SavingState,
        };

        bool result = StateJsonReader.GetInstance().UpdateJob("testeSave", updatedDef);

        Assert.True(result);

        // Future improve of the assert to implement
        //SaveJob updatedJob = StateJsonReader.GetInstance().GetJobs()[0];
        //Assert.Equal(StateJsonReader.SavingState, updatedJob.State);
    }
}
