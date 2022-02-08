using System;
using System.Collections.Generic;
using System.ComponentModel;
using DimenshipBase;
using DimenshipBase.FungibleItems;
using DimenshipBase.Production;
using NUnit.Framework;

namespace DimenshipBaseTests;

[TestFixture]
public class ProcessTests
{
    public ISystemStateSet GenerateSystem()
    {
        DimenshipSystem system = new DimenshipSystem();
        FacilitySubSystem facilities = new FacilitySubSystem();
        StaticDataSubSystem staticData = new StaticDataSubSystem();
        ProcessSubSystem process = new ProcessSubSystem();
        ItemStorageSubSystem store = new ItemStorageSubSystem(); 
        
        process.Initialize(system);
        facilities.Initialize(system);
        FacilityBaseClass fbFactory = new FacilityBaseClass()
        {
            Name = "Basic Factory",
            Description = "Basic Factory for simple components production",
            Id = Category.factory.Path("basic"),
            Tags = "factory production facility",
            GlyphName = "facilities/factory/basic",
            Functions = "Workshop Assembly Purifier",
            IdlePowerConsumption = 0.1
        };
        staticData.AddFacilityClass(fbFactory);
        var fCount = 0;
        facilities.AddFacility(new Facility(++fCount) { Name = "Starter Factory", ClassId = fbFactory.Id });
        facilities.AddFacility(new Facility(++fCount) { Name = "Second Factory", ClassId = fbFactory.Id });

        var recipe = new ComponentRecipe()
        {
            Name = "Wheel Chassis MK1",
            Description = "Description of Wheel Chassis MK1",
            Tags = "chassis,wheel,component,bot",
            Id = Category.recipe.Path("components", "chassis", "wheel", "mk1"),
            GlyphName = Category.recipe.Path("components", "chassis", "wheel", "mk1"),
            RequiredFacility = Category.factory.Path("basic"),
            BaselineBuildTime = 120,
            BaselineIngredientList = new List<Ingredient>()
            {
                new(){Required = 10, ResourceId = Category.recource.Path("metal"),},
                new(){Required = 20, ResourceId = Category.recource.Path("composite"),}
            }
        };
        
        store.Store(Category.recource.Path("metal"), 1000);
        store.Store(Category.recource.Path("composite"), 1000);
        staticData.AddRecipe(recipe);
        staticData.AddItemClass(new ItemClassBase()
        {
            Id = Category.recource.Path("metal"),
            Description = "Metal description",
            Name = "Metal name",
            Tags = "basic,resource,metal",
            Volume = 1,
            Weight = 7.86,
            GlyphName = Category.recource.Path("metal"),
        });
        
        system.AddSubsystem(facilities);
        system.AddSubsystem(staticData);
        system.AddSubsystem(process);
        system.AddSubsystem(store);
        return system;
    }

    [Test]
    public void TestProcessEstimateGeneration()
    {
        var system = GenerateSystem();
        var recipe = system.GetSubState<StaticDataSubSystem>().GetRecipe(Category.recipe.Path("components", "chassis", "wheel", "mk1"));
        var processFactory = new ProductionPlannerSingleFacility();
        var estimate = processFactory.GetEstimate(system, recipe, null);
        //var ing = new Ingredient() { Required = 1, ResourceId = @"test" };
        Console.WriteLine(Utils.AsJSON(estimate));
    }
    [Test]
    public void TestProcessGeneration()
    {
        var system = GenerateSystem();
        var recipe = system.GetSubState<StaticDataSubSystem>().GetRecipe(Category.recipe.Path("components", "chassis", "wheel", "mk1"));
        var processFactory = new ProductionPlannerSingleFacility();
        var estimate = processFactory.CreateProcess(system, recipe, null);
        //var ing = new Ingredient() { Required = 1, ResourceId = @"test" };
        Console.WriteLine(Utils.AsJSON(estimate));
    }
}