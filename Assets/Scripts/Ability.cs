using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability", order = 1)]
public class Ability : ScriptableObject
{
    public string AbilityName = "New Ability";
    public int baseDamage = 1;
    public int manaCost = 1;
    public GameObject userParticlePrefab;
    public GameObject targetParticlePrefb;
    private GameObject _user;
    //private GameObject _target;

    public void TriggerAbility(Enemy enemy)
    {
        GameObject _target = enemy.gameObject;
        if(targetParticlePrefb != null)
        {
            Instantiate(targetParticlePrefb, _target.transform.position, targetParticlePrefb.transform.rotation);
        }
        if(userParticlePrefab != null)
        {
            Instantiate(userParticlePrefab, _user.transform.position, userParticlePrefab.transform.rotation);
        }
        enemy.TakeDamage(baseDamage);
    }
}
