using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("State of the game")] 
    public int currentStage;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void CrossedFinishLine(int goalID)
    {
        float time = Time.time;
        Debug.Log("did it");
        //Or stop timer etc.
    }
}
