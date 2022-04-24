using System.Collections.Generic;
using System.Runtime.Serialization;
using DimenshipBase.FungibleItems;
using DimenshipBase.SubSystems;

namespace DimenshipBase.Production;

[DataContract(Name="LogisticsStep",Namespace = "P")]
public class LogisticsStep : StepBase
{
    [DataMember]
    public List<Ingredient> Ingridients;
    public override string LogLine => "Moved resources to the production location";
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

[DataContract(Name="ProductionStep",Namespace = "P")]
public class ProductionStep : StepBase
{
    [DataMember]
    public string TargetName { get; set; }
    [DataMember]
    public string TargetClassId { get; set; }
    [DataMember]
    public int TargetCount { get; set; }
    public override string LogLine => "Producing " + TargetName;
    
    public override void OnProcessEnd(ISystemStateSet system)
    {
        base.OnProcessEnd(system);
        var storage = system.GetSubState<ItemStorageSubSystem>();
        //var staticData = system.GetSubState<StaticDataSubSystem>();
        storage.Store(TargetClassId, TargetCount);
        
    }
}