using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAbility : Ability
{
    public AttackAbility(AbilityData data, GameObject source, List<StatusEffectData> statusEffectDataList) : base(data, source, statusEffectDataList) { }
    public virtual void Trigger(Hero heroUser, Enemy enemyTarget, out float rawDamage)
    {
        AttackAbilityData data = abilityData as AttackAbilityData;
        data.TriggerEffect(heroUser, enemyTarget);
        rawDamage = heroUser.CalculateDamage(Multiplier);
        if (statusEffects == null) return;
        foreach (StatusEffect status in statusEffects)
        {
            heroUser.AddStatusEffect(status);
        }
    }
}
