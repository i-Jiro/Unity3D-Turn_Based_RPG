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
    [SerializeField] float _currentHealth = 100;
    public float CurrentHealth { get { return _currentHealth; } private set { CurrentHealth = value; } }
    [SerializeField] float _maxHealth = 100;
    public float MaxHealth { get { return _maxHealth; } private set { _maxHealth = value; } }
    [SerializeField] protected float currentMana = 100;
    public float CurrentMana { get { return currentMana; } private set { currentMana = value; } }
    [SerializeField] protected  float maxMana = 100;
    public float MaxMana { get { return maxMana; } private set { currentMana = value; } }
    [SerializeField] float _manaRegenRate = 10;
    [SerializeField] float _defence = 10;
    [SerializeField] float _speed = 1;
    [SerializeField] float _defenseMultiplier = 2f;

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
        enemy.TakeDamage(10); //placeholder damage
    }

    //For abilities that target enemys
    public virtual void UseAbility(Enemy enemyTarget, AbilityData ability)
    {
        _animationHandler.PlaySpecialAttack();
        AttackAbility attackAbility = ability as AttackAbility;
        attackAbility.Trigger(enemyTarget, this);
        UseMana(ability.manaCost);
        if (OnManaChanged != null)
            OnManaChanged.Invoke(currentMana);
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
        buffAbility.Trigger(this);
        UseMana(ability.manaCost);
        if (OnManaChanged != null)
            OnManaChanged.Invoke(currentMana);
    }

    //For abilities that target party members
    public virtual void UseAbility(Hero hero, AbilityData ability)
    {
        UseMana(ability.manaCost);
        if (OnManaChanged != null)
            OnManaChanged.Invoke(currentMana);
    }

    public virtual void Defend()
    {
        _isDefending = true;
        _defence *= _defenseMultiplier;
        _animationHandler.PlayDefend();
        Debug.Log(gameObject.name + " defends.");
    }

    protected virtual void ResetDefence()
    {
        if (_isDefending)
        {
            _defence /= _defenseMultiplier;
            _isDefending = false;
            _animationHandler.StopDefend();
        }
    }

    public virtual void TakeDamage(float rawDamage)
    {
        _animationHandler.PlayGetDamaged();
        float damage = rawDamage - _defence;
        if (damage < 0)
            damage = 0;
        _currentHealth -= damage;
        if (OnHealthChanged != null)
            OnHealthChanged.Invoke(_currentHealth);
    }

    protected virtual void RegenerateMana()
    {
        if (currentMana + _manaRegenRate > maxMana)
            currentMana = maxMana;
        else
            currentMana += _manaRegenRate;
        if (OnManaChanged != null)
            OnManaChanged.Invoke(currentMana);
    }

    protected virtual void UseMana(float manaUsed)
    {
        CurrentMana -= manaUsed;
    }

    //Charges character's turn meter based on it's speed.
    protected virtual void TickTurnTimer()
    {
        if (_turnTimer < _turnTimerMax)
        {
            _turnTimer += Time.deltaTime * _speed;
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

    #region Modify Stats Methods

    #endregion

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
