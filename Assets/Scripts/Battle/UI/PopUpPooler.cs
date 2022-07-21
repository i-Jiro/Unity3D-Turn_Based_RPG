using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpPooler : MonoBehaviour
{
    public static PopUpPooler Instance;
    private List<ActionInfoPopUp> _pool;
    [SerializeField] int _startingPoolSize;
    [SerializeField] GameObject actionInfoPrefab;

    public PopUpPooler()
    {
        _pool = new List<ActionInfoPopUp>();
    }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        InitialzePool();
        SubscribeToBattlerEvents();
    }
    
    private void InitialzePool()
    {
        for (int i = 0; i < _startingPoolSize; i++)
        {
            var popUp = Instantiate(actionInfoPrefab, transform.position, Quaternion.identity);
            _pool.Add(popUp.GetComponent<ActionInfoPopUp>());
            popUp.gameObject.SetActive(false);
        }
    }

    private void SubscribeToBattlerEvents()
    {
        foreach (var hero in BattleManager.Instance.heroes)
        {
            hero.TookDamage += TriggerPopUp;
        }
        foreach (var enemy in BattleManager.Instance.enemies)
        {
            enemy.TookDamage += TriggerPopUp;
        }
    }

    private void TriggerPopUp(Battler battler, float damage)
    {
        ActionInfoPopUp popUp = GetPooledPopUp();
        popUp.transform.position = battler.transform.position;
        popUp.gameObject.SetActive(true);
        popUp.Activate(damage.ToString());
    }

    private ActionInfoPopUp GetPooledPopUp()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].gameObject.activeInHierarchy)
            {
                return _pool[i];
            }
        }
        var popUp = Instantiate(actionInfoPrefab, transform.position, Quaternion.identity);
        _pool.Add(popUp.GetComponent<ActionInfoPopUp>());
        
        return popUp.GetComponent<ActionInfoPopUp>();
    }

    private void OnDestroy()
    {
        foreach (var hero in BattleManager.Instance.heroes)
        {
            hero.TookDamage -= TriggerPopUp;
        }
        foreach (var enemy in BattleManager.Instance.enemies)
        {
            enemy.TookDamage -= TriggerPopUp;
        }
    }
}
