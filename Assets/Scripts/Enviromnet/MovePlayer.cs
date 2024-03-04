using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    private bool playerIsOn = false;
    private GameObject player;
    private GameObject dummy;
    private void Update()
    {
        if (!playerIsOn) return;
        ThirdPersonController controller = player.GetComponent<ThirdPersonController>();
        
        // controller._controller.enabled = false;
        // controller._controller.transform.position = dummy.transform.position;
        // controller._controller.enabled = true;
        // player.GetComponent<CharacterController>().Move(dummy.transform.position - player.transform.position);
        
        // Debug.Log("start");
        // Debug.Log(player.transform.position);
        // Debug.Log(dummy.transform.position);
        // Debug.Log(dummy.transform.position + dummy.transform.parent.position);
        // player.transform.position = dummy.transform.position;
        // Debug.Log("after");
        // Debug.Log(player.transform.localPosition);
        Camera.main.transform.rotation = dummy.transform.rotation;
        // controller. = dummy.transform.rotation;
    }

    private void Start()
    {
        dummy = new GameObject("dummy").gameObject;
        dummy.transform.SetParent(gameObject.transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.tag.Equals("Player")) return;
        player = collision.transform.parent.gameObject;
        playerIsOn = true;
        dummy.transform.position = player.transform.position;
        dummy.transform.rotation = player.transform.rotation;
        collision.transform.parent.parent.SetParent(gameObject.transform);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.tag.Equals("Player")) return;
        playerIsOn = false;
        collision.transform.parent.parent.SetParent(null);
        Debug.Log("left");
    }
    
    
}
