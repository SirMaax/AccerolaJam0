using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This LocalSoundManager is attachted to entities like the player etc
/// The GlobalSoundManager is then called for audio clips
/// </summary>
public class LocalSoundManager : MonoBehaviour
{
    // [SerializeField]private List<AudioSource> audioSources;
    [SerializeField] private AudioSource backgroundMusic;
    private bool isPlaying;
    private List<AudioClip> audioClips;
    private int currentIndex;
    [SerializeField]private List<AudioSource> sounds;
    private void Start()
    {
        audioClips = new List<AudioClip>();
    }
    
    private void Update()
    {
        if (Timer.isTimerRunning && !isPlaying && backgroundMusic!=null)
        {
            if (backgroundMusic.isPlaying) return;
            backgroundMusic.Play();
            isPlaying = true;
        }
        else
        {
            if(backgroundMusic!=null)backgroundMusic.Stop();
            isPlaying = false;
        }
    }

    public void Play(SoundManager.EAudioClips index)
    {
        if (index == SoundManager.EAudioClips.flipPanelClockSound)
        {
            GetComponent<AudioSource>().Play();
            return;
        }
        if (index == (int)SoundManager.EAudioClips.abilityWasBlockedDueToCorrupt) return;
        sounds[(int)index].Play();
        return;
        
        AudioSource audio = SoundManager.GetAudioClip(index);
        if (!audio) return;
        PlayAudioClip(audio.clip);
    }

    public void Pause(SoundManager.EAudioClips index)
    {
        if (index == (int)SoundManager.EAudioClips.abilityWasBlockedDueToCorrupt) return;
        sounds[(int)index].Stop();
        return;
    }
    
    private void PlayAudioClip(AudioClip clip)
    {

        // if (audioClips.Contains(clip))
        // {
        //     audioSources[audioClips.IndexOf(clip)].Play();
        //     return;
        // }
        //
        // source = gameObject.AddComponent<AudioSource>();
        //
        // source.loop = false;
        // source.clip = clip;
        // source.transform.SetParent(gameObject.transform);
        // audioClips.Add(clip);
        // audioSources.Add(source);
        // source.Play();
    }
}
