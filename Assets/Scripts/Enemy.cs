using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100;
    public float mana = 100;
    public float defence = 10;
    public float speed = 1;

    protected float turnTimer = 0;
    protected float turnTimerMax = 100;

    // Update is called once per frame
    void Update()
    {
        if (!BattleManager.Instance.isActiveTurn)
            ChargeTurnTimer();
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
    }

    public virtual void ChargeTurnTimer()
    {
        if (turnTimer < turnTimerMax)
        {
            turnTimer += Time.deltaTime * speed;
        }
        else if (turnTimer > turnTimerMax)
        {
            TakeTurn();
            Debug.Log(gameObject.name + " has reached it's turn.");
        }
    }

    public virtual void TakeTurn()
    {
        BattleManager.Instance.TakeActiveTurn(this);
    }
}
