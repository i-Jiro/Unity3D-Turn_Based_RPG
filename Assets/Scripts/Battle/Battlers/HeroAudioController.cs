using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ RequireComponent(typeof(AudioSource))]
public class HeroAudioController : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] List<AudioClip> _attackVoiceClips;
    [SerializeField] List<AudioClip> _specialAttackVoiceClips;
    [SerializeField] List<AudioClip> _hurtVoiceClips;
    [SerializeField] List<AudioClip> _startTurnVoiceClips;
    [SerializeField] List<AudioClip> _evadeVoiceClips;
    [SerializeField] List<AudioClip> _guardVoiceClips;
    [SerializeField] List<AudioClip> _startGuardVoiceClips;
    [SerializeField] List<AudioClip> _selfBuffVoiceClips;
    [SerializeField] List<AudioClip> _itemUseVoiceClips;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayAttackVoice()
    {
        if (_attackVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _attackVoiceClips.Count);
        _audioSource.PlayOneShot(_attackVoiceClips[index]);
    }

    public void PlaySpecialAttackVoice()
    {
        if (_specialAttackVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _specialAttackVoiceClips.Count);
        _audioSource.PlayOneShot(_specialAttackVoiceClips[index]);
    }

    public void PlayHurtVoice()
    {
        if (_hurtVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _hurtVoiceClips.Count);
        _audioSource.PlayOneShot(_hurtVoiceClips[index]);
    }

    public void PlayStartTurnVoice()
    {
        if (_startTurnVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _startTurnVoiceClips.Count);
        _audioSource.PlayOneShot(_startTurnVoiceClips[index]);
    }

    public void PlayEvadeVoice()
    {
        if (_evadeVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _evadeVoiceClips.Count);
        _audioSource.PlayOneShot(_evadeVoiceClips[index]);
    }
    public void PlayStartGuardVoice()
    {
        if (_startTurnVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _startGuardVoiceClips.Count);
        _audioSource.PlayOneShot(_startGuardVoiceClips[index]);
    }

    public void PlayGuardVoice()
    {
        if (_guardVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _guardVoiceClips.Count);
        _audioSource.PlayOneShot(_guardVoiceClips[index]);
    }
    public void PlaySelfBuffVoice()
    {
        if (_selfBuffVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _selfBuffVoiceClips.Count);
        _audioSource.PlayOneShot(_selfBuffVoiceClips[index]);
    }

    public void PlayItemUseVoice()
    {
        if (_itemUseVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _itemUseVoiceClips.Count);
        _audioSource.PlayOneShot(_itemUseVoiceClips[index]);
    }

}
