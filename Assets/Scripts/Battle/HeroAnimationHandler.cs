using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hero))]
public class HeroAnimationHandler : MonoBehaviour
{
    private Animator _animator;
    public Animator Animator { get { return _animator; } private set { _animator = value; } }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayIdle()
    {
        _animator.SetBool("isReady", false);
    }

    public void PlayAttack()
    {
        _animator.SetTrigger("AttackTrigger");
    }

    public void PlaySpecialAttack()
    {
        _animator.SetTrigger("SpecialTrigger");
    }

    public void PlayGetDamaged()
    {
        _animator.SetTrigger("HurtTrigger");
    }

    public void PlayBuff()
    {
        _animator.SetTrigger("BuffTrigger");
    }

    public void PlayDefend()
    {
        _animator.SetBool("isDefending", true);
    }

    public void StopDefend()
    {
        _animator.SetBool("isDefending", false);
    }

    public void PlayReady()
    {
        _animator.SetBool("isReady", true);
    }

    public void PlayMoveForward()
    {
        _animator.SetTrigger("MoveForward");
    }

    public void PlayMoveBackward()
    {
        _animator.SetTrigger("MoveBackward");
    }

    public void PlayEvade()
    {
        _animator.SetTrigger("EvadeTrigger");
    }

    public void PlayWin()
    {

    }


}
 
