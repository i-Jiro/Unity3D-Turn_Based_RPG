using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect
{
    public StatusEffectData Data;
    public object target;
    private int _currentTurnDuration;
    public bool isFinished = false;
    protected int _effectStacks;

    public StatusEffect(StatusEffectData data, int duration, object target)
    {
        this.target = target;
        Data = data;
    }

    public void Tick()
    {
        if(_currentTurnDuration > 0)
        {
            _currentTurnDuration--;
        }
        else if(_currentTurnDuration <= 0)
        {
            isFinished = true;
            End();
        }
    }

    public void Start()
    {
        isFinished = false;
        if (Data.isEffectStackable || _currentTurnDuration == 0)
        {
            ApplyEffect();
            _effectStacks++;
        }

        if(Data.isDurationStackable || _currentTurnDuration == 0)
        {
            _currentTurnDuration += Data.turnDuration;
        }
    }

    protected abstract void ApplyEffect();
    public virtual void End()
    {
        _effectStacks = 0;
    }
}
