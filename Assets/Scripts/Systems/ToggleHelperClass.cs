using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHelperClass : MonoBehaviour
{
    [Header("Settings")] 
    public bool canBeToggeld = true;
    public bool applyButton = false;
    [SerializeField]private float moveDownHeight;
    private int id;
    private bool movedDown = true;
    
    [Header("Apperances")]
    [SerializeField] private CorruptAbilities.ECorruptedAbilities list;

    private float startHeight;
    private Toggle toggle;
    private ProgressSystem _progress;
    private GameObject selectionBorder;
    private GameObject checkMark;
    private GameObject explanation;
    
    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        _progress = GameObject.FindWithTag("ProgressSystem").GetComponent<ProgressSystem>();
        startHeight = transform.position.y;
        id = (int)list;
        selectionBorder = transform.GetChild(0).gameObject;
        checkMark = transform.GetChild(1).GameObject();
        explanation = transform.GetChild(3).gameObject;
        movedDown = false;
    }
    
    public void ToggleButton(bool overide = false)
    {
        if (!canBeToggeld) return;
        if (applyButton)
        {
            // _progress.EnableHeatModifieres();
            ButtonDeSelect();
            _progress.SetText(false);
            return;
        }

        if (overide)
        {
            _progress.SetStatusOfHeat(true, id);
            checkMark.SetActive(true);
            return;
        }
        _progress.SetStatusOfHeat(toggle.isOn, id);
        checkMark.SetActive(!checkMark.activeSelf);
        _progress.SetCurrentHeatText();
    }

    public void ButtonSelected()
    {
        selectionBorder.SetActive(true);
        if(!applyButton)explanation.SetActive(true);
    }
    
    public void ButtonDeSelect()
    {
        selectionBorder.SetActive(false);
        explanation.SetActive(false);
    }

    public void MoveDown()
    {
        transform.transform.Translate(Vector3.down * moveDownHeight);
        movedDown = true;
    }

    public void MoveUp()
    {
        transform.transform.Translate(Vector3.up * moveDownHeight);
    }

    public void ResetHeight()
    {
        if (!movedDown) return;
        movedDown = false;
        MoveUp();
        // Vector3 pos = transform.position;
        // pos.y = startHeight +;
        // transform.position = pos;
    }
    
}
