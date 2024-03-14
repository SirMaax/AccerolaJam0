using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class CorruptMove : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] int overrideDir = -1;
    [SerializeField] float distance;
    private CorruptAbilities _corruptAbilities;
    private Vector3 direction;
    private Vector3 startPos;
    private bool active = false;
    // 0 = up and down // 1 == left and right // 2 == forward and backward
    private int whichDir = 0;
    // Start is called before the first frame update
    void Start()
    {
        active = false;
        startPos = transform.position;
        // _corruptAbilities \
        int which = Random.Range(0, 2);
        which = overrideDir == -1 ? which : overrideDir;
        if (which == 0)
        {
            //Up and Down
            direction = Vector3.up;
            whichDir = 0;
        }
        else
        {
            //Left or right or backwards forwards
            which = Random.Range(0, 2);
            if (which == 0)
            {
                //Left 
                whichDir = 1;
                direction = Vector3.right;
            }
            else
            {
                direction = Vector3.forward;
                whichDir = 2;
            }
        }

        speed = Random.Range(speed - speed / 2, speed + speed / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (!CorruptAbilities.globalAbilitiesAreOverRidden || !CorruptAbilities.movingStageCorruption)
        {
            Reset();
            return;
            
        }
        
        // if (!CorruptAbilities.globalAbilitiesAreOverRidden && active)
        // {
        //     Reset();
        //     active = false;
        //     return;
        // }
        active = true;
        transform.Translate(direction * speed * Time.deltaTime);
        if (whichDir == 0)
        {
            if (Mathf.Abs(transform.position.y - startPos.y) > distance) { direction *= -1; } 
        }
        else if (whichDir == 1)
        {
            if (Mathf.Abs(transform.position.x - startPos.x) > distance) { direction *= -1; } 
        }
        else if (whichDir == 2)
        {
            if (Mathf.Abs(transform.position.z - startPos.z) > distance) { direction *= -1; } 
        }
        
    }

    public void Reset()
    {
        transform.position = startPos;
    }

    [ContextMenu("Test Move")]
    private void Test()
    {
        active = true;
        CorruptAbilities.globalAbilitiesAreOverRidden = true;
    }
    
}
