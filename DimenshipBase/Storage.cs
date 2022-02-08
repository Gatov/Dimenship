using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DimenshipBase
{
    /// <summary>
    /// Basic static class descriptor
    /// </summary>
    [Serializable]
    [DataContract]
    public class ClassBase
    {
        [DataMember(Order = 1)][XmlAttribute] public string Id { get; set; }
        [DataMember(Order = 2)][XmlAttribute] public string Name { get; set; }
        [DataMember(Order = 3)][XmlAttribute] public string Tags { get; set; }
        [DataMember(Order = 4)][XmlAttribute] public string GlyphName { get; set; }
        [DataMember(Order = 5)]public string Description { get; set; }
    }


    /// <summary>
    /// Basic fungible item
    /// </summary>
    public interface IInventoryItem
    {
        int Count { get; }
        string ClassId { get; }
        void Add(int count);
        void Retrieve(int count);
    }


    [Serializable]
    [DataContract]
    public class ItemClassBase : ClassBase
    {
        [DataMember] [XmlAttribute] public double Volume { get; set; }
        [DataMember] public double Weight { get; set; }
    }


    /// <summary>
    /// Represents stack of items
    /// </summary>
    public class InventoryItem : IInventoryItem
    {
        public InventoryItem(string  classId)
        {
            ClassId = classId;
        }

        public int Count { get; private set; }
        public string ClassId { get; }

        public void Add(int count)
        {
            Count += count;
        }

        public void Retrieve(int count)
        {
            Count = Math.Max(0, Count - count); // take as much as we can
        }
    }
}