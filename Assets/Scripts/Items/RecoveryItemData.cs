using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemRecoveryType {Health, Mana}
public class RecoveryItemData : ItemData
{
    public ItemRecoveryType RecoveryType;
    public float AmountToRecover;
    
    public override Item Initialize(Inventory inventory)
    {
        return new RecoveryItem(this,inventory);
    }
}
