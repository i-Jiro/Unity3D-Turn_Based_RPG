using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public GameObject battleInterface;

    public List<Enemy> enemies;
    public List<Hero> heroes;
    private Hero _currentHero;
    private bool _isActiveTurn = false;
    public bool isActiveTurn
    {
        get { return _isActiveTurn; }
        set { _isActiveTurn = value; }
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

    public void ChoseAttack(Enemy enemy)
    {
        _currentHero.Attack(enemy);
    }

    public void ChoseDefend()
    {
        _currentHero.Defend();
    }

    public void TakeActiveTurn(Hero hero)
    {
        _currentHero = hero;
        _isActiveTurn = true;
        // When a turn is claimed by a character, pause everyone else's turn meter.
        battleInterface.gameObject.SetActive(true);
        // Give control to player, display UI with link to hero
    }

    public void TakeActiveTurn(Enemy enemy)
    {
        _isActiveTurn = true;
    }


    public void EndTurn()
    {
        _isActiveTurn = false;
        battleInterface.gameObject.SetActive(false);
    }
}
