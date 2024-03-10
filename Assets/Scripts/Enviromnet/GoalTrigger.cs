using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [Header("Goal Settings")] 
    [SerializeField] private TypeGoals _typeGoal;
    [SerializeField] private float timeSave;
    [SerializeField] private int goalID;
    [Tooltip("When this is set upon entering through the portal will teleport the player to the position")]
    [SerializeField] private Vector3 teleportPoint;
    [SerializeField] private bool canBeTriggeredMoreThanOnce;
    
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
        //Maybe return after triggering firstcheckpoint
        if(goalID == 0)_checkPointManager.FirstCheckPoint();
        if (!other.gameObject.tag.Equals("PlayerTrigger") || (wasTriggered && !canBeTriggeredMoreThanOnce)) return;
        wasTriggered = true;
        respawnPoint = other.transform;
        _checkPointManager.CrossedFinishLine(goalID,respawnPoint,timeSave);
        if (teleportPoint != Vector3.zero)other.transform.parent.GetComponentInChildren<ThirdPersonController>().Teleport(teleportPoint);
    }
}
