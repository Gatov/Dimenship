using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DimenshipBase
{
    /// <summary>
    /// Base class for Dimenship facilities
    /// </summary>
    public class FacilityBaseClass : ItemClassBase
    {
        // future expansion
        public double IdlePowerConsumption { get; set; }
    }

    public class Facility
    {
        public FacilityBaseClass Class;
        //public double ConsumptionEfficiency;
        //public double EnergyEfficiency;
        //public double SpeedEfficiency;
        public Queue<ProcessBase> OngoingProcesses  
    }


    public class FacilitySubSystem : ISystemSubState
    {
        public string Id => "Facilities";
        public string Name => "Facilities";
        private List<Facility> Facilities;

        public IEnumerable<Facility> GetAvailableFacility(string id)
        {
            return Facilities.Where(x=>x.Class.Id == id && x.)
        }
    }

}


