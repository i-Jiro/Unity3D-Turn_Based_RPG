using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Support Ability", menuName = "Abilities/Support Ability", order = 1)]
public class SupportAbility : AbilityData
{
    //note to self: use enums to distringuish types of buffs?
    // rename class to SupportAbiility for clarity later
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
        // Do something with hero stats here
    }
}
