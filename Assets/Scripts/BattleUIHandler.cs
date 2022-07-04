using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIHandler : MonoBehaviour
{
    public static BattleUIHandler Instance { get; private set; }

    [SerializeField] Button _attackButton;
    [SerializeField] Button _defendButton;
    [SerializeField] Button _abilitiesButton;
    [SerializeField] GameObject _actionMenu;
    [SerializeField] GameObject _abilitiesMenu;
    [SerializeField] List<Button> _abilityButtons;
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
            _abilitiesButton.onClick.AddListener(OnClickedAbility);
            InitializeHeroUI();
        }
        else
        {
            Debug.LogError("Battle Manager Instance was not found!");
        }
    }

    private void InitializeHeroUI()
    {
   
        // If there is any leftover Hero UIs that don't need to be assigned, they will not be active and displayed.
        for (int i = 0; i < _heroInfoControllers.Count; i++)
        {
            if(i >= BattleManager.Instance.heroes.Count)
            {
                _heroInfoControllers[i].gameObject.SetActive(false);
                continue;
            }
            Hero hero = BattleManager.Instance.heroes[i];
            _heroInfoControllers[i].Initialize(hero);
        }
    }

    private void Update()
    {
        MoveEnemySelector();
    }

    //Display abilities menu of availiable skills for the hero
    public void OnClickedAbility()
    {
        _actionMenu.gameObject.SetActive(false);
        _abilitiesMenu.gameObject.SetActive(true);
        Hero currentHero = BattleManager.Instance.GetCurrentHero();
        for(int i = 0; i < _abilityButtons.Count; i++)
        {
            //Buttons that are not assigned an ability are hidden.
            if(i >= currentHero.abilities.Count)
            {
                _abilityButtons[i].gameObject.SetActive(false);
                continue;
            }
            Ability ability = currentHero.abilities[i];
            string abilityName = ability.AbilityName;
            _abilityButtons[i].GetComponentInChildren<TextMeshProUGUI>().SetText(abilityName);
            _abilityButtons[i].onClick.AddListener(delegate { BattleManager.Instance.ChoseAbility(ability); });
        }
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
                if (_index >= enemies.Count)
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
                _selector.gameObject.SetActive(false);
                _isSelectingEnemy = false;
                BattleManager.Instance.ChoseAttack(enemies[_index]);
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
