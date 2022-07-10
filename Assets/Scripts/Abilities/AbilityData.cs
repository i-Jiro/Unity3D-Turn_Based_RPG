using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityData : ScriptableObject
{
    public string AbilityName = "New Ability";
    public int Multiplier = 1;
    public int ManaCost = 1;
    public GameObject userParticlePrefab;
    public GameObject targetParticlePrefab;
    public List<StatusEffectData> statusEffectDataList;

    public abstract Ability Initialize(GameObject source);
}
