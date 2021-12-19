using System.Collections.Generic;
using System.Linq;

namespace DimenshipBase;

public class FacilitySubSystem : ISystemSubState
{
    public string Id => "Facilities";
    public string Name => "Facilities";
    private readonly List<Facility> Facilities = new();
    private List<ResourceLock> locks = new();
    private ISystemStateSet system;

    public FacilitySubSystem(ISystemStateSet system)
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
                locks.LastOrDefault(l => l.UniqueId == f.UniqueId) // Assume for now that last lock is most recent
            where lastLock == null || lastLock.Owner.FinishBy <= startTime // not locked at the moment
            select f;
    }

    public void Book(int facilityId, ProcessBase production)
    {
        locks.Add(new ResourceLock() { UniqueId = facilityId, Owner = production });
    }
}