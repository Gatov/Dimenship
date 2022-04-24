using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DimenshipBase.Production;
using DimenshipBase.SubSystems;

namespace DimenshipBase
{
    /// <summary>
    /// !do we allow queuing processes - Not initially, this requires cancellation mechanics and recalculation
    /// of queued processes, return of resources. Later on we may extend the game with queuing
    /// !how do we resolve resource outage at the step - We will not allow queuing process without available resources 
    /// </summary>
    public class ProcessBase
    {
        public int UniqueId { get; set; }
        public bool Complete { get; private set; }
        public List<StepBase> Steps;

        public GameTime FinishBy
        {
            get
            {
                var last = Steps.Last();
                return last.StartTime + last.DurationTicks;
            }
        }

        public void PassTime(ISystemStateSet system, GameTime newTime)
        {
            if (Complete)
                return;
            foreach (var step in Steps)
            {
                if (step.Complete) continue;
                if(step.FinishTime <newTime)
                    step.OnStepEnd(system);
                else
                    return; // the step is not complete and we assume sequential performance
            }

            Complete = true;
            system.Notify($"Assembly of an item is complete");
        }

        
    }

    /// <summary>
    /// STep of the process
    /// </summary>
    [DataContract]
    [KnownTypeAttribute(typeof(LogisticsStep))]
    [KnownTypeAttribute(typeof(ProductionStep))]
    public abstract class StepBase
    {
        [DataMember(Order = 4)]
        public bool Complete { get; set; }
        
        [DataMember(Order = 3)]
        public int DurationTicks { get; set; }
        [DataMember(Order = 2)]
        public GameTime StartTime { get; set; }
        public GameTime FinishTime => StartTime + DurationTicks;
        public abstract string LogLine { get; }
        [DataMember(Order = 5)]
        public string DetailedDescription { get; set; }

        public virtual void OnProcessStart(ISystemStateSet system) { }
        //public virtual void OnStepStart(ISystemStateSet system) { }
        public virtual void OnStepEnd(ISystemStateSet system)
        {
            Complete = true;
            system.Log(LogLine);

        }
        public virtual void OnProcessEnd(ISystemStateSet system) { }


    }

    public static class LogExtension
    {
        public static void Log(this ISystemStateSet system, string line)
        {
            var log = system.GetSubState<NotificationSubSystem>();
            log.LogLine(line);
        }
        public static void Notify(this ISystemStateSet system, string line)
        {
            var log = system.GetSubState<NotificationSubSystem>();
            log.Notify(line);
        }
    }
}