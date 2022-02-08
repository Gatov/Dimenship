using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using DimenshipBase.FungibleItems;

namespace DimenshipBase
{


    public class StaticDataSubSystem : ISystemSubState
    {
        public string Id => "StaticData";
        public string Name => "Static data subsystem";
        private Dictionary<string, ItemClassBase> FungibleItemClasses = new Dictionary<string, ItemClassBase>();
        private Dictionary<string, FacilityBaseClass> FacilityClasses = new Dictionary<string, FacilityBaseClass>();
        private Dictionary<string, ComponentRecipe> Recipes = new Dictionary<string, ComponentRecipe>();

        public FacilityBaseClass GetFacilityClass(string classId) { return FacilityClasses[classId]; }
        public void AddFacilityClass(FacilityBaseClass fc) { FacilityClasses[fc.Id] = fc; }
        public ItemClassBase GetItemClass(string classId) {return FungibleItemClasses[classId]; }
        public void AddItemClass(ItemClassBase ic) {FungibleItemClasses[ic.Id] = ic; }
        public void AddRecipe(ComponentRecipe recipe)
        {
            Recipes.Add(recipe.Id, recipe);
        }

        public ComponentRecipe GetRecipe(string id)
        {
            return Recipes[id];
        }

        public byte[] SerializeJSON()
        {
            var data = new StaticDataSerializationStruct
            {
                Facility = FacilityClasses.Values.ToList(),
                Items = FungibleItemClasses.Values.ToList(),
                Recipes = Recipes.Values.ToList()
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
                    FungibleItemClasses = sd.Items.ToDictionary(x => x.Id);
                    FacilityClasses = sd.Facility.ToDictionary(x => x.Id);
                    Recipes = sd.Recipes.ToDictionary(x => x.Id);
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
        public List<ItemClassBase> Items { get; set; }
        [DataMember]
        public List<FacilityBaseClass> Facility { get; set; }
        [DataMember]
        public List<ComponentRecipe> Recipes { get; set; }
    }
    
    public enum Category
    {
        none,
        factory,
        component,
        recipe,
        recource
    }

    public static class StaticIdPathEtensions
    {
        public static string Path(this Category c, params string[] args)
        {
            return $"{c}.{string.Join(".",args)}";
        }
    }
}