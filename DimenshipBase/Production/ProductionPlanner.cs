using System;
using System.Collections.Generic;
using System.Linq;
using DimenshipBase.FungibleItems;

namespace DimenshipBase.Production;

public interface IProductionOptions
{
}

public class ProductionPlannerSingleFacility
{
    public ProductionEstimate GetEstimate(ISystemStateSet system, ComponentRecipe recipe, IProductionOptions options)
    {
        ProductionEstimate result = new ProductionEstimate();
        var fs = system.GetSubState<FacilitySubSystem>();
        var ss = system.GetSubState<ItemStorageSubSystem>();
        var facility = fs.GetAvailableFacility(recipe.RequiredFacility, system.CurrentTime).FirstOrDefault();
        // TOPO: use options to select best fit
        double resEfficiency = 1;
        if (facility==null)
        {
            result.Facilities = new List<FacilityEstimate>()
                { new FacilityEstimate() { Function = recipe.RequiredFacility } };
        }
        else
        {
            result.Facilities = new List<FacilityEstimate>()
            {
                new FacilityEstimate()
                {
                    Function = recipe.RequiredFacility,
                    AllocatedId = facility.UniqueId, AllocatedName = facility.Name
                }
            };
        }
        //resEfficiency = facility.First().ResourceEfficiency;
        result.Ingredients = recipe.BaselineIngredientList
            .Select(x => new IngredientEstimate()
            {
                Available = ss.Check(x.ResourceId),
                Required = x.Required,
                ResourceId = x.ResourceId
            }).ToList();
        result.TimeToFinish = recipe.BaselineBuildTime;
        return result;
    }
    
    public ProcessBase CreateProcess(ISystemStateSet system, ComponentRecipe recipe, IProductionOptions options)
    {
        ProcessBase p = new ProcessBase();
        var estimate = GetEstimate(system, recipe, options);
        // book resources
        var storage = system.GetSubState<ItemStorageSubSystem>();
        var staticData = system.GetSubState<StaticDataSubSystem>();
        foreach (var ingredient in estimate.Ingredients)
        {
            storage.Book(ingredient.ResourceId, ingredient.Required);
        }

        LogisticsStep resMove = new LogisticsStep()
        {
            Ingridients = estimate.Ingredients.Select(x => new Ingredient()
            {
                Required = x.Required,
                ResourceId = x.ResourceId
            }).ToList(),
            DurationTicks = estimate.Ingredients
                .Select(x => CalculateMoveTime(x.ResourceId, x.Required, system))
                .Sum()
            //staticData.GetItemClass(x.ResourceId).Weight*)
        };
        

        // book rooms
        var facilities = system.GetSubState<FacilitySubSystem>();
        
        foreach (var facility in estimate.Facilities)
        {
            facilities.Book(facility.AllocatedId,p);
        }
        

        return p;
    }

    private int CalculateMoveTime(string argResourceId, int argRequired, ISystemStateSet system)
    {
        var staticData = system.GetSubState<StaticDataSubSystem>();
        var resClass = staticData.GetItemClass(argResourceId);
        // TODO LogisticSubSystem
        return (int)Math.Ceiling(resClass.Weight * argRequired/10);
    }
}

public class ProductionEstimate
{
    public List<IngredientEstimate> Ingredients;
    public List<FacilityEstimate> Facilities;
    public bool Failed {
        get
        {
            return Facilities.Any(x => x.AllocatedId==0) ||
                   Ingredients.Any(x => !x.IsEnough);
        }
        
    }

    public int TimeToFinish { get; set; }
}

public class FacilityEstimate
{
    public string Function { get; set; }
    public int AllocatedId { get; set; }
    public string AllocatedName { get; set; }
}

public class IngredientEstimate : Ingredient
{
    public int Available { get; set; }
    public bool IsEnough => Available >= Required;
}