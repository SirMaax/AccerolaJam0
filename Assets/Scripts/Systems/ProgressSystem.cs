using System;
using System.Collections.Generic;
using System.Linq;
using StarterAssets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ProgressSystem : MonoBehaviour
{
    [Header("Test")] [SerializeField] private int test;
    
    [Header("Progression")] 
    public int currentSection;
    public int currentSubSection;
    [SerializeField] private int maxSection;
    public static int CURRENT_SECTION;
    
    [Header("Heat")]
    private int currentHeat;
    private int amountModifiers;

    [Header("Player Telepor")] 
    [SerializeField] private Transform beforeGateCoord;
    
    [Header("Requirements")] 
    [SerializeField]private List<float> timeRequirements;
    [SerializeField]private List<int> heatRequirements;
    [SerializeField] private List<int> mainSections;
    
    [Header("GameObjects")] 
    [SerializeField] private List<bool> loadUpNewLevel;
    [SerializeField] private List<GameObject> sectionParts;
    [SerializeField] private List<GameObject> removeParts;
    [SerializeField] private List<GameObject> additionalHeat;
    [SerializeField] private List<GameObject> texts;
    [SerializeField] private List<GameObject> failTexts;
    [SerializeField] private List<GameObject> startTexts;

    
    [Header("References")] 
    [SerializeField]private CorruptAbilities _corruptAbilities;
    [SerializeField] private TMP_Text heatText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text requiredHeatText;
    [SerializeField] private TMP_Text lastrequiredHeatText;
    [SerializeField] private TMP_Text requiredTimeText;
    [SerializeField] private TMP_Text lastRequiredTimeText;
    [SerializeField] private TMP_Text lastTimeText;
    private Timer timer;
    private DialogManager _dialog;

    private List<GameObject> timeGates;
    // Start is called before the first frame update
    void Start()
    {
        timer = GameObject.FindWithTag("Timer").GetComponent<Timer>();
        // _corruptAbilities = GameObject.FindWithTag("Player").GetComponent<CorruptAbilities>();
        // _dialog = GameObject.FindWithTag("DialogManager").GetComponent<DialogManager>();
        amountModifiers = _corruptAbilities.isCorrupted.Count;
        
        timeGates = GameObject.FindGameObjectsWithTag("Gate").ToList();
    }

    // Update is called once per frame
    void Update()
    {
    }

    
    
    public bool CanPlayerProgress()
    {
        return timer.timeForCurrentSection[currentSection][heatRequirements[currentSection]] <= timeRequirements[currentSection] &&
                currentHeat >= heatRequirements[currentSection];
    }

    [ContextMenu("Load Next Section")]
    private void LoadNextSection()
    {
        currentSection += 1;
        CURRENT_SECTION = currentSection;
        if(loadUpNewLevel[currentSection])sectionParts[currentSection].SetActive(true);
        if(removeParts[currentSection]!=null)removeParts[currentSection].SetActive(false);
        if(additionalHeat.Count > currentSection && additionalHeat[currentSection]!= null)additionalHeat[currentHeat].SetActive(true);
    }
    
    public void SetStatusOfHeat(bool status, int id)
    {
        _corruptAbilities.isCorrupted[id] = status;
    }
    
    public void EnableHeatModifieres()
    {
        currentHeat = 0;
        foreach (var ability in _corruptAbilities.isCorrupted)
        {
            currentHeat += ability ? 1 : 0;
        }
        
    }

    public void SetText(bool success)
    {
        EnableHeatModifieres();
        heatText.SetText("Heat " + currentHeat.ToString());

        float time = timer.GetTimeForHeatAndSectionFloat(currentSection, currentHeat);
        if(time == 0) timeText.SetText("No completion time");
        else timeText.SetText(TimeSpan.FromSeconds(time).ToString());
        if (currentSection>0)
        {
            lastRequiredTimeText.SetText("Time: " + Timer.GetTimeString(timeRequirements[currentSection-1]));
            lastrequiredHeatText.SetText("Heat:" + heatRequirements[currentSection-1]);
        }
        requiredHeatText.SetText("Heat: " + heatRequirements[currentSection]);
        requiredTimeText.SetText("Time: " + Timer.GetTimeString(timeRequirements[currentSection]));

    }

    public int GetCurrentHeat()
    {
        return currentHeat;
    }

    public int GetMaxSections()
    {
        return maxSection;
    }

    public int GetMaxPossibleHeat()
    {
        return _corruptAbilities.isCorrupted.Count;
    }
    
    [ContextMenu("Set Stage to TEST")]
    public void FakeFinished()
    {
        currentSection = test - 1;
        ShowTextForSection();
        LoadNextSection();
        ShowNextStartText();
        SetText(true);
        ResetGates();
        timer.Reset();
    }
    public void Finished()
    {
        lastTimeText.SetText(timer.GetTimeForHeatAndSection(currentSection, currentHeat));

        if (CanPlayerProgress())
        {
            ShowTextForSection();
            LoadNextSection();
            ShowNextStartText();
            SetText(true);
        }
        else
        {
            ShowFailTextForCurrentSection();
            SetText(false);
        }

        ResetGates();
        timer.Reset();
    }

    private void ShowTextForSection()
    {
        if (currentSection != 0)
        {
            texts[currentSection - 1].SetActive(false);
            failTexts[currentSection-1].SetActive(false);
        }
        texts[currentSection].SetActive(true);
    }

    private void ShowFailTextForCurrentSection()
    {
        failTexts[currentSection].SetActive(true);
    }

    private void ShowNextStartText()
    {
        startTexts[currentSection-1].SetActive(false);
        startTexts[currentSection].SetActive(true);
    }

    private void ResetGates()
    {
        foreach (var gate in timeGates)
        {
            gate.GetComponent<GoalTrigger>().Reset();
        }
    }

    public void ResetPlayerToBeforeGates()
    {
        ResetGates();
        timer.Reset();
        GameObject.FindWithTag("Player").transform.parent.GetComponentInChildren<ThirdPersonController>().Teleport(beforeGateCoord.position);
    }
    
    public int GetCurrentSection()
    {
        return mainSections[currentSection];
    }

    public void ApplyAbberationEffect()
    {
        _corruptAbilities.abilitiesAreOverRidden = true;
    }
    
}
