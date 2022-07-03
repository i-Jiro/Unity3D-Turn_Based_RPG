using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIHandler : MonoBehaviour
{
    public static BattleUIHandler Instance;
    [SerializeField] Button _attackButton;
    [SerializeField] Button _defendButton;
    [SerializeField] GameObject _actionMenu;
    [SerializeField] GameObject _selector;
    [SerializeField] float _selectorOffset = 2f;
    private bool _isSelectingEnemy = false;
    private int _index;

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
            _attackButton.onClick.AddListener(StartSelectEnemy);
            _defendButton.onClick.AddListener(BattleManager.Instance.ChoseDefend);
        }
        else
        {
            Debug.LogError("Battle Manager Instance was not found!");
        }
    }

    private void Update()
    {
        MoveEnemySelector();
    }

    public void ToggleActionMenu(bool value)
    {
        _actionMenu.gameObject.SetActive(value);
    }

    private void StartSelectEnemy()
    {
        ToggleActionMenu(false);
        _selector.gameObject.SetActive(true);
        _isSelectingEnemy = true;
        _selector.transform.position = BattleManager.Instance.enemies[0].gameObject.transform.position + new Vector3(0, _selectorOffset, 0);
        _index = 0;
    }

    private void MoveEnemySelector()
    {
        // Moves selector between enemies; left or right.
        if (_isSelectingEnemy)
        {
            List<Enemy> enemies = BattleManager.Instance.enemies;
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _index++;
                if (_index > enemies.Count - 1)
                {
                    _index = 0;
                }
                _selector.transform.position = enemies[_index].gameObject.transform.position + new Vector3(0, _selectorOffset, 0);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _index--;
                if (_index < 0)
                {
                    _index = enemies.Count - 1;
                }
                _selector.transform.position = enemies[_index].gameObject.transform.position + new Vector3(0, _selectorOffset, 0);
            }
            else if (Input.GetKeyDown(KeyCode.Space)) //Confirm select current enemy to attack.
            {
                BattleManager.Instance.ChoseAttack(enemies[_index]);
                _selector.gameObject.SetActive(false);
                _isSelectingEnemy = false;
            }
            else if (Input.GetKeyDown(KeyCode.F)) //Returns player back to action menu if cancels.
            {
                ToggleActionMenu(true);
                _selector.gameObject.SetActive(false);
                _isSelectingEnemy = false;
            }
        }
    }
}
