using UnityEngine;

public abstract class StatusModifier
{
    public StatusModifierData modifierData;
    protected float _currentTurnDuration;
    protected int effectStacks;
    protected readonly GameObject entity;
    public bool isFinished;

    public StatusModifier(StatusModifierData modData, GameObject entity)
    {
        this.entity = entity;
        this.modifierData = modData;
        isFinished = false;
    }

    public void Tick()
    {
        _currentTurnDuration -= 1;
        if(_currentTurnDuration <= 0)
        {
            End();
            isFinished = true;
        }
    }

    public void Activate()
    {
        if (modifierData.isEffectStackable || _currentTurnDuration == 0)
        {
            ApplyEffects();
            effectStacks++;
        }

        if(modifierData.isDurationStackable || _currentTurnDuration == 0)
        {
            _currentTurnDuration += modifierData.turnDuration;
        }
    }

    protected abstract void ApplyEffects();
    public abstract void End();
}
