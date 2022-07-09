using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Speed Status Effect", menuName = "Status Effect/Speed Status Effect")]
public class SpeedStatusEffectData : StatusEffectData
{
    public StatModifier speedModifier = new StatModifier(1, StatModifierType.PercentAdd);
    public override StatusEffect Initialize(GameObject target)
    {
        return new SpeedStatusEffect(this, turnDuration, target);
    }
}
