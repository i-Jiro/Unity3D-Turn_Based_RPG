using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectData : ScriptableObject
{
    public string Name;
    public int turnDuration;
    public bool isEffectStackable;
    public bool isDurationStackable;
    public abstract StatusEffect Initialize();
}
