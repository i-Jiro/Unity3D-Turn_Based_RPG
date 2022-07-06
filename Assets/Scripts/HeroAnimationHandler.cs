using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hero))]
public class HeroAnimationHandler : MonoBehaviour
{
    private Animator _animator;

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

    public void PlayGetDamaged()
    {

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

    public void PlayMove()
    {

    }

    public void PlayWin()
    {

    }


}
 
