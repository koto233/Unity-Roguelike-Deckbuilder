using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioService
{
    void PlayBGM(string clipName, bool loop = true, float volume = 1f);
    void PlaySFX(string clipName, float volume = 1f);
    void StopBGM();
    void SetBGMVolume(float volume);
    void SetSFXVolume(float volume);
}