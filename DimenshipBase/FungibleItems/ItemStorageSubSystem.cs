using System.Collections.Generic;

namespace DimenshipBase;

public class ItemStorageSubSystem : ISystemSubState
{
    public double MaxVolume { get; set; }
    public Dictionary<string, InventoryItem> Storage = new Dictionary<string, InventoryItem>();

    public int Check(string itemId)
    {
        if (Storage.TryGetValue(itemId, out var item))
        {
            return item.Count;
        }

        return 0;
    }

    public bool Book(string itemId, int quantity)
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

        return false;
    }
    public void Store(string itemId, int quantity)
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

    public string Id => "Storage";
    public string Name => "Storage";
}