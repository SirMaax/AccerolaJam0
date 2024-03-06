using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    [SerializeField] private ThirdPersonController _controller;
    
    [SerializeField] private LayerMask _layerMask;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (_controller.Grounded) return;
        int thisLayerMask = 1 << collision.gameObject.layer;
        if ((thisLayerMask & _layerMask) == 0 || _controller.onWall) return;
        // if (!collision.gameObject.tag.Equals("Wall") || _controller.onWall) return;
        _controller.onWall = true;
        Vector3 verticalVelocity = _controller.velocityVector;
        verticalVelocity.y = 0;
        _controller.entryVector = Vector3.Reflect(verticalVelocity.normalized , collision.impulse.normalized).normalized;
        _controller.entryVector.y = _controller.wallJumpAngle;
        Debug.DrawRay(transform.position + new Vector3(0,1,0),_controller.entryVector);
        Debug.DrawRay(transform.position,  50* _controller.velocityVector *-1,Color.cyan);
        Debug.DrawRay(transform.position,  _controller.entryVector,Color.red);
        Debug.LogError("Drawing Stuff");
        Debug.Log(_controller.entryVector);
        _controller.touchedWall = collision.gameObject;
    }
}
