using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This LocalSoundManager is attachted to entities like the player etc
/// The GlobalSoundManager is then called for audio clips
/// </summary>
public class LocalSoundManager : MonoBehaviour
{
    [SerializeField]private List<AudioSource> audioSources;
    private List<AudioClip> audioClips;
    private int currentIndex;
    public void Play(SoundManager.EAudioClips index)
    {
        
        AudioSource audio = SoundManager.GetAudioClip(index);
        if (!audio) return;
        PlayAudioClip(audio.clip);
    }

    private void PlayAudioClip(AudioClip clip)
    {

        if (audioClips.Contains(clip))
        {
            audioSources[audioClips.IndexOf(clip)].Play();
            return;
        }

        AudioSource source = new AudioSource();
        source.loop = false;
        source.clip = clip;
        source.transform.SetParent(gameObject.transform);
        audioClips.Add(clip);
        audioSources.Add(source);
        source.Play();
    }
}
