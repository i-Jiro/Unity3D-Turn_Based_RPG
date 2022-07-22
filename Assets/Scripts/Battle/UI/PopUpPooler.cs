using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PopUpPooler : MonoBehaviour
{
    public static PopUpPooler Instance;
    private List<ActionInfoPopUp> _pool;
    [SerializeField] int _startingPoolSize;
    [SerializeField] ActionInfoPopUp _actionInfoPrefab;
    [SerializeField] private float _offsetPositionX;
    [SerializeField] private float _offsetPositionZ;

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
        InitializePool();
        SubscribeToBattlerEvents();
    }
    
    private void InitializePool()
    {
        for (int i = 0; i < _startingPoolSize; i++)
        {
            var popUp = Instantiate(_actionInfoPrefab, transform.position, Quaternion.identity);
            _pool.Add(popUp.GetComponent<ActionInfoPopUp>());
            popUp.gameObject.SetActive(false);
        }
    }

    private void SubscribeToBattlerEvents()
    {
        foreach (var hero in BattleManager.Instance.heroes)
        {
            hero.DisplayPopUp += TriggerPopUp;
        }
        foreach (var enemy in BattleManager.Instance.enemies)
        {
            enemy.DisplayPopUp += TriggerPopUp;
        }
    }

    private void TriggerPopUp(Battler battler, string message, PopUpType type)
    {
        ActionInfoPopUp popUp = GetPooledPopUp();
        Vector3 offsetPosition = new Vector3(_offsetPositionX, 0, _offsetPositionZ);
        popUp.transform.position = battler.transform.position + offsetPosition;
        popUp.gameObject.SetActive(true);
        popUp.Activate(message, type);
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
        var popUp = Instantiate(_actionInfoPrefab, transform.position, Quaternion.identity);
        _pool.Add(popUp);
        
        return popUp;
    }

    private void OnDestroy()
    {
        foreach (var hero in BattleManager.Instance.heroes)
        {
            hero.DisplayPopUp -= TriggerPopUp;
        }
        foreach (var enemy in BattleManager.Instance.enemies)
        {
            enemy.DisplayPopUp -= TriggerPopUp;
        }
    }
}
