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
    [SerializeField] List<HeroUIController> _heroInfoControllers;
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
            InitializeHeroUI();
        }
        else
        {
            Debug.LogError("Battle Manager Instance was not found!");
        }
    }

    private void InitializeHeroUI()
    {
        // There must be a minimum amounts of hero UI controllers that to the number of heroes on scene.
        // If there is any leftover Hero UIs that don't need to be assigned, they will not be active and displayed.
        for (int i = 0; i < BattleManager.Instance.heroes.Count; i++)
        {
            Hero hero = BattleManager.Instance.heroes[i];
            _heroInfoControllers[i].Initialize(hero);
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

    //Disable action menu and enables a selectors to allow player to choose an enemy.
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
            else if (Input.GetKeyDown(KeyCode.F)) //Returns player back to action menu if canceled.
            {
                ToggleActionMenu(true);
                _selector.gameObject.SetActive(false);
                _isSelectingEnemy = false;
            }
        }
    }
}
