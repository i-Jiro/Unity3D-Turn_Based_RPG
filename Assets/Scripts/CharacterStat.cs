using System.Collections;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using UnityEngine;

public enum StatType { MaxHealth, MaxMana, PhysicalAttack, PhysicalDefense, Speed, Critical, Evasion}


[System.Serializable]
public class CharacterStat 
{
    public float BaseValue;
    public readonly StatType Type;
    private float _lastBaseValue = float.MinValue;
    private bool _isDirty = true;
    private readonly List<StatModifier> _statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;
    [SerializeField]
    private float _value;
    public float Value
    {
        get 
        { 
            if (_isDirty || _lastBaseValue != BaseValue)
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

    public CharacterStat(float baseValue, StatType type) : this()
    {
        this.BaseValue = baseValue;
        this.Type = type;
    }

    public CharacterStat(StatType type) : this(0, type) {}

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
            if(_statModifiers[i].Source == source)
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
        if(a.Order < b.Order)
        {
            return -1;
        }
        else if(a.Order > b.Order)
        {
            return 1;
        }

        return 0;
    }

    private float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0;
        for(int i = 0; i < _statModifiers.Count; i++)
        {
            if(_statModifiers[i].Type == StatModifierType.Flat)
            {
                finalValue += _statModifiers[i].Value;
            }
            else if(_statModifiers[i].Type == StatModifierType.PercentAdd)
            {
                sumPercentAdd += _statModifiers[i].Value;
                if(_statModifiers.Count - 1 < i + 1 || _statModifiers[i+1].Type != StatModifierType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if(_statModifiers[i].Type == StatModifierType.PercentMultiply)
            {
                finalValue *= 1 + _statModifiers[i].Value;
            }
        }
        _isDirty = false;
        return (float)Math.Round(finalValue, 4);
    }

}
