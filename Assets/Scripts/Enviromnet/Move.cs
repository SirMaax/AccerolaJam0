using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Obstacle
{
    [Header("Settings")] 
    [SerializeField] private float speed;
    [SerializeField] private List<Transform> _transforms;
    private List<Vector3> locations;
    [SerializeField] Transform overrideTransform;
    
    [Header("Player Touching")]
    [Tooltip("Starts at first cycle when player is touching and when player leaves will stop again at 0 cycle")]
    [SerializeField] private bool startCycleWithPlayerTouching;
    [Tooltip("Will stop the plattform at the last position, when player leaves it will go back to 0 cycle")]
    [SerializeField] private bool stopCycleAtLastPosition;
    private int currentIndex;
    private bool stopped;
    private bool playerTouching = false;
    // Start is called before the first frame update
    void Start()
    {
        if (startCycleWithPlayerTouching) stopped = true;
        else stopped = false;

        locations = new List<Vector3>();
        foreach (Transform t in _transforms)
        {
            locations.Add(t.position);
            Destroy(t.gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = overrideTransform!=null ? overrideTransform.position : transform.position;
        
        if (stopped) return;
        Vector3 targetPos = locations[currentIndex];
        
        Vector3 direction = targetPos - pos;
        Vector3 posBefore= pos;
        // if(overrideTransform!=null)overrideTransform.Translate(speed * Time.deltaTime * direction.normalized);
        // else transform.Translate(speed * Time.deltaTime * direction.normalized);

        if (overrideTransform != null)
        {
            Vector3 currentPos = overrideTransform.position;
            currentPos += speed * Time.deltaTime * direction.normalized;
            overrideTransform.position = currentPos;
        }
        else
        {
            Vector3 currentPos = transform.position;
            currentPos += speed * Time.deltaTime * direction.normalized;
            transform.position = currentPos;
        }
        Vector3 posAfter = overrideTransform!=null? overrideTransform.position : transform.position;
        if((posBefore - targetPos).magnitude + (posAfter - targetPos).magnitude 
           == (posBefore - posAfter).magnitude)
        {
            NextInCycle();
        }
    }

    private void NextInCycle()
    {
        if(overrideTransform!=null) overrideTransform.position = locations[currentIndex];
        else transform.position = locations[currentIndex];
        if (currentIndex + 1 == locations.Count)currentIndex = 0;
        else currentIndex += 1;
        if (stopCycleAtLastPosition&& playerTouching && startCycleWithPlayerTouching && currentIndex == 0) stopped = true;
        else if (startCycleWithPlayerTouching && !playerTouching && currentIndex == 1) stopped = true;
    }
    
    protected override void ExecuteOnTriggEnter()
    {
        playerTouching = true;
        stopped = false;
    }

    protected override void ExecuteOnTriggLeave()
    {
        playerTouching = false;
        if (stopCycleAtLastPosition) stopped = false;
    }
}
