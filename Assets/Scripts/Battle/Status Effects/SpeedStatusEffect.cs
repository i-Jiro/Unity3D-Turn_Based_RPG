using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedStatusEffect : StatusEffect
{
    Hero _hero;
    SpeedStatusEffectData speedStatusEffect;
    public SpeedStatusEffect(SpeedStatusEffectData data, int duration, GameObject target) : base(data, duration, target)
    {
        speedStatusEffect = Data as SpeedStatusEffectData;
        _hero = target.GetComponent<Hero>();
    }

    protected override void ApplyEffect()
    {
        _hero.AddModifier(speedStatusEffect.speedModifier);
    }

    public override void End()
    {
        _hero.RemoveAllModifierFromSource(speedStatusEffect);
        base.End();
    }
}
