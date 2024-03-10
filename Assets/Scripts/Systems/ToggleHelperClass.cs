using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHelperClass : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private CorruptAbilities.ECorruptedAbilities list;
    [SerializeField] private int id;
    private Toggle toggle;
    private ProgressSystem _progress;
    
    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        _progress = GameObject.FindWithTag("ProgressSystem").GetComponent<ProgressSystem>();
    }
    
    public void ToggleButton()
    {
        _progress.SetStatusOfHeat(toggle.isOn, id);
    }
}
