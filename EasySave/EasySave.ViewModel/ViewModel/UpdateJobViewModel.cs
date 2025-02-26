using EasySave.CustomExceptions;
using EasySave.Utils.JobStates;

namespace EasySave.Graphic3._0.ViewModel;

public class UpdateJobViewModel
{
    public static async Task<UserResponse> Update(string saveJobName)
    {
        SaveJob? saveJob = null;
        try
        {
            saveJob = SaveJob.GetJob(saveJobName) ?? throw new KeyNotFoundException("Job name key not found");
            if (saveJob.State.Equals(StateJsonReader.SavedState))
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
                    if (ex is not BusinessSoftwareRunningException && ex is not PlayPauseStopException)
                    {
                        saveJob?.ResetState();
                    }
                    throw;
                }
            }
            else if (saveJob.State.Equals(StateJsonReader.SavingState) || saveJob.State.Equals(StateJsonReader.EncryptingState))
            {
                await Task.Run(() => saveJob.Pause());
                return UserResponse.GetEmptyUserResponse();
            }
            else if (saveJob.State.Equals(StateJsonReader.PausedState)) 
            {
                await Task.Run(() => saveJob.Play());
                return UserResponse.GetEmptyUserResponse();
            }
            else
            {
                throw new Exception("Unknown state");
            }
        }
        catch (Exception ex)
        {
            if (ex is BusinessSoftwareRunningException)
            {
                return new UserResponse(false, "BUSINESS_SOFTWARE_DETECTED_ERROR");
            }
            if (ex is PlayPauseStopException)
            {
                return UserResponse.GetEmptyUserResponse();
            }
            return new UserResponse(false, "SAVE_JOB_UPDATE_FAILED_MESSAGE");
        }
    }

    public static async Task<UserResponse> Stop(string saveJobName)
    {
        SaveJob? saveJob = null;
        try
        {
            saveJob = SaveJob.GetJob(saveJobName) ?? throw new KeyNotFoundException("Job name key not found");
            await Task.Run(() => saveJob.Stop());
            return UserResponse.GetEmptyUserResponse();
        }
        catch (Exception ex)
        {
            if (ex is BusinessSoftwareRunningException)
            {
                return new UserResponse(false, "BUSINESS_SOFTWARE_DETECTED_ERROR");
            }
            if (ex is PlayPauseStopException)
            {
                return UserResponse.GetEmptyUserResponse();
            }
            return new UserResponse(false, "SAVE_JOB_UPDATE_FAILED_MESSAGE");
        }
    }

    public static long? GetSaveJobProgress(string name)
    {
        var progression = StateJsonReader.GetInstance().GetJob(name).Progression;
        return progression == 100 || progression == 0 ? 100 : progression;
    }
}
