using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HeroAnimationController))]
[RequireComponent(typeof(HeroAudioController))]
public class Hero : Battler
{
    [SerializeField] float _moveOffset = 2f;
    [SerializeField] float _moveSpeed = 9f;
    protected float manaRegenRate = 10f;

    private StatModifier defendStanceModifier = new StatModifier(1,StatType.PhysicalDefense, StatModifierType.PercentMultiply);
    private bool isDefending = false;

    private HeroAnimationController animationController;
    private HeroAudioController audioController;
    
    protected float rawDamage;
    
    //Events for UI
    public delegate void HealthEventHandler(float health);
    public event HealthEventHandler OnHealthChanged;
    public delegate void TurnTimerEventHandler(float time);
    public event TurnTimerEventHandler OnTurnTimeChanged;
    public delegate void ManaEventHandler(float mana);
    public event ManaEventHandler OnManaChanged;

    //Events for Battle Manager
    public delegate void StartTurnEventHandler(Hero hero);
    public event StartTurnEventHandler OnStartTurn;
    public delegate void EndTurnEventHandler();
    public event EndTurnEventHandler OnEndTurn;

    //Events used by Camera Manager
    public delegate void TargetSelfEventHandler(Battler battler);
    public event TargetSelfEventHandler OnTargetSelf;

    private delegate void DealDamageCallback(float damage);
    private DealDamageCallback _dealDamageCallback;

    protected virtual void Awake()
    {
        animationController = GetComponent<HeroAnimationController>();
        audioController = GetComponent<HeroAudioController>();
    }

    protected override void TickTurnTimer()
    {
        UpdateTimeUI();
        base.TickTurnTimer();
    }

    public virtual void Attack(Enemy enemy)
    {
        OnDisplayAlert("Attack");
        rawDamage = CalculateDamage(baseDamageMultiplier);
        _dealDamageCallback = enemy.TakeDamage;
        audioController.PlayAttackVoice();
        animationController.PlayAttack();
    }

    //Called on by an animation event at the point of impact to deal damage to enemy.
    protected virtual void OnHitAnimation()
    {
        _dealDamageCallback?.Invoke(rawDamage);
    }

    //For abilities that target enemies
    public virtual void UseAbility(Enemy enemyTarget, Ability ability)
    {
        AttackAbility attackAbility = ability as AttackAbility;
        attackAbility.Trigger(this, enemyTarget, out rawDamage);
        _dealDamageCallback = enemyTarget.TakeDamage;
        UseMana(ability.ManaCost);
        OnDisplayAlert(ability.Name);
        audioController.PlaySpecialAttackVoice();
        animationController.PlaySpecialAttack();
    }

    //For abilities that target self
    public virtual void UseAbility(Ability ability)
    {
        SupportAbility buffAbility = ability as SupportAbility;
        buffAbility.Trigger(this);
        UseMana(ability.ManaCost);
        OnDisplayAlert(ability.Name);
        audioController.PlaySelfBuffVoice();
        animationController.PlayBuff();
        OnTargetSelf?.Invoke(this);
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
        OnDisplayAlert("Defend");
        audioController.PlayStartGuardVoice();
        isDefending = true;
        AddModifier(defendStanceModifier);
        animationController.PlayDefend();
        Debug.Log(gameObject.name + " defends.");
    }

    protected virtual void ResetDefence()
    {
        if (isDefending)
        {
            RemoveModifier(defendStanceModifier);
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
            OnDisplayPopUp(this, "Missed", PopUpType.Damage);
            return;
        }

        if (isDefending) { audioController.PlayGuardVoice(); }
        else { audioController.PlayHurtVoice(); }
        animationController.PlayGetDamaged();
        float damage = rawDamage - physicalDefenseStat.Value;
        if (damage < 0)
            damage = 0;
        currentHealth -= damage;
        OnDisplayPopUp(this,damage.ToString(), PopUpType.Damage);
        UpdateHealthUI();
    }

    protected virtual void RegenerateMana()
    {
        if (currentMana + manaRegenRate > maxManaStat.Value)
            currentMana = maxManaStat.Value;
        else
            currentMana += manaRegenRate;
        OnDisplayPopUp(this,manaRegenRate.ToString(), PopUpType.Mana);
        UpdateManaUI();
    }

    public override void UseItem(Item item, Battler user)
    {
        OnDisplayAlert(item.Name);
        item.Use(user);
        audioController.PlayItemUseVoice();
        animationController.PlayUseItem();
        OnTargetSelf?.Invoke(this);
    }

    public override void RecoverHealth(float amountRecovered)
    {
        base.RecoverHealth(amountRecovered);
        UpdateHealthUI();
    }

    public override void RecoverMana(float amountRecovered)
    {
        base.RecoverMana(amountRecovered);
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
        OnStartTurn?.Invoke(this);
    }

    //Called by Animation Events at the end of an animation
    protected override void EndTurn()
    {
        TickStatusEffects();
        StartCoroutine(MoveRight());
        animationController.PlayMoveBackward();
        turnTimer = 0;
        animationController.PlayIdle();
        OnEndTurn?.Invoke();
    }

    protected void UpdateManaUI()
    {
        OnManaChanged?.Invoke(currentMana);
    }

    protected void UpdateHealthUI()
    {
        OnHealthChanged?.Invoke(currentHealth);
    }
    protected void UpdateTimeUI()
    {
        OnTurnTimeChanged?.Invoke(turnTimer);
    }

    #region IEnumerators
    //Moves hero forward to show that it's ready for commands.
    private IEnumerator MoveLeft()
    {
        float startingXpos = transform.position.x;
        while(transform.position.x  > startingXpos - _moveOffset)
        {
            transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime,Space.World);
            yield return null;
        }
    }

    //Moves here back to original starting position.
    private IEnumerator MoveRight()
    {
        float startingXpos = transform.position.x;
        while (transform.position.x < startingXpos + _moveOffset)
        {
            transform.Translate(Vector3.right * _moveSpeed * Time.deltaTime, Space.World);
            yield return null;
        }
    }
    #endregion


}
