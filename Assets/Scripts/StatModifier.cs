using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatModifierType { Flat, PercentAdd, PercentMultiply}

public class StatModifier
{
    public readonly int order;
    public readonly float value;
    public readonly StatModifierType type;
    public readonly object source;
    public StatModifier(float value, StatModifierType type, int order, object source)
    {
        this.value = value;
        this.order = order;
        this.source = source;
    }

    public StatModifier(float value, StatModifierType type) : this(value, type, (int)type, null) { }
    public StatModifier(float value, StatModifierType type, int order) : this(value, type, order, null) { }
    public StatModifier(float value, StatModifierType type, object source) : this(value, type, (int)type, source) { }

}
