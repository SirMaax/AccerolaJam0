using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class TestShowUi : MonoBehaviour
{
    [SerializeField]private List<TMP_Text> texts;
    [SerializeField] private List<GameObject> objects;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float value = objects[0].GetComponent<ThirdPersonController>().currentJumpIndex;
        texts[0].SetText("JumpIndex: " + value);
        value = objects[1].GetComponent<ThirdPersonController>().velocity;
        texts[1].SetText("Velocity: " + Mathf.Round(value * 1000)/1000);
    }
}
