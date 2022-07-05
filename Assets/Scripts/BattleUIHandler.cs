using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIHandler : MonoBehaviour
{
    public static BattleUIHandler Instance { get; private set; }

    enum HeroChoiceState {Attack, Ability, Idle}

    [SerializeField] Button _attackButton;
    [SerializeField] Button _defendButton;
    [SerializeField] Button _abilitiesButton;
    [SerializeField] GameObject _actionMenu;
    [SerializeField] GameObject _abilitiesMenu;
    [SerializeField] List<Button> _abilityButtons;
    [SerializeField] List<HeroUIController> _heroInfoControllers;
    [SerializeField] GameObject _selector;
    [SerializeField] float _selectorOffset = 2f;

    private HeroChoiceState _currentState;
    private bool _isSelectingEnemy = false;
    private bool _isSelectingAlly = false;
    private int _index;
    private Ability _selectedAbility;

    public delegate void AttackSelectEnemyEvent(Enemy enemy);
    public static event AttackSelectEnemyEvent OnSelectEnemyAttack;

    public delegate void AbilitySelectEnemyEvent(Enemy enemy, Ability ability);
    public static event AbilitySelectEnemyEvent OnSelectEnemyAbility;

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
            _attackButton.onClick.AddListener(delegate { StartSelectEnemy(HeroChoiceState.Attack); });
            _defendButton.onClick.AddListener(BattleManager.Instance.ChoseDefend);
            _abilitiesButton.onClick.AddListener(DisplayAbilitiesMenu);
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
    private void DisplayAbilitiesMenu()
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
            float manaCost = ability.manaCost;
            _abilityButtons[i].GetComponentInChildren<TextMeshProUGUI>().SetText(abilityName);

            if (manaCost > currentHero.mana)
            {
                _abilityButtons[i].interactable = false;
            }

            if (ability.GetType() == typeof(AttackAbility))
            {
                _abilityButtons[i].onClick.AddListener(delegate { StartSelectEnemy(HeroChoiceState.Ability, ability); });
                Debug.Log(abilityName + "is an Attack Ability.");
            }
            else if(ability.GetType() == typeof(BuffAbility))
            {
                _abilityButtons[i].onClick.AddListener(delegate { BattleManager.Instance.ChoseAbility(ability); _abilitiesMenu.SetActive(false); });
                Debug.Log(abilityName + " is a Buff Ability.");
            }
            else
            {
                Debug.LogError("Unable to set an ability to a skill button.");
            }
        }
    }

    public void ToggleActionMenu(bool value)
    {
        _actionMenu.gameObject.SetActive(value);
    }

    //Disable action menu and enables a selector to allow player to choose an enemy.
    private void StartSelectEnemy(HeroChoiceState state)
    {
        _currentState = state;
        ToggleActionMenu(false);
        _selector.gameObject.SetActive(true);
        _isSelectingEnemy = true;
        _selector.transform.position = BattleManager.Instance.enemies[0].gameObject.transform.position + new Vector3(0, _selectorOffset, 0);
        _index = 0;
    }
    //Overload for abilities that require targeting enemies.
    private void StartSelectEnemy(HeroChoiceState state, Ability ability)
    {
        _abilitiesMenu.gameObject.SetActive(false);
        _selectedAbility = ability;
        StartSelectEnemy(state);
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
                switch (_currentState)
                {
                    case HeroChoiceState.Attack:
                        if(OnSelectEnemyAttack != null)
                            OnSelectEnemyAttack.Invoke(enemies[_index]);
                        break;
                    case HeroChoiceState.Ability:
                        if (OnSelectEnemyAbility != null)
                            OnSelectEnemyAbility.Invoke(enemies[_index], _selectedAbility);
                        break;
                    default:
                        Debug.LogError("In unknown state in Battle UI Handler!");
                        break;
                }
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
