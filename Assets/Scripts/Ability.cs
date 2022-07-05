using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    Attack, Buff, Heal
}
[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability", order = 1)]
public class Ability : ScriptableObject
{
    public string AbilityName = "New Ability";
    public int baseDamage = 1;
    public int manaCost = 1;
    public AbilityType abilityType;
    public GameObject userParticlePrefab;
    public GameObject targetParticlePrefb;
}
