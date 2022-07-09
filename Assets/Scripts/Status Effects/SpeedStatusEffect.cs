using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedStatusEffect : StatusEffect
{
    Hero _hero;
    public SpeedStatusEffect(SpeedStatusEffectData data, int duration, GameObject target) : base(data, duration, target)
    {
        _hero = target.GetComponent<Hero>();
    }

    protected override void ApplyEffect()
    {
        SpeedStatusEffectData speedStatusEffect = Data as SpeedStatusEffectData;
        _hero.AddModifier(speedStatusEffect.speedModifier);
    }

    public override void End()
    {
        SpeedStatusEffectData speedStatusEffect = Data as SpeedStatusEffectData;
        _hero.RemoveAllModifierFromSource(speedStatusEffect);
        base.End();
    }
}
