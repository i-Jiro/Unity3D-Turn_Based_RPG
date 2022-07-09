using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAbility : Ability, IEnemyTargetable
{
    public AttackAbility(AbilityData data, GameObject source, List<StatusEffectData> statusEffectDataList) : base(data, source, statusEffectDataList) { }
    public void Trigger(Hero heroUser, Enemy enemyTarget)
    {
        AttackAbilityData data = abilityData as AttackAbilityData;
        data.Trigger(heroUser, enemyTarget);
        foreach(StatusEffect status in statusEffects)
        {
            heroUser.AddStatusEffect(status);
        }
    }
}
