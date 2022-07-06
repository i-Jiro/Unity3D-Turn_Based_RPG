using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicHandler : MonoBehaviour
{
    [SerializeField] AudioSource _audioSourceIntro;
    [SerializeField] AudioSource _audioSourceLoop;
    [SerializeField] AudioClip _songIntro;
    [SerializeField] AudioClip _songLoop;

    private void Start()
    {
        _audioSourceIntro.loop = false;
        _audioSourceIntro.playOnAwake = false;

        _audioSourceLoop.loop = true;
        _audioSourceLoop.playOnAwake = false;
        _audioSourceIntro.clip = _songIntro;
        _audioSourceLoop.clip = _songLoop;

        _audioSourceIntro.Play();
        _audioSourceLoop.PlayDelayed(_audioSourceIntro.clip.length);
    }
}
