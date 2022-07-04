using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public string charName = "";
    public float health = 100;
    public float maxHealth = 100;
    public float mana = 100;
    public float maxMana = 100;
    public float defence = 10;
    public float speed = 1;

    public List<Ability> abilities;

    private HeroUIController _heroUI;

    protected float turnTimer = 0;
    protected float turnTimerMax = 100;

    // Update is called once per frame
    void Update()
    {
        if(!BattleManager.Instance.isActiveTurn)
            ChargeTurnTimer();
    }

    public virtual void Attack(Enemy enemy)
    {
        Debug.Log(gameObject.name + " attacked " + enemy.gameObject.name);
        enemy.TakeDamage(10); //placeholder damage
        EndTurn();
    }

    public virtual void UseAbility(Enemy enemy, Ability ability)
    {
        ability.TriggerAbility(enemy);
        mana -= ability.manaCost;
        _heroUI.UpdateMana(mana);
    }

    public virtual void Defend()
    {
        defence *= 2;
        Debug.Log(gameObject.name + " defends.");
        EndTurn();
    }

    public virtual void TakeDamage(float rawDamage)
    {
        health -= rawDamage;
        if (_heroUI != null)
            _heroUI.UpdateHealth(health);
    }

    //Charges character's turn meter based on it's speed.
    public virtual void ChargeTurnTimer()
    {
        if (turnTimer < turnTimerMax)
        {
            turnTimer += Time.deltaTime * speed;
            if (_heroUI != null)
                _heroUI.UpdateTurnTimer(turnTimer);
        }
        else if (turnTimer > turnTimerMax)
        {
            TakeTurn();
            Debug.Log(gameObject.name + " has reached it's turn.");
        }
    }

    public void SetHeroUI(HeroUIController heroUIController)
    {
        _heroUI = heroUIController;
    }

    public virtual void TakeTurn()
    {
        BattleManager.Instance.TakeActiveTurn(this);
    }

    public virtual void EndTurn()
    {
        turnTimer = 0;
        BattleManager.Instance.EndTurn();
    }

}
