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
        set { if(value.Length > 49) {Debug.LogWarning("Hero set with a name longer than 49 characters! Name will not fit UI.");}}
     }

    public float CurrentHealth { get { return _currentHealth; } }
    public float CurrentMana { get { return _currentMana; } }
    public float MaxHealth { get { return _maxHealthStat.Value; } }
    public float MaxMana { get { return _maxManaStat.Value; } }

    [SerializeField] float _currentHealth = 100;
    [SerializeField] protected float _currentMana = 100;
    [SerializeField] float _manaRegenRate = 10;

    [SerializeField] CharacterStat _maxHealthStat;
    [SerializeField] CharacterStat _maxManaStat;
    [SerializeField] CharacterStat _physicalAttackStat;
    [SerializeField] CharacterStat _physicalDefenseStat;
    [SerializeField] CharacterStat _speedStat;
    [SerializeField] CharacterStat _criticalStat;
    [SerializeField] CharacterStat _evasionStat;

    private StatModifier _defendStanceModifier = new StatModifier(1, StatModifierType.PercentMultiply);

    [SerializeField] float _moveOffset = 3f;
    [SerializeField] float _moveSpeed = 3f;

    public List<AbilityData> abilities;
    private Dictionary<StatusModifierData, StatusModifier> _statusModifiers;

    private float _turnTimer = 0;
    private float _turnTimerMax = 100;
    private bool _isTurnTimerActive = false;
    private bool _isDefending = false;

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
        _currentHealth = _maxHealthStat.Value;
        _currentMana = _maxManaStat.Value;
        _statusModifiers = new Dictionary<StatusModifierData, StatusModifier>();
        _isTurnTimerActive = true;
        if (FindObjectOfType<BattleManager>())
        {
            BattleManager.OnActiveTurnChanged += ToggleTurnTimer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_isTurnTimerActive)
            TickTurnTimer();
    }
    public void ToggleTurnTimer(bool value)
    {
        _isTurnTimerActive = !value;
    }
    public virtual void Attack(Enemy enemy)
    {
        _animationHandler.PlayAttack();
        Debug.Log(gameObject.name + " attacked " + enemy.gameObject.name);
        enemy.TakeDamage(_physicalAttackStat.Value); //placeholder damage
    }

    //For abilities that target enemys
    public virtual void UseAbility(Enemy enemyTarget, AbilityData ability)
    {
        _animationHandler.PlaySpecialAttack();
        AttackAbility attackAbility = ability as AttackAbility;
        attackAbility.Trigger(enemyTarget, this);
        UseMana(ability.manaCost);
        if (OnManaChanged != null)
            OnManaChanged.Invoke(_currentMana);
    }

    //For abilities that target self
    public virtual void UseAbility(AbilityData ability)
    {
        Debug.Log(charName + " used " + ability.name);
        _animationHandler.PlayBuff();
        SupportAbility buffAbility = ability as SupportAbility;
        if (ability.targetParticlePrefb != null)
        {
            Instantiate(ability.targetParticlePrefb, transform.position, ability.targetParticlePrefb.transform.rotation);
        }
        StatModifier speedModifier = new StatModifier(1, StatModifierType.PercentAdd);
        _speedStat.AddModifier(speedModifier);
        buffAbility.Trigger(this);
        UseMana(ability.manaCost);
        if (OnManaChanged != null)
            OnManaChanged.Invoke(_currentMana);
    }

    //For abilities that target party members
    public virtual void UseAbility(Hero hero, AbilityData ability)
    {
        UseMana(ability.manaCost);
        if (OnManaChanged != null)
            OnManaChanged.Invoke(_currentMana);
    }

    public virtual void Defend()
    {
        _isDefending = true;
        _physicalDefenseStat.AddModifier(_defendStanceModifier);
        _animationHandler.PlayDefend();
        Debug.Log(gameObject.name + " defends.");
    }

    protected virtual void ResetDefence()
    {
        if (_isDefending)
        {
            _physicalDefenseStat.RemoveModifier(_defendStanceModifier);
            _isDefending = false;
            _animationHandler.StopDefend();
        }
    }

    public virtual void TakeDamage(float rawDamage)
    {
        _animationHandler.PlayGetDamaged();
        float damage = rawDamage - _physicalDefenseStat.Value;
        if (damage < 0)
            damage = 0;
        _currentHealth -= damage;
        if (OnHealthChanged != null)
            OnHealthChanged.Invoke(_currentHealth);
    }

    protected virtual void RegenerateMana()
    {
        if (_currentMana + _manaRegenRate > _maxManaStat.Value)
            _currentMana = _maxManaStat.Value;
        else
            _currentMana += _manaRegenRate;
        if (OnManaChanged != null)
            OnManaChanged.Invoke(_currentMana);
    }

    protected virtual void UseMana(float manaUsed)
    {
        _currentMana -= manaUsed;
    }

    //Charges character's turn meter based on it's speed.
    protected virtual void TickTurnTimer()
    {
        if (_turnTimer < _turnTimerMax)
        {
            _turnTimer += Time.deltaTime * _speedStat.Value;
            if (OnTurnTimeChanged != null)
                OnTurnTimeChanged.Invoke(_turnTimer);
        }
        else if (_turnTimer > _turnTimerMax)
        {
            StartTurn();
            Debug.Log(gameObject.name + " has reached it's turn.");
        }
    }

    private void TickStatusModifier()
    {
        foreach(StatusModifier status in _statusModifiers.Values)
        {
            status.Tick();
            if (status.isFinished)
            {
                _statusModifiers.Remove(status.modifierData);
            }
        }
    }

    public void AddModifier(StatusModifier statusMod)
    {
        if (_statusModifiers.ContainsKey(statusMod.modifierData))
        {
            _statusModifiers[statusMod.modifierData].Activate();
        }
        else
        {
            _statusModifiers.Add(statusMod.modifierData, statusMod);
            _statusModifiers[statusMod.modifierData].Activate();
        }
    }

    protected virtual void StartTurn()
    {
        StartCoroutine(MoveLeft());
        _animationHandler.PlayMoveForward();
        _animationHandler.PlayReady();
        RegenerateMana();
        ResetDefence();
        OnStartTurn.Invoke(this);
    }

    protected virtual void EndTurn()
    {
        TickStatusModifier();
        StartCoroutine(MoveRight());
        _animationHandler.PlayMoveBackward();
        _turnTimer = 0;
        _animationHandler.PlayIdle();
        OnEndTurn.Invoke();
    }

    #region IEnumerators
    //Moves hero forward to show that it's ready to commands.
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
