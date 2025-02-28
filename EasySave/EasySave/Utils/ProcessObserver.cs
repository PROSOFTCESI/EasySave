using EasySave.CustomExceptions;
using EasySave.Utils.JobStates;
using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Timer = System.Timers.Timer;

namespace EasySave.Utils;

public class ProcessObserver : IDisposable
{
    private static readonly Lazy<ProcessObserver> _instance = new Lazy<ProcessObserver>(() => new ProcessObserver());
    private string[] _processNames;
    private readonly Timer _checkTimer;
    private bool _lastState = false;

    public event Action<bool> OnProcessStateChanged;

    // Rendre le constructeur privé
    private ProcessObserver()
    {
        _processNames = SettingsJson.GetInstance()
            .GetContent()
            .businessSoftwares.Split(" ")
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToArray();
        _checkTimer = new Timer(1000); // Interval par défaut, peut être modifié via une méthode
        _checkTimer.Elapsed += CheckProcess;
        _checkTimer.AutoReset = true;
        _checkTimer.Start();
    }

    // Méthode publique pour accéder à l'instance unique et changer l'intervalle de check
    public static ProcessObserver GetInstance(int checkIntervalMs = 1000)
    {
        _instance.Value.SetCheckInterval(checkIntervalMs);
        return _instance.Value;
    }

    private void SetCheckInterval(int checkIntervalMs)
    {
        _checkTimer.Interval = checkIntervalMs;
    }

    private void CheckProcess(object sender, ElapsedEventArgs e)
    {
        // Recharger les processus métier
        _processNames = SettingsJson.GetInstance().GetContent().businessSoftwares.Split(" ").Where(name => !string.IsNullOrWhiteSpace(name)).ToArray();
        var isRunning = CheckIfProcessRunning();

        if (isRunning != _lastState)
        {
            OnProcessStateChanged?.Invoke(isRunning);
            _lastState = isRunning;
            PlayPauseJobs(isRunning);
        }
        /*
         * faire en sorte que si ya plus de process, on relance les jobs en pause
         * */
    }

    private void PlayPauseJobs(bool isRunning)
    {
        var saveJobs = SaveJob.Instances;
        if (isRunning)
        {
            foreach (SaveJob job in saveJobs)
            {
                Pause(job);
            }
        }
        else
        {
            foreach (SaveJob job in saveJobs)
            {
                Play(job);
            }
        }
    }

    private bool CheckIfProcessRunning()
    {
        foreach (var processName in _processNames)
        {
            if (Process.GetProcessesByName(processName).Length > 0)
                return true;
        }
        return false;
    }

    public static async void Pause(SaveJob saveJob)
    {
        try
        {
            if (saveJob.State.Equals(StateJsonReader.SavingState) || saveJob.State.Equals(StateJsonReader.EncryptingState))
            {
                await Task.Run(() => saveJob.Pause());
            }
        }
        catch (Exception ex)
        {
            if (ex is BusinessSoftwareRunningException || ex is PlayPauseStopException)
            {
                return;
            }
            throw;
        }
    }

    public static async void Play(SaveJob saveJob)
    {
        try
        {
            if (saveJob.State.Equals(StateJsonReader.PausedState))
            {
                await Task.Run(() => saveJob.Play());
            }
        }
        catch (Exception ex)
        {
            if (ex is BusinessSoftwareRunningException || ex is PlayPauseStopException)
            {
                return;
            }
            throw;
        }
    }

    public void Dispose()
    {
        _checkTimer.Stop();
        _checkTimer.Dispose();
    }
}