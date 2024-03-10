using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [Header("Timer")] 
    public List<float> timeForCurrentSection;
    private float currentTimer;
    private bool isTimerRunning;
    
    [Header("References")] 
    private TMP_Text timer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTimerRunning) return;
        currentTimer += Time.deltaTime; 
        timer.SetText(currentTimer.ToString());
    }

    public void StartTimer()
    {
        if (isTimerRunning) return;
        isTimerRunning = true;
        
    }

    public void StopTimer()
    {
        float oldTime = timeForCurrentSection[ProgressSystem.CURRENT_SECTION];
        if (oldTime == 0 || oldTime > currentTimer) timeForCurrentSection[ProgressSystem.CURRENT_SECTION] = currentTimer;
        isTimerRunning = false;
    }


}
