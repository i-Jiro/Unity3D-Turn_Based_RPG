using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public readonly ItemData Data;
    public readonly Inventory Inventory;
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int Quantity { get; private set; }

    protected Item(ItemData data, Inventory inventory)
    {
        Data = data;
        Name = Data.Name;
        Description = Data.Description;
        Quantity = 1;
        Inventory = inventory;
    }
    
    public void AddToStack()
    {
        Quantity++;
    }

    public void RemoveFromStack()
    {
        Quantity--;
    }

    public abstract void Use();
}
