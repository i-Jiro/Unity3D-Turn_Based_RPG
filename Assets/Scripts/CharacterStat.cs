using System.Collections;
using System.Collections.Generic;
using System;


public class CharacterStat 
{
    public float baseValue;
    private bool _isDirty;
    private readonly List<StatModifier> _statModifiers;
    private float _value;
    public float Value
    {
        get 
        { 
            if (_isDirty)
            {
                _value = CaculateFinalValue();
            }
            return Value;
        }

    }

    public CharacterStat(float baseValue)
    {
        _isDirty = true;
        this.baseValue = baseValue;
        _statModifiers = new List<StatModifier>();
    }

    public void AddModifier(StatModifier mod)
    {
        _isDirty = true;
        _statModifiers.Add(mod);
    }

    public bool RemoveModifier(StatModifier mod)
    {
        _isDirty = true;
        return _statModifiers.Remove(mod);
    }


    private float CaculateFinalValue()
    {
        float finalValue = baseValue;
        for(int i = 0; i < _statModifiers.Count; i++)
        {
            finalValue += _statModifiers[i].value;
        }
        _isDirty = false;
        return (float)Math.Round(finalValue, 4);
    }

}
