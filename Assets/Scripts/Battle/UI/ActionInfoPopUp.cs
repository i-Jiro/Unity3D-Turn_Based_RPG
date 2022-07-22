using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// Displays the damage, heal, or other battle information as a text pop up on the battler.
public class ActionInfoPopUp : MonoBehaviour
{
    private TextMeshPro _text;
    [SerializeField] private float _speed = 0.0025f;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshPro>();
    }

    public void Activate(string text)
    {
        _text.SetText(text);
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        var color = new Color(1, 1, 1, 1);
        for (float alpha = 1f; alpha >= 0; alpha -= _speed)
        {
            color.a = alpha;
            _text.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }
    
}
