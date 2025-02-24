using EasySave.CustomExceptions;
using EasySave.Utils.JobStates;

namespace EasySave.Graphic3._0.ViewModel;

internal class UpdateJobViewModel
{
    public static async Task<UserResponse> Update(SaveJob saveJob)
    {
        try
        {
            bool success = await Task.Run(() => saveJob.Save()); // Exécute Save() dans un nouveau thread
            if (success)
            {
                return new UserResponse(true, "SAVE_JOB_UPDATED_SUCCESSFULLY", saveJob.Name);
            }
            else
            {
                return new UserResponse(false, "SAVE_JOB_UPDATE_FAILED_MESSAGE");
            }
        }
        catch (Exception ex)
        {
            if (ex is BusinessSoftwareRunningException)
            {
                return new UserResponse(false, "BUSINESS_SOFTWARE_DETECTED_ERROR");
            }
            return new UserResponse(false, "SAVE_JOB_UPDATE_FAILED_MESSAGE");
        }
        finally
        {
            saveJob.ResetState();
        }
    }
    
    public static long? GetSaveJobProgress(string name)
    {
        var progression = StateJsonReader.GetInstance().GetJob(name).Progression;
        return progression == 100 || progression == 0 ? 100 : progression;
    }
}
