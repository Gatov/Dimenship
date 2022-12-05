using System;
using DimenshipBase;
using DimenshipBase.Production;
using NUnit.Framework;

namespace DimenshipBaseTests;

[TestFixture]
public class RunningTests
{
    [Test]
    public void ProcessExecutionTest()
    {
        var system = (DimenshipSystem)SystemTestHelper.GenerateSystem();
        DateTime baseTime = new DateTime(2022, 03, 21);
        system.UpdateTime(new GameTime(baseTime));
        var recipe = system.GetSubState<StaticDataSubSystem>().GetRecipe(Category.recipe.Path("component", "chassis", "wheel", "mk1"));
        var processFactory = new ProductionPlannerSingleFacility();
        var process = processFactory.CreateProductionProcess(system, recipe, null);
        process.StartProcess(system);
        
        Assert.AreEqual(2,process.Steps.Count);
        Assert.AreEqual(13,process.Steps[0].DurationTicks);
        Assert.AreEqual(120,process.Steps[1].DurationTicks);
        process.PassTime(system, system.CurrentTime);
        
        system.UpdateTime(new GameTime(baseTime.AddSeconds(14)));
        process.PassTime(system, system.CurrentTime);
        
        Assert.IsTrue(process.Steps[0].Complete);
        Assert.IsFalse(process.Steps[1].Complete);
        
        system.UpdateTime(new GameTime(baseTime.AddSeconds(132)));
        process.PassTime(system, system.CurrentTime);
        
        Assert.IsTrue(process.Steps[0].Complete);
        Assert.IsFalse(process.Steps[1].Complete);
        
        system.UpdateTime(new GameTime(baseTime.AddSeconds(134)));
        process.PassTime(system, system.CurrentTime);
        Assert.IsTrue(process.Steps[1].Complete);

    }
}