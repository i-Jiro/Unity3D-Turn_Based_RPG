using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Support Ability", menuName = "Abilities/Support Ability", order = 1)]
public class SupportAbility : AbilityData
{
    public StatusEffectData statusEffect;
    public void Trigger(Hero heroUser)
    {
        Debug.Log(heroUser.Name + " used " + AbilityName);
        if (targetParticlePrefb != null)
        {
            Instantiate(userParticlePrefab, heroUser.transform.position, userParticlePrefab.transform.rotation);
        }
        if (userParticlePrefab != null)
        {
            Instantiate(userParticlePrefab, heroUser.transform.position, userParticlePrefab.transform.rotation);
        }

        heroUser.AddStatusEffect(statusEffect.Initialize(heroUser.gameObject));
    }
}
