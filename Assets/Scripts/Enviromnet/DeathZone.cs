using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class DeathZone : Obstacle
{
    private CheckPointManager _checkPointManager;
    private void Start()
    {
       _checkPointManager = GameObject.FindWithTag("CheckPointManager").GetComponent<CheckPointManager>();
    }

    protected override void ExecuteOnTriggEnter(GameObject player)
    {
        player.transform.parent.GetComponent<ThirdPersonController>().Teleport(_checkPointManager.GetRespawnPosition());
        
        //Maybe ground player? Aka reset some stuff?
    }
}
