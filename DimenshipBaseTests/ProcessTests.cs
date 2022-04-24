using System;
using System.ComponentModel;
using System.IO;
using DimenshipBase;
using DimenshipBase.Production;
using DimenshipBase.SubSystems;
using NUnit.Framework;

namespace DimenshipBaseTests;

[TestFixture]
public class ProcessTests
{

    [Test]
    public void TestProcessEstimateGeneration()
    {
        var system = SystemTestHelper.GenerateSystem();
        var recipe = system.GetSubState<StaticDataSubSystem>().GetRecipe(Category.recipe.Path("component", "chassis", "wheel", "mk1"));
        var processFactory = new ProductionPlannerSingleFacility();
        var estimate = processFactory.GetEstimate(system, recipe, null);
        //var ing = new Ingredient() { Required = 1, ResourceId = @"test" };
        Console.WriteLine(Utils.AsJSON(estimate));
    }
    [Test]
    public void TestProcessGeneration()
    {
        var system = LoadSystem();
        var recipe = system.GetSubState<StaticDataSubSystem>().GetRecipe(Category.recipe.Path("component", "chassis", "wheel", "mk1"));
        var processFactory = new ProductionPlannerSingleFacility();
        var process = processFactory.CreateProcess(system, recipe, null);
        //var ing = new Ingredient() { Required = 1, ResourceId = @"test" };
        Console.WriteLine(Utils.AsJSON(process));
    }
    private ISystemStateSet LoadSystem()
    {
        var file = "StaticDataTests.json";
        var resFileName = Path.Join(TestContext.CurrentContext.TestDirectory, "Data", file);
        var buf = File.ReadAllBytes(resFileName);
        StaticDataSubSystem sd = new StaticDataSubSystem();
        sd.DeserializeJSON(buf);
        
        
        
        DimenshipSystem system = new DimenshipSystem();
        FacilitySubSystem facilities = new FacilitySubSystem();
        ProcessSubSystem process = new ProcessSubSystem();
        ItemStorageSubSystem store = new ItemStorageSubSystem(); 
        
        process.Initialize(system);
        facilities.Initialize(system);

        FacilityBaseClass fbFactory = sd.GetFacilityClass(Category.factory.Path("basic")); 

        var fCount = 0;
        facilities.AddFacility(new Facility(++fCount) { Name = "Starter Factory", ClassId = fbFactory.Id });
        facilities.AddFacility(new Facility(++fCount) { Name = "Second Factory", ClassId = fbFactory.Id });

        store.Store(Category.resource.Path("metal"), 1000);
        store.Store(Category.resource.Path("ice"), 1000);
        store.Store(Category.resource.Path("organic"), 1000);
        
        system.AddSubsystem(facilities);
        system.AddSubsystem(sd);
        system.AddSubsystem(process);
        system.AddSubsystem(store);
        return system;
    }
}