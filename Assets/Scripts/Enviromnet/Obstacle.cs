using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    //Initial value is zero
    [SerializeField] private ETag tag;

    public enum ETag
    {
        Player,
        PlayerTrigger,
    }
    
    protected virtual void ExecuteOnTriggEnter(GameObject collision) { }
    
    protected virtual void ExecuteOnTriggLeave(GameObject collision) { }
    protected virtual void ExecuteOnTriggLeave() { }
    protected virtual void ExecuteOnTriggEnter() { }
    
    protected void OnTriggerEnter(Collider collision)
    {
        if (!collision.gameObject.tag.Equals(tag.ToString())) return;
        ExecuteOnTriggEnter(collision.gameObject);
        ExecuteOnTriggEnter();
    }
    
    protected void OnTriggerExit(Collider collision)
    {
        if (!collision.gameObject.tag.Equals(tag.ToString())) return;
        ExecuteOnTriggLeave(collision.gameObject);
        ExecuteOnTriggLeave();
    }
}
