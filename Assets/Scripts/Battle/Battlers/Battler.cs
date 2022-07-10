using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Linq;

public abstract class Battler : MonoBehaviour
{
    protected const float CHANCE_CAP = 1000f;
    protected const float TURN_TIMER_MAX = 100;

    [SerializeField] protected string charName = "";
    [SerializeField] protected float currentHealth = 100;
    [SerializeField] protected float currentMana = 100;

    [SerializeField] protected CharacterStat maxHealthStat;
    [SerializeField] protected CharacterStat maxManaStat;
    [SerializeField] protected CharacterStat physicalAttackStat;
    [SerializeField] protected CharacterStat physicalDefenseStat;
    [SerializeField] protected CharacterStat speedStat;
    [SerializeField] protected CharacterStat criticalStat;
    [SerializeField] protected CharacterStat evasionStat;

    [SerializeField] protected float baseDamageMultiplier = 1.0f;

    [SerializeField]protected List<AbilityData> abilitiesData;
    protected readonly List<Ability> abilities;
    public readonly ReadOnlyCollection<Ability> Abilities;
    protected Dictionary<StatusEffectData, StatusEffect> statusEffects;

    protected float turnTimer = 0;
    protected bool isTurnTimerActive = false;

    public string Name
    {
        get { return charName; }
        set { if (value.Length > 49) { Debug.LogWarning("Battler set with a name longer than 49 characters! Name will not fit UI."); } }
    }

    public float CurrentHealth { get { return currentHealth; } }
    public float CurrentMana { get { return currentMana; } }
    public float MaxHealth { get { return maxHealthStat.Value; } }
    public float MaxMana { get { return maxManaStat.Value; } }

    public Battler()
    {
        abilitiesData = new List<AbilityData>();
        abilities = new List<Ability>();
        statusEffects = new Dictionary<StatusEffectData, StatusEffect>();
        Abilities = abilities.AsReadOnly();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealthStat.Value;
        currentMana = maxManaStat.Value;
        isTurnTimerActive = true;
        if (FindObjectOfType<BattleManager>())
        {
            BattleManager.OnActiveTurnChanged += ToggleTurnTimer;
        }
        else { Debug.LogError(gameObject.name + ": Can't find Battle Manager on scnene!"); }
        InitializeAbilities();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isTurnTimerActive)
            TickTurnTimer();
    }

    protected void InitializeAbilities()
    {
        foreach (AbilityData data in abilitiesData)
        {
            Ability ability = data.Initialize(this.gameObject);
            abilities.Add(ability);
        }
    }

    protected abstract void StartTurn();
    protected abstract void EndTurn();

    public virtual float CalculateDamage(float damageMultiplier)
    {
        float finalDamage = physicalAttackStat.Value * damageMultiplier;
        float critChance = criticalStat.Value;
        float randValue = Random.value;
        if (critChance > CHANCE_CAP) { critChance = CHANCE_CAP; }
        critChance /= CHANCE_CAP;
        if (randValue < 1 - critChance)
        {
            float critMultiplier = 1.25f;
            finalDamage *= critMultiplier;
        }
        return Mathf.Round(Random.Range(finalDamage, (finalDamage * 1.01f)));
    }

    protected virtual void TickTurnTimer()
    {
        if (turnTimer < TURN_TIMER_MAX)
        {
            turnTimer += Time.deltaTime * speedStat.Value;
        }
        else if (turnTimer > TURN_TIMER_MAX)
        {
            Debug.Log(gameObject.name + " has reached it's turn.");
            StartTurn();
        }
    }

    public virtual void ToggleTurnTimer(bool value)
    {
        isTurnTimerActive = !value;
    }

    public virtual void TakeDamage(float rawDamage)
    {
        if (Evade())
        {
            Debug.Log(gameObject.name + " evaded.");
            return;
        }

        float damage = rawDamage - physicalDefenseStat.Value;
        if (damage < 0)
            damage = 0;
        currentHealth -= damage;
    }

    protected virtual void UseMana(float manaUsed)
    {
        currentMana -= manaUsed;
    }

    protected virtual bool Evade()
    {
        bool didEvade = false;
        float evasionChance = evasionStat.Value;
        float randValue = Random.value;
        if (evasionChance > CHANCE_CAP) { evasionChance = CHANCE_CAP; }
        evasionChance /= CHANCE_CAP;
        if (randValue < evasionChance) { didEvade = true; }
        return didEvade;
    }

    protected void TickStatusEffects()
    {
        foreach (StatusEffect status in statusEffects.Values.ToList()) //creates copy into a list to iterate with. Avoids error if iterating and operating in oringal dict.
        {
            status.Tick();
            if (status.isFinished)
            {
                statusEffects.Remove(status.Data);
            }
        }
    }

    public void AddStatusEffect(StatusEffect statusEffect)
    {
        Debug.Log("Status effect added: " + statusEffect.Data.Name + " on " + this.Name);
        if (statusEffects.ContainsKey(statusEffect.Data))
        {
            statusEffects[statusEffect.Data].Start();
        }
        else
        {
            statusEffects.Add(statusEffect.Data, statusEffect);
            statusEffects[statusEffect.Data].Start();
        }
    }

    public void AddModifier(StatModifier statMod)
    {
        speedStat.AddModifier(statMod);
    }

    public void RemoveModifier(StatModifier statMod)
    {
        speedStat.RemoveModifier(statMod);
    }

    public void RemoveAllModifierFromSource(object source)
    {
        speedStat.RemoveAllModifierFromSource(source);
    }

}
