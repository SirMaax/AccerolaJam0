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

    [Header("ProgressItem")] 
    [SerializeField]private GameObject progress;
    
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
    [SerializeField] private List<ToggleHelperClass> requiredAbberation;
    
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
        heatText.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!heatText.enabled) heatText.enabled = true;
    }

    
    
    public bool CanPlayerProgress()
    {
        float tempTime = timer.timeForCurrentSection[currentSection][heatRequirements[currentSection]]; 
        return tempTime <= timeRequirements[currentSection] &&
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
        timeGates.Clear();
        timeGates = GameObject.FindGameObjectsWithTag("Gate").ToList();
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
        if(currentSection!= 0)failTexts[currentSection-1].SetActive(false);
        failTexts[currentSection].SetActive(false);
    }

    public void SetText(bool success)
    {
        EnableHeatModifieres();
        SetCurrentHeatText();
        
        float time = timer.GetTimeForHeatAndSectionFloat(currentSection, currentHeat);
        if(time == 0) timeText.SetText("No completion time");
        else timeText.SetText(TimeSpan.FromSeconds(time).ToString());
        if (currentSection>0)
        {
            lastRequiredTimeText.SetText("Time: " + Timer.GetTimeString(timeRequirements[currentSection-1]));
            lastrequiredHeatText.SetText("Abberation:" + heatRequirements[currentSection-1]);
        }
        requiredHeatText.SetText("Abberation: " + heatRequirements[currentSection]);
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
        return _corruptAbilities.isCorrupted.Count + 1;
    }
    
    [ContextMenu("Set Stage to TEST")]
    public void FakeFinished()
    {
        currentSection = test - 1;
        ShowTextForSection();
        LoadNextSection();
        ShowNextStartText();
        SetText(true);
        SetRequiredAbberationForCurrent();
        ResetGates();
        timer.Reset();
    }
    public void Finished()
    {
        lastTimeText.SetText(timer.GetTimeForHeatAndSection(currentSection, currentHeat));

        if (CanPlayerProgress())
        {
            ShowProgressStage();
        }
        else
        {
            ShowFailTextForCurrentSection();
            SetText(false);
        }
        GameObject.FindWithTag("Medal").GetComponent<MedalSystem>().SetMedal(timer.currentTimer);
        ResetGates();
        _corruptAbilities.Reset();
        timer.Reset();
    }

    public void ProgressStage()
    {
        ShowTextForSection();
        SetRequiredAbberationForCurrent();
        LoadNextSection();
        ShowNextStartText();
        SetText(true);
        progress.SetActive(false);
        GameObject.FindWithTag("Medal").GetComponent<MedalSystem>().ResetMedal();
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
        texts[currentSection-1].SetActive(false);
        failTexts[currentSection].SetActive(true);
    }

    private void ShowNextStartText()
    {
        if(startTexts[currentSection-1]!=null)startTexts[currentSection-1].SetActive(false);
        if (startTexts[currentSection] != null)startTexts[currentSection].SetActive(true);
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
        _corruptAbilities.Reset();
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

    public void DisableAbberationEffect()
    {
        _corruptAbilities.abilitiesAreOverRidden = false;
    }
    
    public void SetRequiredAbberationForCurrent()
    {
        if (requiredAbberation[currentSection] == null) return;
        requiredAbberation[currentSection].ToggleButton();
        requiredAbberation[currentSection].canBeToggeld = false;
    }

    public void SetCurrentHeatText()
    {
        EnableHeatModifieres();
        heatText.SetText("Aberration score: " + currentHeat.ToString());

    }

    private void ShowProgressStage()
    {
        progress.SetActive(true);
    }

    public void ResetSlime()
    {
        _corruptAbilities.ResetSlime();
    }
}
