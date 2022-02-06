using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DimenshipBase;

[DataContract(Name = "Processes")]
public class ProcessSubSystem : ISystemSubState
{
    public string Id => "Processes";
    public string Name => "Processes";
    private readonly object _syncRoot = new object();
    private Dictionary<int, ProcessBase> _dic = new Dictionary<int, ProcessBase>();
    private ISystemStateSet _system;
    [DataMember(Name = "list")]
    private List<ProcessBase> List 
    {
        get { lock (_syncRoot)  return _dic.Values.ToList(); }
        set { lock (_syncRoot)  _dic = value.ToDictionary(x=>x.UniqueId); }
    }

    public void Initialize(ISystemStateSet system)
    {
        _system = system;
    }

    public ProcessBase Get(int id)
    {
        lock (_syncRoot)  return _dic[id];
    }
    
    /// <summary>
    /// Problem statement - we will use real time, it will make system susceptible to incorrect time set
    /// We need to introduce anti-cheat check, if time has moved backwards insignificantly, it's ok.
    /// </summary>
    private void RealTimePass(GameTime newTime)
    {
        //subStatesValue.RealTimePass(newTime);
        lock (_syncRoot)
        {
            foreach (var proc in _dic.Values)
            {
                proc.PassTime(_system, newTime);
            }
        }
    }
}