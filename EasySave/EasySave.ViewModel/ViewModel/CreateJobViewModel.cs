using EasySave.CustomExceptions;
using LoggerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Graphic3._0.ViewModel;

public class CreateJobViewModel
{
    public static async Task<UserResponse> Create(string name, string sourcePath, string targetPath, string saveType)
    {
        try
        {
            Logger.GetInstance().Log(
                new
                {
                    Type = "Create",
                    Statue = "Start",
                    Time = DateTime.Now,
                    Name = name,
                    SaveType = saveType,
                    SourcePath = sourcePath,
                    DestinationPath = targetPath
                }
            );

            SaveJob newJob;

            if (saveType.Equals("TOTAL"))
            {
                newJob = new FullSave(name, sourcePath, targetPath, true);
            }
            else if (saveType.Equals("DIFFERENTIAL"))
            {
                newJob = new DifferentialSave(name, sourcePath, targetPath, true);
            }
            else
            {
                throw new Exception("Unsupported type of save");
            }

            bool isCreated = await Task.Run(() => JobManager.Instance.NewProcess(newJob,saveAction.Create)); // Exécute CreateSave() dans un nouveau thread
            if (!isCreated)
            {
                Logger.GetInstance().Log(
                    new
                    {
                        Type = "Create",
                        Time = DateTime.Now,
                        Statut = "Error",
                        Message = "Save Job creation failed : " + newJob.Name
                    }
                );
                return new UserResponse(false, "SAVE_JOB_CREATION_FAILED_MESSAGE");
            }

            Logger.GetInstance().Log(
                new
                {
                    Type = "Create",
                    Time = DateTime.Now,
                    Statut = "Success",
                    Message = "Save Job creation is Success : " + newJob.Name
                }
            );
            SaveJob.Instances.Add(newJob);
            return new UserResponse(true, "SAVE_JOB_CREATED_SUCCESSFULLY", name);
        }
        catch (Exception ex)
        {
            if (ex is BusinessSoftwareRunningException)
            {
                return new UserResponse(false, "BUSINESS_SOFTWARE_DETECTED_ERROR");
            }
            return new UserResponse(false, "SAVE_JOB_CREATION_FAILED_MESSAGE");
        }
    }
}
