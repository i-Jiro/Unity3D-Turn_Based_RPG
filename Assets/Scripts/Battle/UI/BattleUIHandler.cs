using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Collections;
using UnityEngine.Serialization;

[RequireComponent(typeof(UISoundHandler))]
public class BattleUIHandler : MonoBehaviour
{
    public static BattleUIHandler Instance { get; private set; }

    private enum SelectorType {Attack, Ability}
    private SelectorType _selectorType;

    [SerializeField] Button _attackButton;
    [SerializeField] Button _defendButton;
    [SerializeField] Button _abilitiesButton;
    [SerializeField] Button _itemsButton;
    [SerializeField] GameObject _actionMenu;
    [SerializeField] GameObject _subMenu;
    [SerializeField] List<HeroUIController> _heroInfoControllers;
    [SerializeField] GameObject _selector;
    [SerializeField] float _selectorOffsetX = 2f;
    [SerializeField] float _selectorOffsetZ = -1f;
    [SerializeField] GameObject _subMenuButtonPrefab;
    [SerializeField] private int _startingButtonPoolSize;

    private bool _isSelectingEnemy = false;
    private bool _isSelectingAlly = false;
    private bool _isInSubMenu = false;
    private int _selectorIndex;
    private Ability _selectedAbility;
    private UISoundHandler _soundHandler;
    private readonly List<Button> _subMenuButtonPool = new List<Button>();

    public delegate void AttackSelectEnemyEventHandler(Enemy enemy);
    public static event AttackSelectEnemyEventHandler OnSelectEnemyAttack;

    public delegate void AbilitySelectEnemyEventHandler(Enemy enemy, Ability ability);
    public static event AbilitySelectEnemyEventHandler OnSelectEnemyAbility;

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
            _itemsButton.onClick.AddListener(DisplayItemsMenu);
            InitializeHeroUI();
            InitializeButtonPool(_startingButtonPoolSize);
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

    private void InitializeButtonPool(int startingSize)
    {
        for (int i = 0; i < startingSize; i++)
        {
            var button = Instantiate(_subMenuButtonPrefab, _subMenu.transform, true);
            _subMenuButtonPool.Add(button.GetComponent<Button>());
        }
    }
    
    private void Update()
    {
        MoveEnemySelector();
        //If in the sub menu, pressing escape will take you back to the action menu.
        if(_isInSubMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            _subMenu.SetActive(false);
            _actionMenu.SetActive(true);
            _isInSubMenu = false;
        }
    }

    //Display a menu showing available usable items in the party's inventory.
    private void DisplayItemsMenu()
    {
        _isInSubMenu = true;
        _subMenu.gameObject.SetActive(true);
        Hero currentHero = BattleManager.Instance.GetCurrentHero();

        ResetSubButtons();

        //Creates more buttons if there's not enough button in the pool.
        if (PartyManager.Instance.Inventory.Items.Count > _subMenuButtonPool.Count)
        {
            int difference = PartyManager.Instance.Inventory.Items.Count - _subMenuButtonPool.Count;
            for (int i = 0; i < difference; i++)
            {
                var button = Instantiate(_subMenuButtonPrefab, _subMenu.transform, true);
                _subMenuButtonPool.Add(button.GetComponent<Button>());
            }
        }
        
        for (int i = 0; i < _subMenuButtonPool.Count; i++)
        {
            //Buttons that are not assigned an item are hidden.
            if (i >= PartyManager.Instance.Inventory.Items.Count)
            {
               _subMenuButtonPool[i].gameObject.SetActive(false);
                continue;
            }

            Item item = PartyManager.Instance.Inventory.Items[i];
            string itemName = item.Name;
            string itemQuantity = "x" + item.Quantity;
            var itemTexts = _subMenuButtonPool[i].GetComponentsInChildren<TextMeshProUGUI>();
            //Set text on the button.
            foreach (TextMeshProUGUI text in itemTexts)
            {
                if(text.gameObject.CompareTag("UI_Name"))
                {
                    text.SetText(itemName);
                }
                else if (text.gameObject.CompareTag("UI_Quantity"))
                {
                    text.SetText(itemQuantity);
                }
            }
            _subMenuButtonPool[i].onClick.AddListener(delegate { BattleManager.Instance.ChoseUseItem(item, currentHero); _subMenu.gameObject.SetActive(false);ToggleActionMenu(false); });
        }
    }

    //Clears button for reuse.
    private void ResetSubButtons()
    {
        foreach(Button button in _subMenuButtonPool)
        {
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(true);
        }
    }

    //Display abilities menu of available skills for the hero
    private void DisplayAbilitiesMenu()
    {
        _isInSubMenu = true;
        _subMenu.gameObject.SetActive(true);
        Hero currentHero = BattleManager.Instance.GetCurrentHero();
        
        ResetSubButtons();
        
        //Creates more buttons if there's not enough button in the pool.
        if (PartyManager.Instance.Inventory.Items.Count > currentHero.Abilities.Count)
        {
            int difference = PartyManager.Instance.Inventory.Items.Count - currentHero.Abilities.Count;
            for (int i = 0; i < difference; i++)
            {
                var button = Instantiate(_subMenuButtonPrefab, _subMenu.transform, true);
                _subMenuButtonPool.Add(button.GetComponent<Button>());
            }
        }

        for(int i = 0; i < _subMenuButtonPool.Count; i++)
        {
            //Buttons that are not assigned an ability are hidden.
            if(i >= currentHero.Abilities.Count)
            {
                _subMenuButtonPool[i].gameObject.SetActive(false);
                continue;
            }

            Ability ability = currentHero.Abilities[i];
            string abilityName = ability.Name;
            float manaCost = ability.ManaCost;

            //Set text on the button.
            var texts = _subMenuButtonPool[i].GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in texts)
            {
                if(text.gameObject.CompareTag("UI_Name"))
                {
                    text.SetText(abilityName);
                }
                else if (text.gameObject.CompareTag("UI_Quantity"))
                {
                    text.SetText(manaCost + " MP");
                }
            }

            if (manaCost > currentHero.CurrentMana)
            {
                _subMenuButtonPool[i].interactable = false;
            }

            //Sort out abilities into buttons based on their ability script subclass type.
            if (ability.GetType() == typeof(AttackAbility))
            {
                _subMenuButtonPool[i].onClick.AddListener(delegate { StartSelectEnemy(SelectorType.Ability, ability); });
            }
            else if(ability.GetType() == typeof(SupportAbility))
            {
                _subMenuButtonPool[i].onClick.AddListener(delegate { BattleManager.Instance.ChoseAbility(ability); _subMenu.SetActive(false); ToggleActionMenu(false); });
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
        _selectorIndex = 0;
    }
    //Overload for abilities that require targeting enemies.
    private void StartSelectEnemy(SelectorType type, Ability ability)
    {
        _subMenu.gameObject.SetActive(false);
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
                _selectorIndex++;
                _soundHandler.PlayHighBeep();
                if (_selectorIndex >= enemies.Count)
                {
                    _selectorIndex = 0;
                }
                _selector.transform.position = enemies[_selectorIndex].gameObject.transform.position + new Vector3(_selectorOffsetX, 0, _selectorOffsetZ);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _selectorIndex--;
                _soundHandler.PlayLowBeep();
                if (_selectorIndex < 0)
                {
                    _selectorIndex = enemies.Count - 1;
                }
                _selector.transform.position = enemies[_selectorIndex].gameObject.transform.position + new Vector3(_selectorOffsetX, 0 , _selectorOffsetZ);
            }
            else if (Input.GetKeyDown(KeyCode.Space)) //Confirm select current enemy to attack.
            {
                _selector.gameObject.SetActive(false);
                _isSelectingEnemy = false;
                switch (_selectorType)
                {
                    case SelectorType.Attack:
                        if(OnSelectEnemyAttack != null)
                            OnSelectEnemyAttack.Invoke(enemies[_selectorIndex]);
                        break;
                    case SelectorType.Ability:
                        if (OnSelectEnemyAbility != null)
                            OnSelectEnemyAbility.Invoke(enemies[_selectorIndex], _selectedAbility);
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
