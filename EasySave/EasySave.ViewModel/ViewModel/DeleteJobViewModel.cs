using EasySave.CustomExceptions;
using EasySave.Graphic3._0.ViewModel;
using EasySave.Utils.JobStates;
using LoggerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.ViewModel.ViewModel;

public class DeleteJobViewModel
{
    public static async Task<UserResponse> Delete(string saveJobName)
    {
        try
        {
            SaveJob jobToDelete = SaveJob.GetJob(saveJobName);
            bool success = await Task.Run(() => jobToDelete.DeleteSave());
            if (!success)
            {
                throw new Exception();
            }
            return new UserResponse(true, "SAVE_JOB_DELETED_SUCCESSFULLY", jobToDelete.Name);
        }
        catch (Exception ex)
        {
            return new UserResponse(false, "SAVE_JOB_DELETION_FAILED_MESSAGE");
        }
    }
}
