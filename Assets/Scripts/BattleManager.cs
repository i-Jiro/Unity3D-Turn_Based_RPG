using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public List<Enemy> enemies;
    public List<Hero> heroes;
    private Hero _currentHero;
    private bool _isActiveTurn = true;
    public bool isActiveTurn
    {
        get { return _isActiveTurn; }
        set { _isActiveTurn = value; }
    }
    private bool _isFightActive = false;


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
        Debug.Log(enemies.Count);
    }

    private void Start()
    {
        _isFightActive = true;
        _isActiveTurn = false;
    }

    public void ChoseAttack(Enemy enemy)
    {
        _currentHero.Attack(enemy);
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
        BattleUIHandler.Instance.ToggleActionMenu(true);
    }

    public void TakeActiveTurn(Enemy enemy)
    {
        // commented out until a basic enemy AI is fleshed out.
        // _isActiveTurn = true;
    }


    public void EndTurn()
    {
        _isActiveTurn = false;
        BattleUIHandler.Instance.ToggleActionMenu(false);
    }
}
