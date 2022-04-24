using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using DimenshipBase.SubSystems;

namespace DimenshipBase
{
    /// <summary>
    /// Base class for Dimenship facilities
    /// </summary>
    public class FacilityBaseClass : ClassBase
    {
        [XmlAttribute]
        public string Functions 
        {
            get => string.Join(' ', _functions);
            set => _functions = (value ?? "").Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet();
        }
        private HashSet<string> _functions=new();
        // future expansion
        [XmlAttribute] public double IdlePowerConsumption { get; set; }

        public bool HasFunction(string function)
        {
            return _functions.Contains(function);
        }
    }

    [DataContract(Name = "Facility")]
    public class Facility
    {
        public Facility(int uniqueId)
        {
            UniqueId = uniqueId;
        }

        [DataMember(Name = "id")]
        [XmlAttribute] public int UniqueId { get; set; }
        
        [DataMember(Name = "name")]
        [XmlAttribute] public string Name { get; set; }

        public FacilityBaseClass GetClass(ISystemStateSet sys)
        {
            return _class ??= sys.GetSubState<StaticDataSubSystem>().GetFacilityClass(ClassId);}

        private FacilityBaseClass _class;

        [DataMember(Name = "class")]
        [XmlElement]
        public string ClassId { get; set; }
        //[XmlIgnore]
        //public double ResourceEfficiency => 

        //public double ConsumptionEfficiency;
        //public double EnergyEfficiency;
        //public double SpeedEfficiency;
    }


    
    [DataContract(Name="flock")]
    public class FacilityLock
    {
        public ProcessBase GetOwner(ISystemStateSet system)
        {
            var processes = system.GetSubState<ProcessSubSystem>();
            return processes.Get(ProcessId);
        }

        public int ProcessId;
        public int FacilityId;
    }
}