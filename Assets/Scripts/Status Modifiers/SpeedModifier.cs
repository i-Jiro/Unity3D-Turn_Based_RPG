using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedModifier : StatusModifier
{
    private Hero _hero;
    public SpeedModifier(StatusModifierData modifierData, GameObject entity) : base(modifierData, entity)
    {
        _hero = entity.GetComponent<Hero>();
    }
    protected override void ApplyEffects()
    {
        
    }

    public override void End()
    {
        throw new System.NotImplementedException();
    }

}
