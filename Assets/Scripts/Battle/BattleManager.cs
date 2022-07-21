using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public List<Enemy> enemies;
    public List<Hero> heroes;
    public List<Transform> heroesStartingPositions;
    private Hero _currentHero;
    private bool _isActiveTurn = false;

    private bool _isFightActive = false;
    [SerializeField] float _turnDelaySeconds = 0.75f;

    public delegate void ActiveTurnEvent(bool value);
    public static ActiveTurnEvent OnActiveTurnChanged;

    public static BattleManager Instance { get; private set; }

    private void OnEnable()
    {
        BattleUIHandler.OnSelectEnemyAttack += ChoseAttack;
        BattleUIHandler.OnSelectEnemyAbility += ChoseAbility;
    }

    private void Awake()
    {
        //Singleton
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _isFightActive = true;
        Intialize();
    }

    //Initialize listeners for events from heroes and enemies that are deployed at start.
    private void Intialize()
    {
        foreach(Hero hero in heroes)
        {
            hero.OnStartTurn += TakeActiveTurn;
            hero.OnEndTurn += EndTurn;
        }
        foreach(Enemy enemy in enemies)
        {
            enemy.OnStartTurn += StartTurn;
            enemy.OnEndTurn += EndTurn;
        }
    }

    public void ChoseAttack(Enemy enemy)
    {
        _currentHero.Attack(enemy);
    }

    public void ChoseAbility(Ability ability)
    {
        _currentHero.UseAbility(ability);
    }

    public void ChoseAbility(Enemy enemy, Ability ability)
    {
        _currentHero.UseAbility(enemy, ability);
    }

    public void ChoseUseItem(Item item, Battler target)
    {
        _currentHero.UseItem(item, target);
    }

    public void ChoseDefend()
    {
        _currentHero.Defend();
    }

    // When a turn is claimed by a character, pause everyone else's turn meter.
    public void TakeActiveTurn(Hero hero)
    {
        _currentHero = hero;
        _isActiveTurn = true;
        OnActiveTurnChanged.Invoke(_isActiveTurn);
        BattleUIHandler.Instance.ToggleActionMenu(true);
    }

    public void StartTurn(Enemy enemy)
    {
         _isActiveTurn = true;
        OnActiveTurnChanged.Invoke(_isActiveTurn);
    }

    public Hero GetCurrentHero()
    {
        return _currentHero;
    }

    public void EndTurn()
    {
        _isActiveTurn = false;
        //Micro delay to allow for any remaining animations to finish.
        StartCoroutine(EndTurnDelay(_turnDelaySeconds));
        BattleUIHandler.Instance.ToggleActionMenu(false);
    }

    private IEnumerator EndTurnDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        OnActiveTurnChanged.Invoke(_isActiveTurn);
    }
}
