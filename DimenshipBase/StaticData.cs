using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using DimenshipBase.FungibleItems;

namespace DimenshipBase;

/// <summary>
/// Provides static access to static data - classes of items, facilities and recipes 
/// Static data is loaded at the app start and is not modified at runtime
/// This clas is thread-safe as all access is readonly
/// </summary>
public class StaticDataSubSystem : ISystemSubState
{
    public string Id => "StaticData";
    public string Name => "Static data subsystem";
    private Dictionary<string, ItemClassBase> _fungibleItemClasses = new();
    private Dictionary<string, FacilityBaseClass> _facilityClasses = new();
    private Dictionary<string, ComponentRecipe> _recipes = new();

    public FacilityBaseClass GetFacilityClass(string classId) => _facilityClasses[classId];
    public void AddFacilityClass(FacilityBaseClass fc) => _facilityClasses[fc.Id] = fc;
    public IEnumerable<FacilityBaseClass> GetAllFacilityClasses() => _facilityClasses.Values;// for use in testing
    
    public ItemClassBase GetItemClass(string classId) => _fungibleItemClasses[classId];
    public void AddItemClass(ItemClassBase ic) => _fungibleItemClasses[ic.Id] = ic;
    public IEnumerable<ItemClassBase> GetAllItemClasses() => _fungibleItemClasses.Values;// for use in testing
    
    public void AddRecipe(ComponentRecipe recipe) => _recipes.Add(recipe.Id, recipe);

    public ComponentRecipe GetRecipe(string id) => _recipes[id];
    public IEnumerable<ComponentRecipe> GetAllRecipes() => _recipes.Values; // for use in testing

    public StaticDataSubSystem()
    {
        Info = new StaticDataInfo()
        {
            Author = "Generated",
            Comments = "Generated",
            Date = DateTime.Today.ToString("yyyy.MM.dd"),
            Version = $"{DateTime.Now:yyyy.MM.dd.HHmm}",
        };
    }
    public byte[] SerializeJSON()
    {
        var data = new StaticDataSerializationStruct
        {
            Info = Info,
            Facility = _facilityClasses.Values.ToList(),
            Items = _fungibleItemClasses.Values.ToList(),
            Recipes = _recipes.Values.ToList()
        };
        DataContractJsonSerializer ser = new DataContractJsonSerializer(data.GetType());
        using (MemoryStream ms = new MemoryStream(100000))
        {
            ser.WriteObject(ms, data);
            byte[] json = ms.ToArray();
            ms.Close();
            return json;
        }
    }

    public StaticDataInfo Info { get; set; }

    public void DeserializeJSON(byte[] buffer)
    {
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(StaticDataSerializationStruct));
        using (var ms = new MemoryStream(buffer))
        {
            var obj = ser.ReadObject(ms);
            if (obj == null)
                throw new InvalidDataException($"No data could be read from a stream of size {buffer.Length}");
            if (obj is StaticDataSerializationStruct sd)
            {
                _fungibleItemClasses = sd.Items.ToDictionary(x => x.Id);
                _facilityClasses = sd.Facility.ToDictionary(x => x.Id);
                _recipes = sd.Recipes.ToDictionary(x => x.Id);
            }
            else
                throw new InvalidDataException($"Invalid read object type. Was expecting {typeof(StaticDataSerializationStruct)}" +
                                               $"but was {obj.GetType()}");
        }
    }
}

[DataContract(Name="StaticData")]
public class StaticDataSerializationStruct
{
    [DataMember]
    public StaticDataInfo Info { get; set; }
    [DataMember]
    public List<ItemClassBase> Items { get; set; }
    [DataMember]
    public List<FacilityBaseClass> Facility { get; set; }
    [DataMember]
    public List<ComponentRecipe> Recipes { get; set; }
}

[DataContract(Name="StaticDataVersion")]
public class StaticDataInfo
{
    [DataMember]
    public string Date { get; set; }
    [DataMember]
    public string Version { get; set; }
    [DataMember]
    public string Author { get; set; }
    [DataMember]
    public string Comments { get; set; }
}
    
public enum Category
{
    none,
    factory,
    component,
    recipe,
    resource
}

public static class StaticIdPathEtensions
{
    public static string Path(this Category c, params string[] args)
    {
        return $"{c}.{string.Join(".",args)}";
    }
}