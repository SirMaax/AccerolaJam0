using System;
using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Settings")] 
    public bool disableAudio;
    private static bool disableAllAudio;
    
    [SerializeField] private EAudioClips list;
    [SerializeField] AudioSource[] SerialeffectSource;
    static AudioSource[] effectSource;

    public enum EAudioClips
    {
        abilityWasBlockedDueToCorrupt,
        flipPanelClockSound,
        jump,
        dive,
        backgroundMusic,
        backgroundNoise,
        
    }
    
    private void Start()
    {
        effectSource = new AudioSource[SerialeffectSource.Length];
        effectSource = SerialeffectSource;
        disableAllAudio = disableAudio;
    }

    public static void Play(int index){
        if (index < effectSource.Length)
        {
            Debug.LogError("Audio sound missing for "+ index);
            return;
        }
        effectSource[index].Play();
    }
    public static void Stop(int index){
        if (index < effectSource.Length)
        {
            Debug.LogError("Audio sound missing for "+ index);
            return;
        }
        effectSource[index].Stop();
    }

    public static AudioSource GetAudioClip(EAudioClips clip)
    {
        if (disableAllAudio) return null;
        int index = (int)clip;
        if (index > effectSource.Length)
        {
            Debug.LogError("Audio sound missing for "+ index);
            return null;
        }
        return effectSource[index];
    }

}

