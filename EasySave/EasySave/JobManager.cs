using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public static readonly Lazy<JobManager> instance = new(() => new JobManager());
    public static JobManager Instance => instance.Value;

    //Avoid 2 process at the same job at the same time
    private static readonly List<SaveJob> ProcessingJobs = [];
    private static readonly ConcurrentQueue<(SaveJob, saveAction)> ProcessQueue = [];
    //Allow 3 task Max
    private static readonly int MAXWORKER = 3;
    static SemaphoreSlim semaphore = new SemaphoreSlim(MAXWORKER); // Allow 3 task at the same time

    private JobManager()
    {
        
        Thread loopThread = new(LaunchMainloop);
        loopThread.Start();
    }

    public void NewProcess(SaveJob job, saveAction action)
    {
        ProcessQueue.Enqueue((job, action));
    }

    public void WaitAllTaskFinished()
    {
        
        while (ProcessingJobs.Count > 0 && ProcessQueue.Count > 0) {
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
                new Task(() => ProcessNextRequest()).Start();
            }
            else
            {
                Thread.Sleep(100);
            }

        }
    }
    

    static async private void ProcessNextRequest()
    {
        if (ProcessQueue.Count == 0)
        {
            semaphore.Release();
            return;
        }
        try
        {
            ProcessQueue.TryDequeue(out (SaveJob, saveAction) result);
            SaveJob job = result.Item1;
            saveAction action = result.Item2;

            if (ProcessingJobs.Contains(job))
            {
                ProcessingJobs.Add(job);
                switch (action)
                {
                    case saveAction.Create:
                        job.CreateSave();
                        break;
                    case saveAction.Restore:
                        job.RestoreSave();
                        break;
                    case saveAction.Save:
                        job.Save();
                        break;
                    case saveAction.Delete:
                        job.DeleteSave();
                        break;
                    case saveAction.Decrypte:
                        job.DeleteSave();
                        break;
                    default:
                        return;
                        break;
                }
            }
            return;
        }
        catch (Exception ex)
        {
            LoggerLib.Logger.GetInstance().Log(ex);
        }
        finally
        {
            semaphore.Release();
        }
    }
}