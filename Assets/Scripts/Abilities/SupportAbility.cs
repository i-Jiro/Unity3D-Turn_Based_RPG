using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportAbility : Ability, ISelfTargetable
{
    public SupportAbility(AbilityData data, GameObject source, List<StatusEffectData> statusEffectDataList) : base (data,source,statusEffectDataList) { }

    public void Trigger(Hero hero)
    {
        SupportAbilityData data = abilityData as SupportAbilityData;
        data.Trigger(hero);
        foreach(StatusEffect status in statusEffects)
        {
            hero.AddStatusEffect(status);
        }
    }
}
