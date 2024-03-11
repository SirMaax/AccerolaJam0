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

    [Header("References")] private Timer timer;
    // Start is called before the first frame update
    void Start()
    {
        times = new List<float>();
        timeSaves = new List<float>();
        timer = GameObject.FindWithTag("Timer").GetComponent<Timer>();
    }

    public void CrossedFinishLine(int goalID, Transform respawnPoint,float timeSave)
    {
        if (goalID <= currentCheckPoint) return;
        currentRespawnPoint = respawnPoint.position;
        currentCheckPoint = goalID;
        timer.StopTimer();
        times.Add(Time.time); 
        timeSaves.Add(timeSave);
        Debug.Log(currentRespawnPoint);
    }
    public Vector3 GetRespawnPosition()
    {
        return currentRespawnPoint;
    }

    public void FirstCheckPoint()
    {
        timer.StartTimer();
        
    }
}
