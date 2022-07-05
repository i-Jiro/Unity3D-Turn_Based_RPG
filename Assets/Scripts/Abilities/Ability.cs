using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string AbilityName = "New Ability";
    public int baseMagnitude = 1;
    public int manaCost = 1;
    public GameObject userParticlePrefab;
    public GameObject targetParticlePrefb;
}
