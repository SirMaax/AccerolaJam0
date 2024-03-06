using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class MovePlayer : Obstacle
{
    
    protected override void ExecuteOnTriggEnter(GameObject collision)
    {
        collision.transform.parent.parent.SetParent(gameObject.transform);
    }

    protected override void ExecuteOnTriggLeave(GameObject collision)
    {
        collision.transform.parent.parent.SetParent(null);
    }
    
}
