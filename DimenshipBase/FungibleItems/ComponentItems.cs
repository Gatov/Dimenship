using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace DimenshipBase.FungibleItems
{
    /// <summary>
    /// Data class for an ingredient used for production
    /// </summary>
    public class Ingredient
    {
        [XmlAttribute("Id")] public string ResourceId { get; set; }
        [XmlAttribute] public int Quantity { get; set; }
    }

    /// <summary>
    /// Recipe for a facility to produce an item
    /// </summary>
    public class ComponentRecipe : ItemClassBase
    {
        //[XmlAttribute] public string RecipeId { get; set; }
        [XmlAttribute] public int BaselineBuildTime { get; set; }
        [XmlElement("Ingridient")] public List<Ingredient> BaselineIngredientList;
    }
    
    
    
}