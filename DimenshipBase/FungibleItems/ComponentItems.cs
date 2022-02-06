using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace DimenshipBase.FungibleItems
{
    /// <summary>
    /// Data class for an ingredient used for production
    /// </summary>
    [DataContract]
    public class Ingredient
    {
        [DataMember] [XmlAttribute("Id")] public string ResourceId { get; set; }
        [DataMember] [XmlAttribute] public int Required { get; set; }
    }

    /// <summary>
    /// Recipe for a facility to produce an item
    /// </summary>
    [DataContract]
    public class ComponentRecipe : ClassBase
    {
        //[XmlAttribute] public string RecipeId { get; set; }
        [DataMember]
        [XmlAttribute] public int BaselineBuildTime { get; set; }
        [DataMember] public string RequiredFacility { get; set; }
        [DataMember]
        [XmlElement("Ingridient")] public List<Ingredient> BaselineIngredientList;
    }
}