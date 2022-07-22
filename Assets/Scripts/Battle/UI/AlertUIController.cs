using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Display message about actions taken by battler in-battle.
public class AlertUIController : MonoBehaviour
{
    [SerializeField] GameObject Alertbox;
    private TextMeshProUGUI _text;
    private void Awake()
    {
        _text = Alertbox.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        Alertbox.SetActive(false);
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (BattleManager.Instance != null)
        {
            foreach (var hero in BattleManager.Instance.heroes)
            {
                hero.DisplayAlert += DisplayMessage;
                hero.OnEndTurn += EndAlert;
            }

            foreach (var enemy in BattleManager.Instance.enemies)
            {
                enemy.DisplayAlert += DisplayMessage;
                enemy.OnEndTurn += EndAlert;
            }
        }
    }

    public void DisplayMessage(string message)
    {
        Alertbox.SetActive(true);
        _text.SetText(message);
    }

    public void EndAlert()
    {
        Alertbox.SetActive(false);
    }

    private void OnDestroy()
    {
        foreach (var hero in BattleManager.Instance.heroes)
        {
            hero.DisplayAlert -= DisplayMessage;
            hero.OnEndTurn -= EndAlert;
        }

        foreach (var enemy in BattleManager.Instance.enemies)
        {
            enemy.DisplayAlert -= DisplayMessage;
            enemy.OnEndTurn -= EndAlert;
        }
    }
}

