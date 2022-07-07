using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Attack Ability", menuName = "Abilities/Attack Ability", order = 1)]
public class AttackAbility : Ability
{
    public void TriggerAbility(Enemy enemyTarget, Hero heroUser)
    {
        Debug.Log(heroUser.Name + " used " + AbilityName + " on " + enemyTarget.gameObject.name);
        if (targetParticlePrefb != null)
        {
            Instantiate(targetParticlePrefb, enemyTarget.transform.position, targetParticlePrefb.transform.rotation);
        }
        if (userParticlePrefab != null)
        {
            Instantiate(userParticlePrefab, heroUser.transform.position, userParticlePrefab.transform.rotation);
        }
        enemyTarget.TakeDamage(baseMagnitude);
    }
}

