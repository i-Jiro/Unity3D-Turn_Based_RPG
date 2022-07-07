using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff Ability", menuName = "Abilities/Buff Ability", order = 1)]
public class BuffAbility : Ability
{
    //note to self: use enums to distringuish types of buffs?
    public void TriggerAbility(Hero heroUser)
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
