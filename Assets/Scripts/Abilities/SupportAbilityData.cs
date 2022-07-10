using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Support Ability", menuName = "Abilities/Support Ability", order = 1)]
public class SupportAbilityData : AbilityData
{

    public void Trigger(Hero heroUser)
    {
        Debug.Log(heroUser.Name + " used " + AbilityName);
        if (targetParticlePrefab != null)
        {
            Instantiate(userParticlePrefab, heroUser.transform.position, userParticlePrefab.transform.rotation);
        }
        if (userParticlePrefab != null)
        {
            Instantiate(userParticlePrefab, heroUser.transform.position, userParticlePrefab.transform.rotation);
        }
    }
    public override Ability Initialize(GameObject source)
    {
        return new SupportAbility(this, source, statusEffectDataList);
    }
}
