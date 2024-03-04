using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update


    [Header("Status")] 
    [SerializeField] private bool cameraChangActive;
    private float requiredSpeedForChange;
    
    [Header("References")]
    [SerializeField]private CharacterController _controller;
    [SerializeField] CinemachineVirtualCamera _closeCamera;
    [SerializeField] CinemachineVirtualCamera _farCamera;
    [SerializeField] private ThirdPersonController thirdPersonController;
    
    void Start()
    {
        if(!_controller|| !_closeCamera ||! _farCamera || !thirdPersonController)Debug.LogError("Ref missing");
        requiredSpeedForChange = thirdPersonController.requiredAccelerationSpeed;
        _closeCamera.gameObject.SetActive(false);
    }

    // Update is called once per frame

#if  cameraChangActive
    
    void Update()
    {
        
        Vector3 velocity = _controller.velocity;
        velocity.y = 0;
        if (velocity.magnitude > requiredSpeedForChange)
        {
            _closeCamera.gameObject.SetActive(true);
            _farCamera.gameObject.SetActive(false);
        }
        else if(velocity.magnitude < requiredSpeedForChange)
        {
            _closeCamera.gameObject.SetActive(false);
            _farCamera.gameObject.SetActive(true);
        }
    }
#endif
}
