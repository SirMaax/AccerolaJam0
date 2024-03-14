using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [Header("Timer")]
    //Time for each section with heat
    public float[][] timeForCurrentSection;
    public float currentTimer;
    public static bool isTimerRunning;
    [Header("References")] 
    [SerializeField]private TMP_Text timerText;
    private ProgressSystem _progressSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        _progressSystem = transform.parent.GetComponentInChildren<ProgressSystem>();
        timeForCurrentSection = new float[_progressSystem.GetMaxSections()][];
        for (int i = 0; i < timeForCurrentSection.Length; i++)
        {
            timeForCurrentSection[i] = new float[_progressSystem.GetMaxPossibleHeat()];
        }
        timerText.enabled = false;
        isTimerRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTimerRunning) return;
        currentTimer += Time.deltaTime;
        try
        {
            String time = TimeSpan.FromSeconds(currentTimer).ToString();
            if (time.Length > 12) time = time.Substring(3, 9);
            else if(time.Length > 11)time = time.Substring(3, 8);
            else if(time.Length > 10)time = time.Substring(3, 7);
            timerText.SetText(time);
        }
        catch (Exception e)
        {
            Debug.Log(TimeSpan.FromSeconds(currentTimer).ToString());
        }
    }

    public void StartTimer()
    {
        if (isTimerRunning) return;
        isTimerRunning = true;
        
        _progressSystem.EnableHeatModifieres();
        timerText.enabled = true;
    }
    
    public void StopTimer(float timeSave = 0)
    {
        GameObject.FindWithTag("PlayerSound").GetComponent<LocalSoundManager>().Pause(SoundManager.EAudioClips.backgroundMusic);

        float oldTime = timeForCurrentSection[ProgressSystem.CURRENT_SECTION][_progressSystem.GetCurrentHeat()];
        if (oldTime == 0 || oldTime > currentTimer) timeForCurrentSection[ProgressSystem.CURRENT_SECTION][_progressSystem.GetCurrentHeat()] = currentTimer - timeSave;
        
        isTimerRunning = false;
        timerText.enabled = false;
        _progressSystem.Finished();
    }
    /// <summary>
    /// Returns the time for the current section with specified heat
    /// </summary>
    /// <param name="section"></param>
    /// <param name="heat"></param>
    /// <returns>Returns 0 if no time exists yet else will return the time</returns>
    public string GetTimeForHeatAndSection(int section, int heat = 0)
    {
        String s = TimeSpan.FromSeconds(timeForCurrentSection[section][heat]).ToString();
        if (s.Length > 12) s = s.Substring(3, 9);
        else if(s.Length > 11)s = s.Substring(3, 8);
        else if(s.Length > 10)s= s.Substring(3, 7);
        return s;
    }

    public float GetTimeForHeatAndSectionFloat(int section, int heat = 0)
    {
        return timeForCurrentSection[section][heat];
    }

    public static string GetTimeString(float time)
    {
        String s = TimeSpan.FromSeconds(time).ToString();
        if (s.Length > 12) s = s.Substring(3, 9);
        else if(s.Length > 11)s = s.Substring(3, 8);
        else if(s.Length > 10)s= s.Substring(3, 7);
        return s;
    }

    public void Reset()
    {
        isTimerRunning = false;
        currentTimer = 0;
    }
}
