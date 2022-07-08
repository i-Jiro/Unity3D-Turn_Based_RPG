using System.Collections;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using UnityEngine;

[System.Serializable]
public class CharacterStat 
{
    public float baseValue;
    private float _LastBaseValue = float.MinValue;
    private bool _isDirty = true;
    private readonly List<StatModifier> _statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;
    [SerializeField]
    private float _value;
    public float Value
    {
        get 
        { 
            if (_isDirty || _LastBaseValue != baseValue)
            {
                _value = CalculateFinalValue();
            }
            return _value;
        }

    }

    public CharacterStat()
    {
        _statModifiers = new List<StatModifier>();
        StatModifiers = _statModifiers.AsReadOnly();
    }

    public CharacterStat(float baseValue) : this()
    {
        this.baseValue = baseValue;
    }

    public void AddModifier(StatModifier mod)
    {
        _isDirty = true;
        _statModifiers.Add(mod);
        _statModifiers.Sort(CompareModifierOrder);
    }

    public bool RemoveModifier(StatModifier mod)
    {
        if (_statModifiers.Remove(mod))
        {
            _isDirty = true;
            return true;
        }
        return false;
    }

    public bool RemoveAllModifierFromSource(object source)
    {
        bool didRemove = false;
        for(int i = _statModifiers.Count - 1; i >= 0; i--)
        {
            if(_statModifiers[i].source == source)
            {
                _isDirty = true;
                didRemove = true;
                _statModifiers.RemoveAt(i);
            }
        }
        return didRemove;
    }

    //Comparison method for .sort()
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

    private float CalculateFinalValue()
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
                if(_statModifiers.Count - 1 < i + 1 || _statModifiers[i+1].type != StatModifierType.PercentAdd)
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
