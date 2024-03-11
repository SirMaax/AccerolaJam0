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
    private float currentTimer;
    private bool isTimerRunning;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTimerRunning) return;
        currentTimer += Time.deltaTime; 
        timerText.SetText(TimeSpan.FromSeconds(currentTimer).ToString());
    }

    public void StartTimer()
    {
        if (isTimerRunning) return;
        isTimerRunning = true;
        _progressSystem.EnableHeatModifieres();
        timerText.enabled = true;
    }
    
    public void StopTimer()
    {
        float oldTime = timeForCurrentSection[ProgressSystem.CURRENT_SECTION][_progressSystem.GetCurrentHeat()];
        if (oldTime == 0 || oldTime > currentTimer) timeForCurrentSection[ProgressSystem.CURRENT_SECTION][_progressSystem.GetCurrentHeat()] = currentTimer;
        isTimerRunning = false;
    }
    /// <summary>
    /// Returns the time for the current section with specified heat
    /// </summary>
    /// <param name="section"></param>
    /// <param name="heat"></param>
    /// <returns>Returns 0 if no time exists yet else will return the time</returns>
    public float GetTimeForHeatAndSection(int section, int heat = 0)
    {
        return timeForCurrentSection[section][heat];
    }
}
