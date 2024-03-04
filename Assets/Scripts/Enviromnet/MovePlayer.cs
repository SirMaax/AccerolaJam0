using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.tag.Equals("Player")) return;
        collision.transform.parent.parent.SetParent(gameObject.transform);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.tag.Equals("Player")) return;
        collision.transform.parent.parent.SetParent(null);
    }
    
    
}
