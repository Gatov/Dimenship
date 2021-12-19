using System;
using System.Xml.Serialization;

namespace DimenshipBase
{
    /// <summary>
    /// Basic static class descriptor
    /// </summary>
    [Serializable]
    public class ClassBase
    {
        [XmlAttribute] public string Id { get; set; }
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public string Tags { get; set; }
        [XmlAttribute] public string GlyphName { get; set; }
        public string Description { get; set; }
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
    public class ItemClassBase : ClassBase
    {
        [XmlAttribute] public double Volume { get; set; }
        [XmlAttribute] public double Weight { get; set; }
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