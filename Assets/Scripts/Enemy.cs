using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected string charName = "";
    public string Name
    {
        get { return charName; }
        private set { if (value.Length > 8) { Debug.LogWarning("Enemy set with a name longer than 8 characters!"); } }
    }
    [SerializeField] protected float currentHealth = 100;
    [SerializeField] protected float currentMana = 100;
    [SerializeField] protected float defence = 10;
    [SerializeField] protected float speed = 1;

    protected float turnTimer = 0;
    protected float turnTimerMax = 100;
    protected bool isTurnTimerActive = false;

    protected List<Hero> heroes;

    public delegate void EventTakeTurn(Enemy enemy);
    public event EventTakeTurn OnTakeActiveTurn;
    public delegate void EventEndTurn();
    public event EventEndTurn OnEndTurn;

    private void Start()
    {
        isTurnTimerActive = true;
        if (FindObjectOfType<BattleManager>())
        {
            BattleManager.OnActiveTurn += ToggleTurnTimer;
            heroes = BattleManager.Instance.heroes;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTurnTimerActive)
            TickTurnTimer();
    }

    public virtual void ToggleTurnTimer(bool value)
    {
        isTurnTimerActive = !value;
    }

    public virtual void Attack(Hero hero)
    {
        Debug.Log(gameObject.name + " attacked " + hero.gameObject.name);
        hero.TakeDamage(15);
        EndTurn();
    }

    public virtual void TakeDamage(float rawDamage)
    {
        currentHealth -= rawDamage;
    }

    public virtual Hero PickRandomHero()
    {
        int index = Random.Range(0, heroes.Count);
        Hero hero = heroes[index];
        return hero;
    }

    public virtual void TickTurnTimer()
    {
        if (turnTimer < turnTimerMax)
        {
            turnTimer += Time.deltaTime * speed;
        }
        else if (turnTimer > turnTimerMax)
        {
            Debug.Log(gameObject.name + " has reached it's turn.");
            StartTurn();         
        }
    }

    public virtual void StartTurn()
    {
        OnTakeActiveTurn.Invoke(this);
        Attack(PickRandomHero());
    }
    public virtual void EndTurn()
    {
        turnTimer = 0;
        OnEndTurn.Invoke();
    }

    private void OnDestroy()
    {
        if (FindObjectOfType<BattleManager>())
        {
            BattleManager.OnActiveTurn -= ToggleTurnTimer;
        }
    }
}
