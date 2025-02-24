using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace EasySave;

public enum saveAction
{
    Create,
    Restore,
    Delete,
    Save,
    Decrypte
}

public class JobManager
{
    //A verifier
    public static readonly Lazy<JobManager> instance = new(() => new JobManager());
    public static JobManager Instance => instance.Value;

    //Avoid 2 process at the same job at the same time
    private static readonly List<SaveJob> ProcessingJobs = [];
    private static readonly ConcurrentQueue<(SaveJob, saveAction, TaskCompletionSource<bool>)> ProcessQueue = [];

    //Allow 3 task Max
    private static readonly int MAXWORKER = 3;
    static SemaphoreSlim semaphore = new SemaphoreSlim(MAXWORKER); // Allow 3 task at the same time

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

                ProcessQueue.TryDequeue(out (SaveJob, saveAction,TaskCompletionSource<bool>) result);
                if (!ProcessingJobs.Where(j => j.Name == result.Item1.Name).Any()) {
                   
                    new Task(() => ProcessNextRequest(result.Item1, result.Item2, result.Item3)).Start();
                }
                else
                {
                    ProcessQueue.Enqueue(result);
                }   

            }
             
            {
                Thread.Sleep(100);
            }
        }
    }

    static private void ProcessNextRequest(SaveJob job, saveAction action, TaskCompletionSource<bool> tcs)
    {
        try
        {
            if (ProcessingJobs.Contains(job))
            {
                ProcessingJobs.Add(job);
                bool result = action switch
                {
                    saveAction.Create => job.CreateSave(),
                    saveAction.Restore => job.RestoreSave(),
                    saveAction.Save => job.Save(),
                    saveAction.Delete => job.DeleteSave(),
                    saveAction.Decrypte => job.DeleteSave(),
                    _ => false,
                };
                tcs.SetResult(result);
            }
            else
            {
                tcs.SetResult(false);
            }
        }
        catch (Exception ex)
        {
            tcs.SetResult(false);
            LoggerLib.Logger.GetInstance().Log(ex);
            throw;
        }
        finally
        {
            semaphore.Release();
        }
    }
}