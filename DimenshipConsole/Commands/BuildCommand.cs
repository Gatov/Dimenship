using System;
using DimenshipBase;
using DimenshipBase.FungibleItems;
using DimenshipBase.Production;
using DimenshipBase.SubSystems;

namespace DimenshipConsole.Commands;

public class BuildCommand : ICommand
{
    public string Command => "build";
    public string HelpLine => $"build recipe_id";
    public bool Execute(ISystemStateSet system, string[] args)
    {
        ComponentRecipe? recipe;
        if (args.Length != 1) return false;
        var id = args[0];
        try
        {
            recipe = system.GetSubState<StaticDataSubSystem>().GetRecipe(id);
        }
        catch (Exception e)
        {
            recipe = null;
        }

        if (recipe == null)
        {
            Console.WriteLine($"Cannot find recipe with id {id}");
            return false;
        }
        
        var processFactory = new ProductionPlannerSingleFacility();
        var process = processFactory.CreateProductionProcess(system, recipe, null);
        var ps = system.GetSubState<ProcessSubSystem>();
        ps.Add(process).StartProcess(system);
        return true;
    }
}