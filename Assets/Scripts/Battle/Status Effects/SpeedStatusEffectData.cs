using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Speed Status Effect", menuName = "Status Effect/Speed Status Effect")]
public class SpeedStatusEffectData : StatusEffectData
{
    public float ModifierValue = 0.0f;
    public StatModifierType ModifierType;
    public StatModifier speedModifier;

    public override StatusEffect Initialize()
    {
        return new SpeedStatusEffect(this);
    }

    private void OnEnable()
    {
        speedModifier = new StatModifier(ModifierValue, ModifierType, this);
    }

}
