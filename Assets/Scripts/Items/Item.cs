using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public readonly ItemData ReferenceData;
    public readonly Inventory Inventory;
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int Quantity { get; private set; }

    protected Item(ItemData referenceData, Inventory inventory)
    {
        ReferenceData = referenceData;
        Name = ReferenceData.Name;
        Description = ReferenceData.Description;
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

    public abstract void Use(Battler user);
}
