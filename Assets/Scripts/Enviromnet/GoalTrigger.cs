using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{
    [Header("Goal Settings")] 
    [SerializeField] private TypeGoals _typeGoal;
    [SerializeField] private float timeSave;
    [SerializeField] private int goalID;
    [Tooltip("When this is set upon entering through the portal will teleport the player to the position")]
    [SerializeField] private Vector3 teleportPoint;
    [SerializeField] private bool canBeTriggeredMoreThanOnce;
    [SerializeField] private String triggerLevelChange;
    [SerializeField] private bool checkPointOnly = false;
    
    
    private Transform respawnPoint;
    private bool wasTriggered = false;
    
    private CheckPointManager _checkPointManager;
    private enum TypeGoals
    {
        normal,
        hard,
    }

    private void Awake()
    {
        Start();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_typeGoal == TypeGoals.normal)
        {
            timeSave = 0;
            transform.parent.GetChild(1).gameObject.SetActive(false);
        }
        _checkPointManager = GameObject.FindWithTag("CheckPointManager").GetComponent<CheckPointManager>();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(_checkPointManager == null)_checkPointManager = GameObject.FindWithTag("CheckPointManager").GetComponent<CheckPointManager>();
        //Maybe return after triggering firstcheckpoint
        if(goalID == 0)_checkPointManager.FirstCheckPoint();
        if (!other.gameObject.tag.Equals("PlayerTrigger") || (wasTriggered && !canBeTriggeredMoreThanOnce)) return;
        wasTriggered = true;
        respawnPoint = other.transform;
        bool overrideRespawn = goalID == 0;
        ProgressSystem pro = GameObject.FindWithTag("ProgressSystem").GetComponent<ProgressSystem>();
        if (triggerLevelChange == "2. Level") SceneManager.LoadScene("2. Level");
        bool teleportPlayer =_checkPointManager.CrossedFinishLine(goalID, respawnPoint, timeSave, overrideRespawn, checkPointOnly);

        
        if (teleportPlayer && !checkPointOnly)
        {
            if (teleportPoint != Vector3.zero)other.transform.parent.GetComponentInChildren<ThirdPersonController>().Teleport(teleportPoint);
            
        }
    }

    public void Reset()
    {
        wasTriggered = false;
    }
}
