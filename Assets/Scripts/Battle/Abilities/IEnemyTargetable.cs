using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyTargetable
{
    public void Trigger(Hero hero, Enemy enemy);
}
