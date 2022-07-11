using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatModifierType { Flat = 100, PercentAdd = 200, PercentMultiply = 300}

public class StatModifier
{
    public readonly int Order;
    public readonly float Value;
    public readonly StatModifierType Type;
    public readonly StatType StatType;
    public readonly object Source;
    public StatModifier(float value, StatType statType, StatModifierType type, int order, object source)
    {
        this.Value = value;
        this.Type = type;
        this.Order = order;
        this.Source = source;
        this.StatType = statType;
    }

    public StatModifier(float value, StatType statype, StatModifierType type) : this(value, statype, type, (int)type, null) { }
    public StatModifier(float value, StatType statype, StatModifierType type, int order) : this(value, statype, type, order, null) { }
    public StatModifier(float value, StatType statype, StatModifierType type, object source) : this(value, statype, type, (int)type, source) { }

}
