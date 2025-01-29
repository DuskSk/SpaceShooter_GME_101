using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip _explosionAudioClip;
    private AudioSource _audioSource; 
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _explosionAudioClip;
    }

    public void PlayExplosionAudio()
    {
        _audioSource.Play();
    }
}
