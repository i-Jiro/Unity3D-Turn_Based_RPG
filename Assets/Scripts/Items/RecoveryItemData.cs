using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemRecoveryType {Health, Mana}

[CreateAssetMenu(fileName = "New Recovery Item", menuName = "Item/Recovery Item")]
public class RecoveryItemData : ItemData
{
    public ItemRecoveryType RecoveryType;
    public float AmountToRecover;
    
    public override Item Initialize(Inventory inventory)
    {
        return new RecoveryItem(this,inventory);
    }
}
