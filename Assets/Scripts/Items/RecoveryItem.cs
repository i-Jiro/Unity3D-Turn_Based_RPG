using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RecoveryItem : Item
{
    private ItemRecoveryType _recoveryType;
    private float _recoveryAmount;
    public RecoveryItem(ItemData referenceData, Inventory inventory): base (referenceData,inventory)
    {
        var data = ReferenceData as RecoveryItemData;
        if (data == null) return;
        _recoveryType = data.RecoveryType;
        _recoveryAmount = data.AmountToRecover;
    }

    public override void Use(Battler user)
    {
        switch (_recoveryType)
        {
            case ItemRecoveryType.Health:
                user.RecoverHealth(_recoveryAmount);
                break;
            case ItemRecoveryType.Mana:
                user.RecoverMana(_recoveryAmount);
                break;
        }
        Inventory.RemoveItem(ReferenceData);
    }
}
