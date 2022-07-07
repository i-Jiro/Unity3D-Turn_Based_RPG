using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusModifierData : ScriptableObject
{
    public string modifierName = "";
    public int turnDuration;
    public bool isDurationStackable;
    public bool isEffectStackable;

    public abstract StatusModifier InitializeStatusModifier(GameObject entity);
}
