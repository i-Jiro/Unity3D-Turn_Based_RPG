using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HeroAnimationController))]
[RequireComponent(typeof(HeroAudioController))]
public class Hero : Battler
{
    [SerializeField] float _moveOffset = 2f;
    [SerializeField] float _moveSpeed = 9f;
    protected float manaRegenRate = 10f;

    private StatModifier defendStanceModifier = new StatModifier(1, StatModifierType.PercentMultiply);
    private bool isDefending = false;

    private HeroAnimationController animationController;
    private HeroAudioController audioController;

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

    protected virtual void Awake()
    {
        animationController = GetComponent<HeroAnimationController>();
        audioController = GetComponent<HeroAudioController>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void TickTurnTimer()
    {
        OnTurnTimeChanged.Invoke(turnTimer);
        base.TickTurnTimer();
    }

    public virtual void Attack(Enemy enemy)
    {
        audioController.PlayAttackVoice();
        animationController.PlayAttack();
        Debug.Log(gameObject.name + " attacked " + enemy.gameObject.name);
        enemy.TakeDamage(CalculateDamage(baseDamageMultiplier));
    }

    //For abilities that target enemys
    public virtual void UseAbility(Enemy enemyTarget, Ability ability)
    {
        audioController.PlaySpecialAttackVoice();
        animationController.PlaySpecialAttack();
        AttackAbility attackAbility = ability as AttackAbility;
        attackAbility.Trigger(this, enemyTarget);
        UseMana(ability.ManaCost);
    }

    //For abilities that target self
    public virtual void UseAbility(Ability ability)
    {
        Debug.Log(charName + " used " + ability.Name);
        audioController.PlaySelfBuffVoice();
        animationController.PlayBuff();
        SupportAbility buffAbility = ability as SupportAbility;
        buffAbility.Trigger(this);
        UseMana(ability.ManaCost);
    }

    //For abilities that target party members
    public virtual void UseAbility(Hero hero, Ability ability)
    {
        UseMana(ability.ManaCost);
    }

    protected override void UseMana(float manaUsed)
    {
        base.UseMana(manaUsed);
        UpdateManaUI();
    }

    public virtual void Defend()
    {
        audioController.PlayStartGuardVoice();
        isDefending = true;
        physicalDefenseStat.AddModifier(defendStanceModifier);
        animationController.PlayDefend();
        Debug.Log(gameObject.name + " defends.");
    }

    protected virtual void ResetDefence()
    {
        if (isDefending)
        {
            physicalDefenseStat.RemoveModifier(defendStanceModifier);
            isDefending = false;
            animationController.StopDefend();
        }
    }

    public override void TakeDamage(float rawDamage)
    {
        if (Evade() && !isDefending)
        {
            audioController.PlayEvadeVoice();
            animationController.PlayEvade();
            Debug.Log(gameObject.name + " Evaded.");
            return;
        }

        if (isDefending) { audioController.PlayGuardVoice(); }
        else { audioController.PlayHurtVoice(); }
        animationController.PlayGetDamaged();
        float damage = rawDamage - physicalDefenseStat.Value;
        if (damage < 0)
            damage = 0;
        currentHealth -= damage;
        UpdateHealthUI();
    }

    protected virtual void RegenerateMana()
    {
        if (currentMana + manaRegenRate > maxManaStat.Value)
            currentMana = maxManaStat.Value;
        else
            currentMana += manaRegenRate;
        UpdateManaUI();
    }

    protected override void StartTurn()
    {
        StartCoroutine(MoveLeft());
        audioController.PlayStartTurnVoice();
        animationController.PlayMoveForward();
        animationController.PlayReady();
        RegenerateMana();
        ResetDefence();
        OnStartTurn.Invoke(this);
    }

    protected override void EndTurn()
    {
        TickStatusEffects();
        StartCoroutine(MoveRight());
        animationController.PlayMoveBackward();
        turnTimer = 0;
        animationController.PlayIdle();
        OnEndTurn.Invoke();
    }

    protected void UpdateManaUI()
    {
        if (OnManaChanged != null)
            OnManaChanged.Invoke(currentMana);
    }

    protected void UpdateHealthUI()
    {
        if (OnHealthChanged != null)
            OnHealthChanged.Invoke(currentHealth);
    }

    #region IEnumerators
    //Moves hero forward to show that it's ready for commands.
    private IEnumerator MoveLeft()
    {
        float startingXpos = transform.position.x;
        while(transform.position.x  > startingXpos - _moveOffset)
        {
            transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    //Moves here back to original starting position.
    private IEnumerator MoveRight()
    {
        float startingXpos = transform.position.x;
        while (transform.position.x < startingXpos + _moveOffset)
        {
            transform.Translate(Vector3.right * _moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
    #endregion


}
