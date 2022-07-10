using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability
{
    public string Name;
    public float Multiplier { get; }
    public float ManaCost { get; }
    public GameObject Source { get; }
    protected readonly AbilityData abilityData;
    protected readonly List<StatusEffectData> statusEffectsDataList;
    protected readonly List<StatusEffect> statusEffects;

    public Ability(AbilityData data, GameObject source, List<StatusEffectData> statusEffectDataList)
    {
        abilityData = data;
        ManaCost = data.ManaCost;
        Name = data.AbilityName;
        Multiplier = data.Multiplier;
        statusEffectsDataList = statusEffectDataList;
        statusEffects = new List<StatusEffect>();
        Source = source;
        InitializeStatusEffect();
    }

    public Ability(AbilityData data, GameObject source) : this( data, source, null) {}

    protected void InitializeStatusEffect()
    {
        if(statusEffectsDataList != null)
        {
            foreach(StatusEffectData data in statusEffectsDataList)
            {
                StatusEffect statusEffect = data.Initialize();
                statusEffects.Add(statusEffect);
            }
        }
    }
}
