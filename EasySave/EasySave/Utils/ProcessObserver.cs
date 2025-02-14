using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;

namespace EasySave.Utils;

public class ProcessObserver : IDisposable
{
    private string[] _processNames;
    private readonly Timer _checkTimer;
    private bool _lastState = false;

    public event Action<bool> OnProcessStateChanged;

    public ProcessObserver(int checkIntervalMs)
    {
        _processNames = SettingsJson.GetInstance()
            .GetContent()
            .businessSoftwares.Split(" ")
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToArray();
        _checkTimer = new Timer(checkIntervalMs);
        _checkTimer.Elapsed += CheckProcess;
        _checkTimer.AutoReset = true;
        _checkTimer.Start();
    }

    private void CheckProcess(object sender, ElapsedEventArgs e)
    {
        // Reload the business process
        _processNames = SettingsJson.GetInstance().GetContent().businessSoftwares.Split(" ").Where(name => !string.IsNullOrWhiteSpace(name)).ToArray();
        var isRunning = CheckIfProcessRunning();

        if (isRunning != _lastState)
        {
            OnProcessStateChanged?.Invoke(isRunning);
            _lastState = isRunning;
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

    public void Dispose()
    {
        _checkTimer.Stop();
        _checkTimer.Dispose();
    }
}