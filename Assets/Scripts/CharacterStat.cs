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
        _statModifiers.Sort(CompareModifierOrder);
    }

    public bool RemoveModifier(StatModifier mod)
    {
        _isDirty = true;
        return _statModifiers.Remove(mod);
    }

    private int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if(a.order < b.order)
        {
            return -1;
        }
        else if(a.order > b.order)
        {
            return 1;
        }

        return 0;
    }

    private float CaculateFinalValue()
    {
        float finalValue = baseValue;
        float sumPercentAdd = 0;
        for(int i = 0; i < _statModifiers.Count; i++)
        {
            if(_statModifiers[i].type == StatModifierType.Flat)
            {
                finalValue += _statModifiers[i].value;
            }
            else if(_statModifiers[i].type == StatModifierType.PercentAdd)
            {
                sumPercentAdd += _statModifiers[i].value;
                if(_statModifiers.Count < i + 1 || _statModifiers[i+1].type != StatModifierType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if(_statModifiers[i].type == StatModifierType.PercentMultiply)
            {
                finalValue *= 1 + _statModifiers[i].value;
            }
        }
        _isDirty = false;
        return (float)Math.Round(finalValue, 4);
    }

}
