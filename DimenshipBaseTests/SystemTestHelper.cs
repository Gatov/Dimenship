using System;
using System.Collections.Generic;
using System.IO;
using DimenshipBase;
using DimenshipBase.FungibleItems;
using DimenshipBase.SubSystems;
using NUnit.Framework;

namespace DimenshipBaseTests;

public static class SystemTestHelper
{
    public static ISystemStateSet GenerateSystem()
    {
        DimenshipSystem system = new DimenshipSystem();
        FacilitySubSystem facilities = new FacilitySubSystem();
        StaticDataSubSystem staticData = new StaticDataSubSystem();
        ProcessSubSystem process = new ProcessSubSystem();
        ItemStorageSubSystem store = new ItemStorageSubSystem();
        string file = "StaticDataTests.json";
        var resFileName = Path.Join(TestContext.CurrentContext.TestDirectory, "Data", file);
        var buf = File.ReadAllBytes(resFileName);
        //StaticDataSubSystem sd = new StaticDataSubSystem();
        staticData.DeserializeJSON(buf);
        
        process.Initialize(system);
        facilities.Initialize(system);
      
        var fbFactory = staticData.GetFacilityClass(Category.factory.Path("basic"));
        var fCount = 0;
        facilities.AddFacility(new Facility(++fCount) { Name = "Starter Factory", ClassId = fbFactory.Id });
        facilities.AddFacility(new Facility(++fCount) { Name = "Second Factory", ClassId = fbFactory.Id });

        store.Store(Category.resource.Path("ore"), 1000);
        store.Store(Category.resource.Path("ice"), 1000);
        store.Store(Category.resource.Path("organic"), 1000);

        var notificationSubSystem = new NotificationSubSystem();
        notificationSubSystem.InfoAction += Console.WriteLine;
        notificationSubSystem.PushNotificationAction += s=>Console.WriteLine("! " +s);
        system.AddSubsystem(notificationSubSystem);
        system.AddSubsystem(facilities);
        system.AddSubsystem(staticData);
        system.AddSubsystem(process);
        system.AddSubsystem(store);
        return system;
    }
}