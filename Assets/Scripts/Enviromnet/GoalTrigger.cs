using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [Header("Goal Settings")] 
    [SerializeField] private TypeGoals _typeGoal;
    [SerializeField] private float timeSave;
    [SerializeField] private int goalID;
    private Transform respawnPoint;
    private bool wasTriggered = false;
    
    private CheckPointManager _checkPointManager;
    private enum TypeGoals
    {
        normal,
        hard,
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (_typeGoal == TypeGoals.normal) timeSave = 0;
        _checkPointManager = GameObject.FindWithTag("CheckPointManager").GetComponent<CheckPointManager>();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.tag.Equals("PlayerTrigger")|| wasTriggered) return;
        wasTriggered = true;
        respawnPoint = other.transform;
        _checkPointManager.CrossedFinishLine(goalID,respawnPoint,timeSave);
    }
}
