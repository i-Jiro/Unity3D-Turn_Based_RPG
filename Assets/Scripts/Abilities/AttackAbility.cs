using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAbility : Ability, IEnemyTargetable
{
    public AttackAbility(AbilityData data, GameObject source, List<StatusEffectData> statusEffectDataList) : base(data, source, statusEffectDataList) { }
    public virtual void Trigger(Hero heroUser, Enemy enemyTarget)
    {
        AttackAbilityData data = abilityData as AttackAbilityData;
        data.TriggerEffect(heroUser, enemyTarget);
        float rawDamage = heroUser.CalculateDamage(Multiplier);
        enemyTarget.TakeDamage(rawDamage);
        foreach (StatusEffect status in statusEffects)
        {
            heroUser.AddStatusEffect(status);
        }
    }
}
