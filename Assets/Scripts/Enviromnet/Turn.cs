using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Turn : MonoBehaviour
{

    [Header("Settings")] 
    [SerializeField] private float turnRate;
    [SerializeField] private Vector3 rotationDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationDirection,turnRate * Time.deltaTime);
    }
}
