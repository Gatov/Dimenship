using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace DimenshipBase
{
    public class ItemClassDictionary
    {
    }
    
    public class Storage
    {
        public string Name { get; set; }
        private readonly Dictionary<string, IInventoryItem> Inventory = new Dictionary<string, IInventoryItem>();
        private readonly object _syncRoot = new object();

        public List<IInventoryItem> GetAll()
        {
            lock (_syncRoot)
            {
                return Inventory.Values.ToList(); // lock to avoid concurrent access to the dictionary 
            }
        }

        
        public int CheckInventory(string itemClassId)
        {
            lock (_syncRoot)
            {
                if (Inventory.TryGetValue(itemClassId, out var storedItem))
                {
                    return storedItem.Count;
                }
            }

            return 0;
        }
        public bool Contains(string itemClassId, int count)
        {
            return CheckInventory(itemClassId) >= count;
        }
        
        /// <summary>
        /// we will create storage with required materials/items and then check/extract it from our inventory
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool ExtractStorage(Storage target)
        {
            lock (_syncRoot)
            {
                bool success = true;
                var list = target.GetAll();
                if (list.Any(x => !Contains(x.Class.Id, x.Count)))
                    return false;
                foreach (var item in list)
                {
                    if (Inventory.TryGetValue(item.Class.Id, out var stored))
                    {
                        stored.Retrieve(item.Count);
                        if (stored.Count == 0)
                            Inventory.Remove(stored.Class.Id);
                    }
                }

                return true;
            }
        }

        public void Add(ItemClassBase itemClass, int count)
        {
            lock (_syncRoot)
            {
                IInventoryItem storedItem;
                if (!Inventory.TryGetValue(itemClass.Id, out storedItem))
                {
                    var newItem = new InventoryItem(itemClass);
                    newItem.Add(count);
                    Inventory.Add(itemClass.Id, newItem);
                }
            }
        }
        
    }
    
    /// <summary>
    /// Basic fungible item
    /// </summary>
    public interface IInventoryItem
    {
        int Count { get; }
        ItemClassBase Class { get; }
        void Add(int count);
        void Retrieve(int count);
    }

    [Serializable]
    public class ItemClassBase
    {
        [XmlAttribute] public string Id { get; set; }
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public string Tags { get; set; }
        [XmlAttribute] public string GlyphName { get; set; }
        [XmlAttribute] public double Volume { get; set; }
        [XmlAttribute] public double Weight { get; set; }
        public string Description { get; set; }
    }
    

    /// <summary>
    /// Represents stack of items
    /// </summary>
    public class InventoryItem : IInventoryItem
    {
        public InventoryItem(ItemClassBase @class)
        {
            Class = @class;
        }

        public int Count { get; private set; }
        public ItemClassBase Class { get; }
        public void Add(int count)
        {
            Count += count;
        }

        public void Retrieve(int count)
        {
            Count = Math.Max(0, Count-count); // take as much as we can
        }
    }
}