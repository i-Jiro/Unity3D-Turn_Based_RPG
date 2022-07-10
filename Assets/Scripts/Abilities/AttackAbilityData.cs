using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Attack Ability", menuName = "Abilities/Attack Ability", order = 1)]
public class AttackAbilityData : AbilityData
{
    public void Trigger(Hero heroUser, Enemy enemyTarget)
    {
        Debug.Log(heroUser.Name + " used " + AbilityName + " on " + enemyTarget.gameObject.name);
        if (targetParticlePrefab != null)
        {
            Instantiate(targetParticlePrefab, enemyTarget.transform.position, targetParticlePrefab.transform.rotation);
        }
        if (userParticlePrefab != null)
        {
            Instantiate(userParticlePrefab, heroUser.transform.position, userParticlePrefab.transform.rotation);
        }
        enemyTarget.TakeDamage(baseMagnitude);
    }

    public override Ability Initialize(GameObject source)
    {
        return new AttackAbility(this, source, statusEffectDataList);
    }

}

