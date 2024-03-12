using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [Header("Checkpoints")] 
    [SerializeField]private Vector3 currentRespawnPoint;
    private int currentCheckPoint = -1;

    [Header("times")] 
    private List<float> times;
    private List<float> timeSaves;

    [Header("References")] 
    private Timer timer;
    private ProgressSystem _progressSystem;
    // Start is called before the first frame update
    void Start()
    {
        times = new List<float>();
        timeSaves = new List<float>();
        timer = GameObject.FindWithTag("Timer").GetComponent<Timer>();
        _progressSystem = GameObject.FindWithTag("ProgressSystem").GetComponent<ProgressSystem>();
    }



    public void CrossedFinishLine(int goalID, Transform respawnPoint,float timeSave, bool overrideTeleport = false)
    {
        if (overrideTeleport) currentRespawnPoint = respawnPoint.position;
        if (goalID <= currentCheckPoint) return;
        currentRespawnPoint = respawnPoint.position;
        currentCheckPoint = goalID;
        Debug.Log(currentRespawnPoint);

        if (_progressSystem.GetCurrentSection() != goalID-1) return;
        timer.StopTimer();
        timeSaves.Add(timeSave);
        _progressSystem.DisableAbberationEffect();
        currentCheckPoint = -1;
        return;
        
    }
    public Vector3 GetRespawnPosition()
    {
        return currentRespawnPoint;
    }

    public void FirstCheckPoint()
    {
        timer.StartTimer();
        _progressSystem.ApplyAbberationEffect();
    }

}
