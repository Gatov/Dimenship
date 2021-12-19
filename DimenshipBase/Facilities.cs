using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

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

    public class Facility
    {
        public Facility(int uniqueId)
        {
            UniqueId = uniqueId;
        }

        [XmlAttribute] public int UniqueId { get; set; }
        [XmlAttribute] public string Name { get; set; }

        public FacilityBaseClass GetClass(ISystemStateSet sys)
        {
            return _class ??= sys.GetSubState<StaticDataSubSystem>().GetFacilityClass(ClassId);}

        private FacilityBaseClass _class;

        [XmlElement]
        public string ClassId { get; set; }
        //[XmlIgnore]
        //public double ResourceEfficiency => 

        //public double ConsumptionEfficiency;
        //public double EnergyEfficiency;
        //public double SpeedEfficiency;
    }


    public class ResourceLock
    {
        public ProcessBase Owner;
        public int UniqueId;
    }
}