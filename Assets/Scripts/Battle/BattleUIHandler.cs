using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(UISoundHandler))]
public class BattleUIHandler : MonoBehaviour
{
    public static BattleUIHandler Instance { get; private set; }

    enum SelectorType {Attack, Ability}
    private SelectorType _selectorType;

    [SerializeField] Button _attackButton;
    [SerializeField] Button _defendButton;
    [SerializeField] Button _abilitiesButton;
    [SerializeField] GameObject _actionMenu;
    [SerializeField] GameObject _abilitiesMenu;
    [SerializeField] List<Button> _abilityButtons;
    [SerializeField] List<HeroUIController> _heroInfoControllers;
    [SerializeField] GameObject _selector;
    [SerializeField] float _selectorOffsetX = 2f;
    [SerializeField] float _selectorOffsetZ = -1f;

    private bool _isSelectingEnemy = false;
    private bool _isSelectingAlly = false;
    private bool _isInAbilityMenu = false;
    private int _index;
    private Ability _selectedAbility;
    private UISoundHandler _soundHandler;

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
        _soundHandler = GetComponent<UISoundHandler>();
    }

    private void Start()
    {
        if(BattleManager.Instance != null)
        {
            _attackButton.onClick.AddListener(delegate { StartSelectEnemy(SelectorType.Attack); });
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
        //If in the abilities menu, pressing escape will take you back to the action menu.
        if(_isInAbilityMenu == true && Input.GetKeyDown(KeyCode.Escape))
        {
            _abilitiesMenu.SetActive(false);
            _actionMenu.SetActive(true);
            _isInAbilityMenu = false;
        }
    }

    //Display abilities menu of availiable skills for the hero
    private void DisplayAbilitiesMenu()
    {
        _isInAbilityMenu = true;
        _actionMenu.gameObject.SetActive(false);
        _abilitiesMenu.gameObject.SetActive(true);
        Hero currentHero = BattleManager.Instance.GetCurrentHero();

        foreach(Button button in _abilityButtons)
        {
           button.onClick.RemoveAllListeners();
        }

        for(int i = 0; i < _abilityButtons.Count; i++)
        {
            //Buttons that are not assigned an ability are hidden.
            if(i >= currentHero.Abilities.Count)
            {
                _abilityButtons[i].gameObject.SetActive(false);
                continue;
            }

            Ability ability = currentHero.Abilities[i];
            string abilityName = ability.Name;
            float manaCost = ability.ManaCost;
            _abilityButtons[i].GetComponentInChildren<TextMeshProUGUI>().SetText(abilityName);

            if (manaCost > currentHero.CurrentMana)
            {
                _abilityButtons[i].interactable = false;
            }

            //Sort out abilities into buttons based on their ability script subclass type.
            if (ability.GetType() == typeof(AttackAbility))
            {
                _abilityButtons[i].onClick.AddListener(delegate { StartSelectEnemy(SelectorType.Ability, ability); });
                Debug.Log(abilityName + "is an Attack Ability.");
            }
            else if(ability.GetType() == typeof(SupportAbility))
            {
                _abilityButtons[i].onClick.AddListener(delegate { BattleManager.Instance.ChoseAbility(ability); _abilitiesMenu.SetActive(false); });
                Debug.Log(abilityName + " is a Support Ability.");
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
    private void StartSelectEnemy(SelectorType type)
    {
        _selectorType = type;
        ToggleActionMenu(false);
        _selector.gameObject.SetActive(true);
        _isSelectingEnemy = true;
        _selector.transform.position = BattleManager.Instance.enemies[0].gameObject.transform.position + new Vector3(_selectorOffsetX, 0, _selectorOffsetZ);
        _index = 0;
    }
    //Overload for abilities that require targeting enemies.
    private void StartSelectEnemy(SelectorType type, Ability ability)
    {
        _abilitiesMenu.gameObject.SetActive(false);
        _selectedAbility = ability;
        StartSelectEnemy(type);
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
                _soundHandler.PlayHighBeep();
                if (_index >= enemies.Count)
                {
                    _index = 0;
                }
                _selector.transform.position = enemies[_index].gameObject.transform.position + new Vector3(_selectorOffsetX, 0, _selectorOffsetZ);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _index--;
                _soundHandler.PlayLowBeep();
                if (_index < 0)
                {
                    _index = enemies.Count - 1;
                }
                _selector.transform.position = enemies[_index].gameObject.transform.position + new Vector3(_selectorOffsetX, 0 , _selectorOffsetZ);
            }
            else if (Input.GetKeyDown(KeyCode.Space)) //Confirm select current enemy to attack.
            {
                _selector.gameObject.SetActive(false);
                _isSelectingEnemy = false;
                switch (_selectorType)
                {
                    case SelectorType.Attack:
                        if(OnSelectEnemyAttack != null)
                            OnSelectEnemyAttack.Invoke(enemies[_index]);
                        break;
                    case SelectorType.Ability:
                        if (OnSelectEnemyAbility != null)
                            OnSelectEnemyAbility.Invoke(enemies[_index], _selectedAbility);
                        break;
                    default:
                        Debug.LogError("In unknown selector in Battle UI Handler!");
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape)) //Returns player back to action menu if canceled.
            {
                ToggleActionMenu(true);
                _selector.gameObject.SetActive(false);
                _isSelectingEnemy = false;
            }
        }
    }
}
