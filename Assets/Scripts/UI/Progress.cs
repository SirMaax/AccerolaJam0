using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class Progress : Obstacle
{
    
    [SerializeField] private StarterAssetsInputs _input;
    [SerializeField] private GameObject InteractionSign;
    [SerializeField]private List<GameObject> border; 

    private bool firstTime;

    private int index = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_input.isUsingUi)
        {
            firstTime = true;
            return;
        }

        if (_input.isUsingUi && firstTime)
        {
            firstTime = false;
            SetDisplayInteractionStatus(false);
            border[0].SetActive(true);
        }
        if (_input.move.y == 1) Up();
        else if (_input.move.y == -1) Down();
        else if (_input.diving)
        {
            _input.diving = false;
            Interact();
        }
        _input.move = Vector2.zero;
    }

    private void Up()
    {
        if (index == 0) return;
        index = 0;
        border[0].SetActive(true);
        border[1].SetActive(false);
    }

    private void Down()
    {
        if (index == 1) return;
        index = 1;
        border[0].SetActive(false);
        border[1].SetActive(true);
    }

    private void Interact()
    {
        if (index == 0) GameObject.FindWithTag("ProgressSystem").GetComponent<ProgressSystem>().ProgressStage();
        _input.isUsingUi = false;
        
        if(index==1)SetDisplayInteractionStatus(true);
        if (index == 0)_input.isInUiRange = false;
        index = 0;
        border[0].SetActive(false);
        border[1].SetActive(false);
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
    
}
