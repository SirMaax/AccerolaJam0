using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    
    
    //Initial collision Tag
    [SerializeField] private ETag tag;

    [Header("Movement Up and Down")] 
    [SerializeField]private float hoverSpeed;
    [SerializeField] private float distanceUpAndDown;
    [SerializeField]private float speedFollowPlayer;
    
    private float startPosition;
    private int multiplier = 1;
    
    public enum ETag
    {
        Player,
        PlayerTrigger,
    }

    private void Awake()
    {
        startPosition = transform.position.y;
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

    protected void HoverUpAndDown()
    {
        if ((multiplier == 1 && transform.position.y > startPosition + distanceUpAndDown) ||
            (multiplier==-1 && transform.position.y < startPosition - distanceUpAndDown))
        {
            multiplier *= -1;
        }
        transform.transform.Translate(Vector3.up * multiplier * hoverSpeed * Time.deltaTime);
    }
    
    [ContextMenu("TurnTowardsPlayer")]
    protected void TurnTowardsPlayer()
    {
        Vector3 position = GameObject.FindWithTag("Player").transform.position;
        position.y = transform.position.y;
        
        transform.LookAt(position);
    }
}
