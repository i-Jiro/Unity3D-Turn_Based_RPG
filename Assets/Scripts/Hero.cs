using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HeroAnimationHandler))]
public class Hero : MonoBehaviour
{
    [SerializeField] protected string charName = "";
     public string Name
     {
        get { return charName;}
        set { if(value.Length > 8) {Debug.LogWarning("Hero set with a name longer than 49 characters! Name will not fit UI.");}}
     }
    [SerializeField] protected float currentHealth = 100;
    public float CurrentHealth { get { return currentHealth; } private set { CurrentHealth = value; } }
    [SerializeField] protected float maxHealth = 100;
    public float MaxHealth { get { return maxHealth; } private set { maxHealth = value; } }
    [SerializeField] protected float currentMana = 100;
    public float CurrentMana { get { return currentMana; } private set { currentMana = value; } }
    [SerializeField] protected  float maxMana = 100;
    public float MaxMana { get { return maxMana; } private set { currentMana = value; } }
    [SerializeField] protected float manaRegenRate = 10;
    [SerializeField] protected float defence = 10;
    [SerializeField] protected float speed = 1;
    [SerializeField] protected float defenseMultiplier = 2f;

    [SerializeField] protected float MoveOffset = 3f;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float delaySeconds = 0.5f;

    public List<Ability> abilities;

    protected float turnTimer = 0;
    protected float turnTimerMax = 100;
    protected bool isTurnTimerActive = false;
    protected bool isDefending = false;

    private HeroAnimationHandler _animationHandler;

    public delegate void HealthEventHandler(float health);
    public event HealthEventHandler OnHealthChanged;
    public delegate void TurnTimerEventHandler(float time);
    public event TurnTimerEventHandler OnTurnTimeChanged;
    public delegate void ManaEventHandler(float mana);
    public event ManaEventHandler OnManaChanged;

    public delegate void EventTakeTurn(Hero hero);
    public event EventTakeTurn OnStartTurn;
    public delegate void EventEndTurn();
    public event EventEndTurn OnEndTurn;

    private void Awake()
    {
        _animationHandler = GetComponent<HeroAnimationHandler>();
    }

    private void Start()
    {
        isTurnTimerActive = true;
        if (FindObjectOfType<BattleManager>())
        {
            BattleManager.OnActiveTurn += ToggleTurnTimer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isTurnTimerActive)
            TickTurnTimer();
    }
    public virtual void ToggleTurnTimer(bool value)
    {
        isTurnTimerActive = !value;
    }
    public virtual void Attack(Enemy enemy)
    {
        _animationHandler.PlayAttack();
        Debug.Log(gameObject.name + " attacked " + enemy.gameObject.name);
        enemy.TakeDamage(10); //placeholder damage
    }

    //For abilities that target enemys
    public virtual void UseAbility(Enemy enemyTarget, Ability ability)
    {
        _animationHandler.PlaySpecialAttack();
        AttackAbility attackAbility = ability as AttackAbility;
        attackAbility.TriggerAbility(enemyTarget, this);
        if (OnManaChanged != null)
            OnManaChanged.Invoke(currentMana);
    }

    //For abilities that target self
    public virtual void UseAbility(Ability ability)
    {
        Debug.Log(charName + " used " + ability.name);
        _animationHandler.PlayBuff();
        BuffAbility buffAbility = ability as BuffAbility;
        if (ability.targetParticlePrefb != null)
        {
            Instantiate(ability.targetParticlePrefb, transform.position, ability.targetParticlePrefb.transform.rotation);
        }
        buffAbility.TriggerAbility(this);
        if (OnManaChanged != null)
            OnManaChanged.Invoke(currentMana);
    }

    //For abilities that target party members
    public virtual void UseAbility(Hero hero, Ability ability)
    {
    }

    public virtual void Defend()
    {
        isDefending = true;
        defence *= defenseMultiplier;
        _animationHandler.PlayDefend();
        Debug.Log(gameObject.name + " defends.");
    }

    public virtual void ResetDefence()
    {
        if (isDefending)
        {
            defence /= defenseMultiplier;
            isDefending = false;
            _animationHandler.StopDefend();
        }
    }

    public virtual void TakeDamage(float rawDamage)
    {
        _animationHandler.PlayGetDamaged();
        float damage = rawDamage - defence;
        if (damage < 0)
            damage = 0;
        currentHealth -= damage;
        if (OnHealthChanged != null)
            OnHealthChanged.Invoke(currentHealth);
    }

    public virtual void RegenerateMana()
    {
        if (currentMana + manaRegenRate > maxMana)
            currentMana = maxMana;
        else
            currentMana += manaRegenRate;
        if (OnManaChanged != null)
            OnManaChanged.Invoke(currentMana);
    }

    public virtual void UseMana(float manaUsed)
    {
        CurrentMana -= manaUsed;
    }

    //Charges character's turn meter based on it's speed.
    public virtual void TickTurnTimer()
    {
        if (turnTimer < turnTimerMax)
        {
            turnTimer += Time.deltaTime * speed;
            if (OnTurnTimeChanged != null)
                OnTurnTimeChanged.Invoke(turnTimer);
        }
        else if (turnTimer > turnTimerMax)
        {
            StartTurn();
            Debug.Log(gameObject.name + " has reached it's turn.");
        }
    }

    public virtual void StartTurn()
    {
        StartCoroutine(MoveLeft());
        _animationHandler.PlayMoveForward();
        _animationHandler.PlayReady();
        RegenerateMana();
        ResetDefence();
        OnStartTurn.Invoke(this);
    }

    //Called at the end of certain animations (attack, defend, abilities) by animations events.
    public virtual void EndTurn()
    {
        StartCoroutine(MoveRight());
        _animationHandler.PlayMoveBackward();
        turnTimer = 0;
        _animationHandler.PlayIdle();
        //Delay in Battle Manager instead?
        StartCoroutine(WaitDelay(delaySeconds));
    }

    private IEnumerator WaitDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        OnEndTurn.Invoke();
    }

    //Moves hero forward to show that it's ready to commands.
    private IEnumerator MoveLeft()
    {
        float startingXpos = transform.position.x;
        while(transform.position.x  > startingXpos - MoveOffset)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    //Moves here back to original starting position.
    private IEnumerator MoveRight()
    {
        float startingXpos = transform.position.x;
        while (transform.position.x < startingXpos + MoveOffset)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            yield return null;
        }
    }



}
