using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _battleSceneCamera;
    [SerializeField] CinemachineVirtualCamera _focusHeroCamera;
    [Header("Focus Group Targeting Settings")]
    [SerializeField] float _targetFocusWeight = 1.0f;
    [SerializeField] float _otherFocusWeight = 1.5f;
    private CinemachineTargetGroup _targetGroup;
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
        _targetGroup = GetComponent<CinemachineTargetGroup>();
        SubscribeToEvents();
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

    private void FocusOnHero(Battler battler)
    {
       _focusHeroCamera.gameObject.SetActive(true);
       _focusHeroCamera.LookAt = battler.transform;
    }

    private void FocusOnGroup(Battler battler, Battler other)
    {
        _targetGroup.AddMember(battler.transform, _targetFocusWeight, 0);
        _targetGroup.AddMember(other.transform, _otherFocusWeight, 0);
        _focusHeroCamera.gameObject.SetActive(true);
        _focusHeroCamera.LookAt = _targetGroup.transform;
        _focusedOnGroup = true;
    }

    private void ReturnToSceneCamera()
    {
        if (_focusedOnGroup)
        {
            foreach (var target in _targetGroup.m_Targets)
            {
                _targetGroup.RemoveMember(target.target);
            }
        }
        _focusHeroCamera.LookAt = null;
        _focusHeroCamera.gameObject.SetActive(false);
    }
    
}
