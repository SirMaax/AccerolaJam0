using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flip : Obstacle
{
    [Header("Setitngs")] 
    [SerializeField] private int rotationDegree = 180;
    [SerializeField] private float flipTime;
    [SerializeField] private Vector3 flipRotation;
    [Tooltip("Has to add up perfectly to 180")]
    [SerializeField] private int flipSpeed;
    [SerializeField] private float timeBetweenSmallFlip;
    [Tooltip("If plattform should flip before the start")]
    [SerializeField] private bool flipSide;
    //Changes between 1 and -1 
    private int multiplier = 1;
    // Start is called before the first frame update

    private void Awake()
    {
        if (flipSide)
        {
            transform.Rotate(flipRotation,rotationDegree);
            multiplier *= -1;
        }
    }

    IEnumerator Start()
    {
        while (true)
        {
            for (int i = 0; i < rotationDegree; i +=flipSpeed)
            {
                yield return new WaitForSeconds(timeBetweenSmallFlip);
                transform.Rotate(flipRotation,flipSpeed * multiplier);
            }
            yield return new WaitForSeconds(flipTime);
            multiplier *= -1;
        }
    }
    
    
}
