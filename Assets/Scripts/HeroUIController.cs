using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] Slider _timerBar;
    [SerializeField] Slider _manaBar;
    [SerializeField] Slider _healthBar;
    [SerializeField] TextMeshProUGUI _healthText;
    [SerializeField] TextMeshProUGUI _manaText;
    private float _maxHealth;
    private float _maxMana;
    private Hero _hero;

    private void OnEnable()
    {
        if (_hero != null)
            Initialize(_hero);
    }

    private void OnDisable()
    {
        if (_hero != null)
        {
            _hero.OnHealthChanged -= UpdateHealth;
            _hero.OnManaChanged -= UpdateMana;
            _hero.OnTurnTimeChanged -= UpdateTurnTimer;
        }
    }

    public void Initialize(Hero hero)
    {
        _hero = hero;
        _nameText.SetText(hero.charName);

        _maxHealth = hero.maxHealth;
        _healthBar.maxValue = _maxHealth;
        UpdateHealth(hero.health);

        _maxMana = hero.maxMana;
        _manaBar.maxValue = _maxMana;
        UpdateMana(hero.mana);

        _timerBar.maxValue = 100;
        gameObject.SetActive(true);

        hero.OnHealthChanged += UpdateHealth;
        hero.OnTurnTimeChanged += UpdateTurnTimer;
        hero.OnManaChanged += UpdateMana;
    }

    public void UpdateHealth(float health)
    {
        _healthBar.value = Mathf.Clamp(health, 0, _maxHealth);
        _healthText.SetText(health + " / " + _maxHealth);
    }

    public void UpdateMana(float mana)
    {
        _manaBar.value = Mathf.Clamp(mana, 0, _maxMana);
        _manaText.SetText(mana + " / " + _maxMana);
    }

    public void UpdateTurnTimer(float time)
    {
        _timerBar.value = Mathf.Clamp(time, 0, 100);
    }
}
