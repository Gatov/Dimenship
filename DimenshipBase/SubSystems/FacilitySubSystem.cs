using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DimenshipBase.SubSystems;

/// <summary>
///  keeps the list of built facilities and tracks availability
/// </summary>
[DataContract(Name = "FacilitySystem")]
public class FacilitySubSystem : ISystemSubState
{
    public string Id => "Facilities";
    public string Name => "Facilities";
    [DataMember(Name="Facilities")]
    private readonly List<Facility> Facilities = new();
    [DataMember(Name="Locks")]
    private List<FacilityLock> locks = new();
    private ISystemStateSet system;

    // for serialization
    public FacilitySubSystem()
    {
    }

    public void Initialize(ISystemStateSet system)
    {
        this.system = system;
        system.AddSubsystem(this);
    }

    public void AddFacility(Facility facility)
    {
        Facilities.Add(facility);
    }

    public IEnumerable<Facility> GetList() => Facilities;

    public IEnumerable<Facility> GetAvailableFacility(string function, GameTime startTime)
    {
        return from f in Facilities
            where f.GetClass(system).HasFunction(function)
            let lastLock =
                locks.LastOrDefault(l => l.FacilityId == f.UniqueId) // Assume for now that last lock is most recent
            where lastLock == null || lastLock.GetOwner(system).FinishBy <= startTime // not locked at the moment
            select f;
    }

    public void Book(int facilityId, ProcessBase production)
    {
        locks.Add(new FacilityLock() { FacilityId = facilityId, ProcessId = production.UniqueId });
    }
}