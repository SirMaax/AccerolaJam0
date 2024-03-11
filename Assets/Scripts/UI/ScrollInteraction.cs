using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class ScrollInteraction : Obstacle
{
    
    [Header("Settings")]
    [SerializeField] private int amountPerPage;
    private int currentIndex = 0;
    private int currentPage = 0;
    private bool firstTime = true;
    
    [Header("References")]
    [SerializeField] private GameObject InteractionSign;
    [SerializeField] private StarterAssetsInputs _input;
    [SerializeField] private List<GameObject> page;
    [SerializeField] private List<ToggleHelperClass> toggleButtons;
    
    // Update is called once per frame
    private void Start()
    {
        foreach (var pages in page)
        {
            pages.SetActive(false);
        }
        page[0].SetActive(true);
    }

    void Update()
    {
        if (!_input.isUsingUi)
        {
            firstTime = true;
            return;
        }
        if (_input.isUsingUi && firstTime)
        {
            SetDisplayInteractionStatus(false);
            toggleButtons[currentIndex].ButtonSelected();
            firstTime = false;
        }
        
        if (_input.move.y == 1) Up();
        else if (_input.move.y == -1) Down();
        else if (_input.diving)
        {
            _input.diving = false;
            ToggleCurrent();
        }
        _input.move = Vector2.zero;
        
    }

    protected override void ExecuteOnTriggEnter()
    {
        Debug.Log("Player enters");
        SetDisplayInteractionStatus(true);
        _input.isInUiRange = true;
    }

    protected override void ExecuteOnTriggLeave()
    {
        SetDisplayInteractionStatus(false);
        _input.isInUiRange = false;
    }

    private void SetDisplayInteractionStatus(bool status)
    {
        InteractionSign.SetActive(status);
    }

    private void Up()
    {
        if (currentIndex == 0) return;
        toggleButtons[currentIndex].ButtonDeSelect();
        ScrollIfNecessary(-1);
        toggleButtons[currentIndex].ButtonSelected();
        MoveOtherButtonsDown();
    }

    private void Down()
    {
        if (currentIndex == toggleButtons.Count-1) return;
        toggleButtons[currentIndex].ButtonDeSelect();
        ScrollIfNecessary(1);
        toggleButtons[currentIndex].ButtonSelected();
        MoveOtherButtonsDown();
    }

    private void ToggleCurrent()
    {
        if (currentIndex == 0 && currentPage == 0)
        {
            //AppflyEffect
            _input.isUsingUi = false;
            SetDisplayInteractionStatus(true);
            toggleButtons[currentIndex].ToggleButton();
            
            return;
        }
        toggleButtons[currentIndex].ToggleButton();
        
        //Effect
    }
    /// <summary>
    /// "Scrolls" down a page
    /// </summary>
    /// <param name="direction">  -1 == up // 1 == down</param>
    private void ScrollIfNecessary(int direction)
    {
        int oldIndex = currentIndex;
        currentIndex += direction;
        
        int sideChange = 0;
        if (oldIndex > currentIndex && currentIndex % amountPerPage == 4) sideChange = -1;
        else if (oldIndex < currentIndex && currentIndex % amountPerPage == 0) sideChange = 1;
        else return;
        ResetHeightOfAllButtons();
        page[currentPage].SetActive(false);
        currentPage += sideChange;
        page[currentPage].SetActive(true);
        
        //Add effect
    }

    private void MoveOtherButtonsDown()
    {
        //Reset height from other buttons before
        ResetHeightOfAllButtons();
        for (int i = currentIndex % amountPerPage; i < amountPerPage; i++)
        {
            toggleButtons[currentIndex].MoveDown();
        }
    }

    private void ResetHeightOfAllButtons()
    {
        foreach (var button in toggleButtons)
        {
            button.ResetHeight();
        }
    }
}
