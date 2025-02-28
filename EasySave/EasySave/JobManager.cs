using EasySave.Utils.JobStates;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace EasySave;

public enum saveAction
{
    Create,
    Restore,
    Delete,
    Save,
    Decrypte,
    Play,
    Pause,
    Stop
}

public class JobManager
{
    //A verifier
    public static readonly Lazy<JobManager> instance = new(() => new JobManager());
    public static JobManager Instance => instance.Value;

    //Avoid 2 process at the same job at the same time
    private static readonly List<SaveJob> ProcessingJobs = [];
    private static readonly ConcurrentQueue<(SaveJob, saveAction, TaskCompletionSource<bool>)> ProcessQueue = [];
    private static List<SaveJob> saveJobs = [];


    //Allow 3 task Max
    private static readonly int MAXWORKER = 3;
    static SemaphoreSlim semaphore = new(MAXWORKER); //  Allow max. 3 task at the same time

    private JobManager()
    {
        Thread loopThread = new(LaunchMainloop);
        loopThread.Start();
    }


    public Task<bool> NewProcess(SaveJob job, saveAction action)
    {
        var tcs = new TaskCompletionSource<bool>();
        ProcessQueue.Enqueue((job, action, tcs));
        return tcs.Task;
    }

    public void WaitAllTaskFinished()
    {

        while (ProcessingJobs.Count > 0 && !ProcessQueue.IsEmpty)
        {
            Thread.Sleep(100);
        }

    }

    async private void LaunchMainloop()
    {
        while (true)
        {
            if (!ProcessQueue.IsEmpty)
            {

                await semaphore.WaitAsync(); // Wait for a Free Thread

                bool res = ProcessQueue.TryDequeue(out (SaveJob, saveAction, TaskCompletionSource<bool>) result);

                if (!ProcessingJobs.Where(j => j.Name == result.Item1.Name).Any())
                {

                    ProcessingJobs.Add(result.Item1);

                    new Task(() => ProcessNextRequest(result.Item1, result.Item2, result.Item3)).Start();
                }
                else
                {

                    ProcessQueue.Enqueue(result);
                }
            }
            else
            {
                Thread.Sleep(100);
            }
        }
    }

    static private void ProcessNextRequest(SaveJob job, saveAction action, TaskCompletionSource<bool> tcs)
    {
        try
        {
            bool result = action switch
            {
                saveAction.Create => job.CreateSave(),
                saveAction.Restore => job.RestoreSave(),
                saveAction.Save => job.Save(),
                saveAction.Delete => job.DeleteSave(),
                saveAction.Decrypte => job.DeleteSave(),
                saveAction.Stop => job.DeleteSave(),
                _ => false,
            };
            tcs.SetResult(result);
        }
        catch (Exception ex)
        {
            tcs.SetResult(false);
            LoggerLib.Logger.GetInstance().Log(ex);
            throw;
        }
        finally
        {
            ProcessingJobs.Remove(job);
            semaphore.Release();
        }
    }
}