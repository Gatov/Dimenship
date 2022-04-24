using System.Collections.Generic;
using System.Linq;

namespace DimenshipBase.SubSystems;

public class ItemStorageSubSystem : ISystemSubState
{
    public double MaxVolume { get; set; }
    private readonly Dictionary<string, InventoryItem> Storage = new Dictionary<string, InventoryItem>();
    private readonly object _syncRoot = new object();

    public int Check(string itemId)
    {
        lock (_syncRoot)
        {
            if (Storage.TryGetValue(itemId, out var item))
            {
                return item.Count;
            }
        }

        return 0;
    }

    public bool Book(string itemId, int quantity)
    {
        lock (_syncRoot)
        {
            if (Storage.TryGetValue(itemId, out var item))
            {
                if (item.Count > quantity) // still have some left
                {
                    item.Retrieve(quantity);
                    return true;
                }

                if (item.Count == quantity) // last of items
                {
                    Storage.Remove(itemId);
                    return true;
                }

                return false; // not enough
            }
        }

        return false;
    }

    public void Store(string itemId, int quantity)
    {
        lock (_syncRoot)
        {
            if (Storage.TryGetValue(itemId, out var item))
            {
                item.Add(quantity);
            }
            else
            {
                var inventoryItem = new InventoryItem(itemId);
                inventoryItem.Add(quantity);
                Storage.Add(itemId, inventoryItem);
            }
        }
    }

    public List<InventoryItem> GetItemsOfCategory(string category)
    {
        lock (_syncRoot)
        {
            return Storage.Values.Where(x => x.ClassId.StartsWith(category)).ToList();
        }
    }

    public string Id => "Storage";
    public string Name => "Storage System";
}