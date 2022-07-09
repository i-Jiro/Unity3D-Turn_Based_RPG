using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public string Name;
    public GameObject Source { get; }
    protected readonly AbilityData abilityData;
    protected readonly List<StatusEffectData> statusEffectsDataList;
    protected List<StatusEffect> statusEffects;

    public Ability(AbilityData data, GameObject source, List<StatusEffectData> statusEffectDataList)
    {
        abilityData = data;
        Name = data.AbilityName;
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
                StatusEffect statusEffect = data.Initialize(Source);
                statusEffects.Add(statusEffect);
            }
        }
    }
}
