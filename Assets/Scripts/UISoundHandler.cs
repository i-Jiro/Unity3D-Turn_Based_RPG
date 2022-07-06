using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UISoundHandler : MonoBehaviour
{
    [SerializeField] AudioClip _lowBeep;
    [SerializeField] AudioClip _highBeep;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayLowBeep()
    {
        _audioSource.PlayOneShot(_lowBeep);
    }

    public void PlayHighBeep()
    {
        _audioSource.PlayOneShot(_highBeep);
    }
}
