using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIHandler : MonoBehaviour
{
    BattleUIHandler Instance;
    [SerializeField] Button _attackButton;
    [SerializeField] Button _defendButton;

    private void Awake()
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

    private void Start()
    {
        if(BattleManager.Instance != null)
        {
            _defendButton.onClick.AddListener(BattleManager.Instance.ChoseDefend);
        }
        else
        {
            Debug.LogError("Battle Manager Instance was not found!");
        }
    }

    private void Attack()
    {
        
    }
}
