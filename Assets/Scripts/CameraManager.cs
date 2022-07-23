using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _battleSceneCamera;
    [SerializeField] CinemachineVirtualCamera _focusHeroCamera;
    private bool _isInitialized = false;
    private void OnEnable()
    {
        if (_isInitialized == true)
        {
            SubscribeToEvents();
        }
    }

    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    private void Start()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        foreach (var hero in BattleManager.Instance.heroes)
        {
            hero.OnTargetSelf += FocusOnHero;
            hero.OnEndTurn += ReturnToSceneCamera;
        }
    }
    
    private void UnsubscribeToEvents()
    {
        foreach (var hero in BattleManager.Instance.heroes)
        {
            hero.OnTargetSelf -= FocusOnHero;
            hero.OnEndTurn -= ReturnToSceneCamera;
            _isInitialized = true;
        }
    }

    private void FocusOnHero(Battler battler)
    {
       _focusHeroCamera.gameObject.SetActive(true);
       _focusHeroCamera.LookAt = battler.transform;
    }

    private void ReturnToSceneCamera()
    {
        _focusHeroCamera.LookAt = null;
        _focusHeroCamera.gameObject.SetActive(false);
    }
    
}
