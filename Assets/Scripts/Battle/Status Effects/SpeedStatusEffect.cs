using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedStatusEffect : StatusEffect
{
    SpeedStatusEffectData _speedStatusEffect;
    Battler _battler;
    public SpeedStatusEffect(SpeedStatusEffectData data) : base(data)
    {
        _speedStatusEffect = Data as SpeedStatusEffectData;
    }

    protected override void ApplyEffect(Battler battler)
    {
       _battler = battler;
       _battler.AddModifier(_speedStatusEffect.speedModifier);
    }

    public override void End()
    {
       _battler.RemoveAllModifierFromSource(_speedStatusEffect);
        base.End();
    }
}
