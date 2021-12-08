using System.Collections.Generic;
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
        private List<StepBase> list;
    }

    public class ProductionProcessFactory
    {
        public ProcessBase Produce(ComponentRecipe recipe, ISystemStateSet system)
        {
            // book Facility
            var facilities = system.GetSubState<FacilitySubSystem>();
            foreach (var facility in facilities.GetAvailableFacility())
            {
                
            }
            
            // book//tap resources
            var storage = system.GetSubState<FungibleItemStorage>();
            foreach (var part in recipe.BaselineIngredientList)
            {
                storage
            }
        }
    }
    
    

    public abstract class StepBase
    {
        public bool Complete { get; private set; }
        public int DurationTicks { get; private set; }
        public GameTime StartTime { get; private set; }
        public abstract string LogLine { get; }
        public abstract string DetailedDescription { get; }

        public abstract void Apply(ISystemStateSet system);
    }

    public class ResourceBase
    {
        public ResourceLock Lock;

    }

    public class ResourceLock
    {
        public ProcessBase Owner;
        public ResourceBase Resource;
            //public 
    }
}