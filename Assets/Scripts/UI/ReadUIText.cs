using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ReadUIText : Obstacle
{
    [Header("Status")] 
    [SerializeField] private bool hasPhysicalBody = true;
    private bool playerInRange;
    [SerializeField] private bool onlyHover = false;


    [Header("Reference")] 
     [SerializeField] private GameObject closedScroll;
    [SerializeField] private GameObject openedScroll;
    [SerializeField]private TMP_Text text;
    private void Start()
    {
        if (onlyHover) return;
        if (!hasPhysicalBody)
        {
            if(closedScroll!=null) closedScroll.SetActive(false);
            if(openedScroll!=null)openedScroll.SetActive(false);
        }
        else
        {
            CloseScrollPhysical();

        }

        if(text!=null)text.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (onlyHover)
        { 
            HoverUpAndDown();
            return;
        }
        if (!playerInRange) return;
        TurnTowardsPlayer();
    }

    protected override void ExecuteOnTriggEnter()
    {
        SetStatusText(true);
        OpenScrollPhysically();
        playerInRange = true;
    }

    protected override void ExecuteOnTriggLeave()
    {
        SetStatusText(false);
        CloseScrollPhysical();
        playerInRange = false;
    }

    private void SetStatusText(bool status)
    {
        text.gameObject.SetActive(true);
    }

    private void CloseScrollPhysical()
    {
        if (!hasPhysicalBody) return;
        closedScroll.SetActive(true);
        openedScroll.SetActive(false);
        text.gameObject.SetActive(false);
    }

    private void OpenScrollPhysically()
    {
        closedScroll.SetActive(false);
        openedScroll.SetActive(true);
        text.gameObject.SetActive(true);
    }
}
