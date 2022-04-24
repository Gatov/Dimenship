using System.Collections.Generic;
using System.Linq;
using DimenshipBase.FungibleItems;

namespace DimenshipBase.SubSystems;

public class ResearchSubSystem : ISystemSubState
{
    public string Id => "Research";
    public string Name => "Research System";
    private readonly List<string> _researchedRecipes = new();
    private ISystemStateSet _system;
    private readonly object _syncRoot = new object();

    public void Initialize(ISystemStateSet system)
    {
        _system = system;
    }
    
    public IEnumerable<ComponentRecipe> GetAllKnownRecipes()
    {
        var sd = _system.GetSubState<StaticDataSubSystem>();
        lock (_syncRoot)
        {
            return _researchedRecipes.Select(id => sd.GetRecipe(id)).ToList();
        }
    }

    public void AddRecipe(string id)
    {
        lock (_syncRoot)
        {
            _researchedRecipes.Add(id);    
        }
        
    }
}