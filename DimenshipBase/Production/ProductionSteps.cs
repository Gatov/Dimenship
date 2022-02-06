using System.Collections.Generic;
using DimenshipBase.FungibleItems;

namespace DimenshipBase.Production;

public class LogisticsStep : StepBase
{
    public List<Ingredient> Ingridients;
    public override string LogLine => "Moved resources to the production location";
    public override string DetailedDescription { get; } = "";

    public override void OnProcessStart(ISystemStateSet system)
    {
        base.OnProcessStart(system);
        var storage = system.GetSubState<ItemStorageSubSystem>();
        foreach (var ingridient in Ingridients)
        {
            //if(storage.Check(ingridient.ResourceId) < )
            storage.Book(ingridient.ResourceId, ingridient.Required);
        }
    }
}

public class ProductionStep : StepBase
{
    public string TargetName { get; set; }
    public string TargetClassId { get; set; }
    public int TargetCount { get; set; }
    public override string LogLine => "Producing" + TargetName;
    public override string DetailedDescription { get; }
    
    public override void OnProcessEnd(ISystemStateSet system)
    {
        base.OnProcessEnd(system);
        var storage = system.GetSubState<ItemStorageSubSystem>();
        //var staticData = system.GetSubState<StaticDataSubSystem>();
        storage.Store(TargetClassId, TargetCount);
        
    }
}