using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Battler
{
    protected List<Hero> heroes;

    public delegate void EventTakeTurn(Enemy enemy);
    public event EventTakeTurn OnTakeActiveTurn;
    public delegate void EventEndTurn();
    public event EventEndTurn OnEndTurn;

    protected override void Start()
    {
        base.Start();
        heroes = BattleManager.Instance.heroes;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected virtual void Attack(Hero hero)
    {
        Debug.Log(gameObject.name + " attacked " + hero.gameObject.name);
        hero.TakeDamage(CalculateDamage(baseDamageMultiplier));
        EndTurn();
    }

    protected virtual Hero PickRandomHero()
    {
        int index = Random.Range(0, heroes.Count);
        Hero hero = heroes[index];
        return hero;
    }

    protected override void StartTurn()
    {
        OnTakeActiveTurn.Invoke(this);
        Attack(PickRandomHero());
    }

    protected override void EndTurn()
    {
        turnTimer = 0;
        OnEndTurn.Invoke();
    }

    protected void OnDestroy()
    {
        if (FindObjectOfType<BattleManager>())
        {
            BattleManager.OnActiveTurnChanged -= ToggleTurnTimer;
        }
    }
}
