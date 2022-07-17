using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Item> _items;
    private readonly Dictionary<ItemData, Item> _itemDictionary;
    public IReadOnlyList<Item> Items;

    public Inventory()
    {
        _items = new List<Item>();
        _itemDictionary = new Dictionary<ItemData, Item>();
        Items = _items;
    }

    public void AddItem(ItemData itemData)
    {
        if(_itemDictionary.TryGetValue(itemData,out Item item))
        {
            item.AddToStack();
        }
        else
        {
            item = itemData.Initialize(this);
            _items.Add(item);
            _itemDictionary.Add(itemData, item);  
        }
    }

    public bool RemoveItem(ItemData itemData)
    {
        if (_itemDictionary.TryGetValue(itemData, out Item item))
        {
            if (item.Quantity > 1)
            {
                item.RemoveFromStack();
                return true;
            }
            else
            {
                _itemDictionary.Remove(itemData);
                _items.Remove(item);
                return true;
            }
        }
        return false;
    }
}
