using System.Collections.Generic;
using DimenshipBase.SubSystems;

namespace DimenshipBase;

/// <summary>
/// Representation of the game
/// </summary>
public class DimenshipSystem : ISystemStateSet
{
    private readonly Dictionary<string, ISystemSubState> _subStates = new Dictionary<string, ISystemSubState>();

    public T GetSubState<T>() where T : ISystemSubState
    {
        return (T)_subStates[typeof(T).Name];
    }

    public GameTime CurrentTime { get; private set; }

    /// <summary>
    /// Problem statement - we will use real time, it will make system susceptible to incorrect time set
    /// We need to introduce anti-cheat check, if time has moved backwards insignificantly, it's ok.
    /// </summary>

    public void UpdateTime(GameTime now)
    {
        CurrentTime = now;
        var ps = GetSubState<ProcessSubSystem>();
        ps.RealTimePass(now);
    }
    public void AddSubsystem(ISystemSubState subSystem)
    {
        _subStates[subSystem.GetType().Name] = subSystem;
    }
}