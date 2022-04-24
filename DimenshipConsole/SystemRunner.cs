using System;
using System.Diagnostics;
using System.Threading;
using DimenshipBase;

namespace DimenshipConsole;

public class SystemRunner
{
    private Thread _runnerThread;
    private CancellationTokenSource _cancelSrc;
    private DimenshipSystem _system;
    private long _spinCycleMs =1000;

    private object _syncRoot = new object();

    public SystemRunner(DimenshipSystem system)
    {
        _system = system;
        _cancelSrc = new CancellationTokenSource();
    }
    public void Start()
    {
        _runnerThread = new Thread(SpinRoutine);
        _runnerThread.Start();

    }

    private void SpinRoutine()
    {
        Stopwatch sw = new Stopwatch();
        while (!_cancelSrc.IsCancellationRequested)
        {
            sw.Restart();

            lock(_syncRoot) // avoid potential command/processing collision
                _system.UpdateTime(new GameTime(DateTime.UtcNow));

            var elapsed = sw.ElapsedMilliseconds;
            if (elapsed < _spinCycleMs)
                _cancelSrc.Token.WaitHandle.WaitOne((int)(_spinCycleMs - elapsed));
        }
    }

    public void Stop()
    {
        _cancelSrc.Cancel();
        _runnerThread.Join((int)_spinCycleMs);
        lock (_syncRoot)
            _system.Log("Stopping the system");
    }

    public void SafeExecute(Action<DimenshipSystem> action)
    {
        lock(_syncRoot)
            action.Invoke(_system);
    }
}