using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using DimenshipBase;
using DimenshipBase.Production;
using NUnit.Framework;

namespace DimenshipBaseTests;

[TestFixture]
public class FacilitiesTests
{
    public ISystemStateSet GenerateSystem()
    {
        DimenshipSystem system = new DimenshipSystem();
        FacilitySubSystem fs = new FacilitySubSystem();
        StaticDataSubSystem sd = new StaticDataSubSystem();
        
        fs.Initialize(system);
        FacilityBaseClass fbFactory = new FacilityBaseClass()
        {
            Name = "Basic Factory",
            Description = "Basic Factory for simple components production",
            Id = "factory/basic",
            Tags = "factory production facility",
            GlyphName = "facilities/factory/basic",
            Functions = "Workshop Assembly Purifier",
            IdlePowerConsumption = 0.1
        };
        FacilityBaseClass fbQuarters = new FacilityBaseClass()
        {
            Name = "Captain Quarters",
            Description = "Captain Quarters and life support",
            Id = "support/captain quarters",
            Tags = "life support facility",
            GlyphName = "facilities/support/captain quarters",
            IdlePowerConsumption = 0.05
        };
        sd.AddFacilityClass(fbFactory);
        sd.AddFacilityClass(fbQuarters);
        var fCount = 0;
        fs.AddFacility(new Facility(++fCount) { Name = "Starter Factory", ClassId = fbFactory.Id });
        fs.AddFacility(new Facility(++fCount) { Name = "Second Factory", ClassId = fbFactory.Id });
        fs.AddFacility(
            new Facility(++fCount) { Name = "Captain Quarters", ClassId = fbQuarters.Id });
        
        system.AddSubsystem(fs);
        system.AddSubsystem(sd);
        return system;
    }

    [Test]
    public void TestLockAndAvailability()
    {
        GameTime systemTime = new GameTime(new DateTime(2021, 12, 10, 19, 00, 00));

        var system = GenerateSystem();
        var fs = system.GetSubState<FacilitySubSystem>();
        var freeFactories = fs.GetAvailableFacility("Workshop", systemTime).ToList();
        Assert.AreEqual(2, freeFactories.Count); // 2 factories
        ProcessBase production = new ProcessBase()
        {
            UniqueId = 1,
            Steps = new List<StepBase>() { new LogisticsStep() { StartTime = systemTime, DurationTicks = 120 } }
        };

        fs.Book(freeFactories.First().UniqueId, production);
        systemTime = systemTime.AddTicks(60); // 1 minute passed
        var stillFreeFactories = fs.GetAvailableFacility("Workshop", systemTime).ToList();
        Assert.AreEqual(1, stillFreeFactories.Count); // 1 factory
        systemTime = systemTime.AddTicks(60); // 1 minute passed
        var againFreeFactories = fs.GetAvailableFacility("Workshop", systemTime).ToList();
        Assert.AreEqual(2, againFreeFactories.Count); // 2 factories
    }

    [Test]
    public void TestFacilitySave()
    {
        var sys = GenerateSystem();
        var fs = sys.GetSubState<FacilitySubSystem>();
        Console.WriteLine(Utils.AsJSON(fs));
    }
}