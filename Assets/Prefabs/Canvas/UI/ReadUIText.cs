using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReadUIText : Obstacle
{
    [Header("Status")]
    
    private bool playerInRange;
    
    
    [Header("Reference")] 
    private TMP_Text text;

    private void Start()
    {
        text = GetComponentInChildren<TMP_Text>();
        text.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        HoverUpAndDown();
        if (!playerInRange) return;
        TurnTowardsPlayer();
    }

    protected override void ExecuteOnTriggEnter()
    {
        SetStatusText(true);
        playerInRange = true;
    }

    protected override void ExecuteOnTriggLeave()
    {
        SetStatusText(false);
        playerInRange = false;
    }

    private void SetStatusText(bool status)
    {
        text.enabled = true;
    }
}
