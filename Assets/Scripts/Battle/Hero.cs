using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

[RequireComponent(typeof(HeroAnimationController))]
[RequireComponent(typeof(HeroAudioController))]
public class Hero : MonoBehaviour
{
    const float CHANCE_CAP = 1000f;

    [SerializeField] protected string charName = "";

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

    [SerializeField] float _baseDamageMultiplier = 1.0f;
    [SerializeField] float _moveOffset = 3f;
    [SerializeField] float _moveSpeed = 3f;

    [SerializeField] List<AbilityData> _abilities;
    public readonly ReadOnlyCollection<AbilityData> Abilities;
    private Dictionary<StatusModifierData, StatusModifier> _statusModifiers;
    private StatModifier _defendStanceModifier = new StatModifier(1, StatModifierType.PercentMultiply);
    private float _turnTimer = 0;
    private float _turnTimerMax = 100;
    private bool _isTurnTimerActive = false;
    private bool _isDefending = false;

    private HeroAnimationController _animationController;
    private HeroAudioController _audioController;

    public string Name
    {
        get { return charName; }
        set { if (value.Length > 49) { Debug.LogWarning("Hero set with a name longer than 49 characters! Name will not fit UI."); } }
    }
    public float CurrentHealth { get { return _currentHealth; } }
    public float CurrentMana { get { return _currentMana; } }
    public float MaxHealth { get { return _maxHealthStat.Value; } }
    public float MaxMana { get { return _maxManaStat.Value; } }

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

    public Hero()
    {
        _abilities = new List<AbilityData>();
        Abilities = _abilities.AsReadOnly();
    }

    private void Awake()
    {
        _animationController = GetComponent<HeroAnimationController>();
        _audioController = GetComponent<HeroAudioController>();
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
        _audioController.PlayAttackVoice();
        _animationController.PlayAttack();
        Debug.Log(gameObject.name + " attacked " + enemy.gameObject.name);
        enemy.TakeDamage(CalculateDamage(_baseDamageMultiplier));
    }

    private float CalculateDamage(float damageMultiplier)
    {
        float finalDamage = _physicalAttackStat.Value * damageMultiplier;
        float critChance = _criticalStat.Value;
        float randValue = Random.value;
        if (critChance > CHANCE_CAP) { critChance = CHANCE_CAP; }
        critChance /= CHANCE_CAP;
        if(randValue < 1 - critChance)
        {
            float critMultiplier = 1.25f;
            finalDamage *= critMultiplier;
            Debug.Log("Critical hit.");
        }
        return Mathf.Round(Random.Range(finalDamage, (finalDamage * 1.01f)));
    }

    //For abilities that target enemys
    public virtual void UseAbility(Enemy enemyTarget, AbilityData ability)
    {
        _audioController.PlaySpecialAttackVoice();
        _animationController.PlaySpecialAttack();
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
        _animationController.PlayBuff();
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
        _animationController.PlayDefend();
        Debug.Log(gameObject.name + " defends.");
    }

    protected virtual void ResetDefence()
    {
        if (_isDefending)
        {
            _physicalDefenseStat.RemoveModifier(_defendStanceModifier);
            _isDefending = false;
            _animationController.StopDefend();
        }
    }

    public virtual void TakeDamage(float rawDamage)
    {
        if (Evade() && !_isDefending)
        {
            _audioController.PlayEvadeVoice();
            _animationController.PlayEvade();
            Debug.Log(gameObject.name + " Evaded.");
            return;
        }

        if (_isDefending) { _audioController.PlayGuardVoice(); }
        else { _audioController.PlayHurtVoice(); }
        _animationController.PlayGetDamaged();
        float damage = rawDamage - _physicalDefenseStat.Value;
        if (damage < 0)
            damage = 0;
        _currentHealth -= damage;
        if (OnHealthChanged != null)
            OnHealthChanged.Invoke(_currentHealth);
    }

    private bool Evade()
    {
        bool didEvade = false;
        float evasionChance = _evasionStat.Value;
        float randValue = Random.value;
        if(evasionChance > CHANCE_CAP) { evasionChance = CHANCE_CAP; }
        evasionChance /= CHANCE_CAP;
        if(randValue < evasionChance) { didEvade = true; }
        return didEvade;
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
        _audioController.PlayStartTurnVoice();
        _animationController.PlayMoveForward();
        _animationController.PlayReady();
        RegenerateMana();
        ResetDefence();
        OnStartTurn.Invoke(this);
    }

    protected virtual void EndTurn()
    {
        TickStatusModifier();
        StartCoroutine(MoveRight());
        _animationController.PlayMoveBackward();
        _turnTimer = 0;
        _animationController.PlayIdle();
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
