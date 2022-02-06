using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using DimenshipBase.FungibleItems;

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
            foreach (var step in Steps)
            {
                if (step.Complete) continue;
                if(step.FinishTime <newTime)
                    step.OnStepEnd(system);
                else
                    break; // the step is not complete and we assume sequential performance
            }
        }
    }

    public class ProductionProcessFactory
    {
        public ProcessBase Produce(ComponentRecipe recipe, ISystemStateSet system)
        {
            throw new NotImplementedException();
            // book Facility
            var facilities = system.GetSubState<FacilitySubSystem>();
            //foreach (var facility in facilities.GetAvailableFacility())
            {
            }

            // book//tap resources
            var storage = system.GetSubState<ItemStorageSubSystem>();
            foreach (var part in recipe.BaselineIngredientList)
            {
                //storage
            }

            return null;
        }
    }


    public abstract class StepBase
    {
        public bool Complete { get; set; }
        public int DurationTicks { get; set; }
        public GameTime StartTime { get; set; }
        public GameTime FinishTime => StartTime + DurationTicks;
        public abstract string LogLine { get; }
        public abstract string DetailedDescription { get; }

        public virtual void OnProcessStart(ISystemStateSet system) { }
        //public virtual void OnStepStart(ISystemStateSet system) { }
        public virtual void OnStepEnd(ISystemStateSet system)
        {
            Complete = true;
            Log(system, LogLine);

        }
        public virtual void OnProcessEnd(ISystemStateSet system) { }

        private void Log(ISystemStateSet system, string line)
        {
            var log = system.GetSubState<NotificationSubSystem>();
            log.LogLine(line);
        }
    }
}