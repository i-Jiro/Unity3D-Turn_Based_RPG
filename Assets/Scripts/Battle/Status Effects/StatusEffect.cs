using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect
{
    public StatusEffectData Data;
    private int _currentTurnDuration;
    public bool isFinished = false;
    protected int _effectStacks;

    public StatusEffect(StatusEffectData data)
    {
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

    public void Start(Battler battler)
    {
        isFinished = false;
        if (Data.isEffectStackable || _currentTurnDuration == 0)
        {
            ApplyEffect(battler);
            _effectStacks++;
        }

        if(Data.isDurationStackable || _currentTurnDuration == 0)
        {
            _currentTurnDuration += Data.turnDuration;
        }
    }

    protected abstract void ApplyEffect(Battler battle);
    public virtual void End()
    {
        _effectStacks = 0;
    }
}
