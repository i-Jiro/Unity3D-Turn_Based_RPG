using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _battleSceneCamera;
    [SerializeField] CinemachineVirtualCamera _focusHeroCamera;
    [Header("Focus Group On Targeting Settings")]
    [SerializeField] float _targetFocusWeight = 1.0f;
    [SerializeField] float _otherFocusWeight = 1.5f;
    [SerializeField] private CinemachineTargetGroup _battleSceneTargetGroup;
    [SerializeField] private CinemachineTargetGroup _focusTargetGroup;
    private bool _focusedOnGroup;
    private bool _isInitialized;
    
    private void OnEnable()
    {
        if (_isInitialized)
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
        _focusHeroCamera.gameObject.SetActive(false);
        SubscribeToEvents();
        InitializeBattleSceneCam();
        _isInitialized = true;
    }

    private void SubscribeToEvents()
    {
        foreach (var hero in BattleManager.Instance.heroes)
        {
            hero.OnTargetSelf += FocusOnHero;
            hero.OnTargetOther += FocusOnGroup;
            hero.OnEndTurn += ReturnToSceneCamera;
        }
    }
    
    private void UnsubscribeToEvents()
    {
        foreach (var hero in BattleManager.Instance.heroes)
        {
            hero.OnTargetSelf -= FocusOnHero;
            hero.OnTargetOther -= FocusOnGroup;
            hero.OnEndTurn -= ReturnToSceneCamera;
        }
    }

    private void InitializeBattleSceneCam()
    {
        if (_battleSceneTargetGroup == null) { Debug.LogWarning("BattleScene CM Target Group not assigned!"); return;}
        foreach (var hero in BattleManager.Instance.heroes)
        {
            _battleSceneTargetGroup.AddMember(hero.transform,1,0);
        }

        foreach (var enemy in BattleManager.Instance.enemies)
        {
            _battleSceneTargetGroup.AddMember(enemy.transform, 1 ,0);
        }
        _battleSceneCamera.LookAt = _battleSceneTargetGroup.Transform;
    }

    private void FocusOnHero(Battler battler)
    {
       _focusHeroCamera.gameObject.SetActive(true);
       _focusHeroCamera.LookAt = battler.transform;
    }

    private void FocusOnGroup(Battler battler, Battler other)
    {
        if (_focusTargetGroup == null) { Debug.LogWarning("Focus CM Target Group not assigned!"); return;}
        _focusTargetGroup.AddMember(battler.transform, _targetFocusWeight, 0);
        _focusTargetGroup.AddMember(other.transform, _otherFocusWeight, 0);
        _focusHeroCamera.gameObject.SetActive(true);
        _focusHeroCamera.LookAt = _focusTargetGroup.transform;
        _focusedOnGroup = true;
    }

    private void ReturnToSceneCamera()
    {
        if (_focusedOnGroup)
        {
            foreach (var target in _focusTargetGroup.m_Targets)
            {
                _focusTargetGroup.RemoveMember(target.target);
            }
        }
        _focusHeroCamera.LookAt = null;
        _focusHeroCamera.gameObject.SetActive(false);
    }
    
}
