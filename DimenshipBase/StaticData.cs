using System.Collections.Generic;
using DimenshipBase.FungibleItems;

namespace DimenshipBase;

public class StaticDataSubSystem : ISystemSubState
{
    public string Id => "StaticData";
    public string Name => "Static data subsystem";
    private Dictionary<string, ItemClassBase> FungibleItemClasses = new Dictionary<string, ItemClassBase>();
    private Dictionary<string, FacilityBaseClass> FacilityClasses = new Dictionary<string, FacilityBaseClass>();
    private Dictionary<string, ComponentRecipe> Recipes = new Dictionary<string, ComponentRecipe>();

    public FacilityBaseClass GetFacilityClass(string classId)
    {
        return FacilityClasses[classId];
    }
    public ItemClassBase GetItemClass(string classId)
    {
        return FungibleItemClasses[classId];
    }

    public void AddFacilityClass(FacilityBaseClass fc)
    {
        FacilityClasses[fc.Id] = fc;
    }
}

public class StaticData
{
    private Dictionary<string, ItemClassBase> FungibleItemClasses;
    private Dictionary<string, FacilityBaseClass> FacilityClasses;
    private Dictionary<string, ComponentRecipe> Recipes;
}