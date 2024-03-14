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


    /// <summary>
    /// 
    /// </summary>
    /// <param name="goalID"></param>
    /// <param name="respawnPoint"></param>
    /// <param name="timeSave"></param>
    /// <param name="overrideTeleport"></param>
    /// <param name="checkPointGate"></param>
    /// <returns> If player can be teleported</returns>
    public bool CrossedFinishLine(int goalID, Transform respawnPoint,float timeSave, bool overrideTeleport = false, bool checkPointGate = false)
    {
        bool result = true;
        if (overrideTeleport) currentRespawnPoint = respawnPoint.position;
        if (goalID <= currentCheckPoint)result = false;
        currentRespawnPoint = respawnPoint.position;
        if(!checkPointGate)currentCheckPoint = goalID;
        Debug.Log(currentRespawnPoint);
        if (!result) return false;
        if (_progressSystem.GetCurrentSection() != goalID-1 || checkPointGate) return false;
        timer.StopTimer(timeSave);
        
        timeSaves.Add(timeSave);
        _progressSystem.DisableAbberationEffect();
        currentCheckPoint = -1;
        return true;
        
    }
    public Vector3 GetRespawnPosition()
    {
        return currentRespawnPoint;
    }

    public void FirstCheckPoint()
    {
        GameObject.FindWithTag("PlayerSound").GetComponent<LocalSoundManager>().Play(SoundManager.EAudioClips.backgroundMusic);
        timer.StartTimer();
        _progressSystem.ApplyAbberationEffect();
    }

}
