using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    public List<Enemy> enemies;
    public List<Hero> heroes;
    private Hero _currentHero;
    private Enemy _selectedEnemy;
    private bool _isActiveTurn = false;
    public bool isActiveTurn
    {
        get { return _isActiveTurn; }
        set { _isActiveTurn = value; }
    }

    private bool _isFightActive = false;

    public delegate void ActiveTurnEvent(bool value);
    public static ActiveTurnEvent OnActiveTurn;

    private void OnEnable()
    {
        BattleUIHandler.OnSelectEnemy += SetSelectedEnemy;
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

    private void SetSelectedEnemy(Enemy enemy)
    {
        _selectedEnemy = enemy;
        Debug.Log("Set Enemy: " + enemy.gameObject.name);
    }

    private void Start()
    {
        _isFightActive = true;
        Intialize();
    }

    private void Intialize()
    {
        foreach(Hero hero in heroes)
        {
            hero.OnTakeActiveTurn += TakeActiveTurn;
            hero.OnEndTurn += EndTurn;
        }
        foreach(Enemy enemy in enemies)
        {
            enemy.OnTakeActiveTurn += TakeActiveTurn;
            enemy.OnEndTurn += EndTurn;
        }
    }

    public void ChoseAttack(Enemy enemy)
    {
        _currentHero.Attack(enemy);
    }

    public void ChoseAbility(Ability ability)
    {
        Debug.Log(ability.name + " activated.");
       //_currentHero.UseAbility(ability);
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
        OnActiveTurn.Invoke(_isActiveTurn);
        BattleUIHandler.Instance.ToggleActionMenu(true);
    }

    public void TakeActiveTurn(Enemy enemy)
    {
         _isActiveTurn = true;
        OnActiveTurn.Invoke(_isActiveTurn);
    }

    public Hero GetCurrentHero()
    {
        return _currentHero;
    }

    public void EndTurn()
    {
        _isActiveTurn = false;
        OnActiveTurn(_isActiveTurn);
        BattleUIHandler.Instance.ToggleActionMenu(false);
    }
}
