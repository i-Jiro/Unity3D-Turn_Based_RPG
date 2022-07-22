using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum PopUpType
{
    Damage,
    Mana,
    Health
};

// Displays the damage, heal, or other battle information as a text pop up on the battler.
public class ActionInfoPopUp : MonoBehaviour
{
    private TextMeshPro _text;
    [SerializeField] private float _lifeTimeInSeconds = 1f;
    [SerializeField] private float _fadeSpeed = 0.0025f;
    [SerializeField] private Color _damageColor;
    [SerializeField] private Color _healthRecoverColor;
    [SerializeField] private Color _manaRecoverColor;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshPro>();
    }

    public void Activate(string text, PopUpType type)
    {
        switch (type)
        {
            case PopUpType.Damage:
                _text.color = _damageColor;
                break;
            case PopUpType.Health:
                _text.color = _healthRecoverColor;
                break;
            case PopUpType.Mana:
                _text.color = _manaRecoverColor;
                break;
        }
        _text.SetText(text);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        var color = _text.color;
        yield return new WaitForSeconds(_lifeTimeInSeconds);
        for (float alpha = 1f; alpha >= 0; alpha -= _fadeSpeed)
        {
            color.a = alpha;
            _text.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }
    
}
