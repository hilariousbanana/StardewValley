﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BGMManager : MonoSingleton<BGMManager>
{
    public AudioClip[] clips;
    private AudioSource source;

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void Play(int _playMusicTrack)
    {
        source.volume = 1f;
        source.clip = clips[_playMusicTrack];
        source.Play();
    }

    public void SetVolume(float _volume)
    {
        source.volume = _volume;
    }
    
    public void Pause()
    {
        source.Pause();
    }

    public void UnPause()
    {
        source.UnPause();
    }

    public void Stop()
    {
        source.Stop();
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine());
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeInCoroutine()
    {
        for(float i = 0.0f; i<=1f; i+= 0.01f)
        {
            source.volume = i;
            yield return waitTime;
        }
    }

    IEnumerator FadeOutCoroutine()
    {
        for (float i = 1.0f; i >= 0f; i -= 0.01f)
        {
            source.volume = i;
            yield return waitTime;
        }
    }
}
