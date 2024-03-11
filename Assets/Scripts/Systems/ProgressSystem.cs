using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressSystem : MonoBehaviour
{
    [Header("Progression")] 
    public int currentSection;
    public int currentSubSection;
    [SerializeField] private int maxSection;
    public static int CURRENT_SECTION;
    
    [Header("Heat")]
    private int currentHeat;
    private int amountModifiers;

    [Header("Requirements")] 
    [SerializeField]private List<float> timeRequirements;
    [SerializeField]private List<int> heatRequirements;
    
    [Header("GameObjects")] 
    [SerializeField] private List<bool> loadUpNewLevel;
    [SerializeField] private List<GameObject> sectionParts;
    [SerializeField] private List<GameObject> additionalHeat;
    
    [Header("References")] 
    [SerializeField]private CorruptAbilities _corruptAbilities;
    [SerializeField] private TMP_Text heatText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text requiredHeatText;
    [SerializeField] private TMP_Text requiredTimeText;
    private Timer timer;
    private DialogManager _dialog;
    
    // Start is called before the first frame update
    void Start()
    {
        timer = GameObject.FindWithTag("Timer").GetComponent<Timer>();
        // _corruptAbilities = GameObject.FindWithTag("Player").GetComponent<CorruptAbilities>();
        // _dialog = GameObject.FindWithTag("DialogManager").GetComponent<DialogManager>();
        amountModifiers = _corruptAbilities.isCorrupted.Count;
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

    private void LoadNextSection()
    {
        currentSection += 1;
        CURRENT_SECTION = currentSection;
        if(loadUpNewLevel[currentSection])sectionParts[currentSection].SetActive(true);
        if(additionalHeat.Count < currentSection && additionalHeat[currentSection]!= null)additionalHeat[currentHeat].SetActive(true);
        _dialog.TriggerDialogForSection(currentSection);
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
        _corruptAbilities.abilitiesAreOverRidden = true;
    }

    public void SetText()
    {
        heatText.SetText("Heat " + currentHeat.ToString());

        float time = timer.GetTimeForHeatAndSection(currentSection, currentHeat);
        if(time == 0) timeText.SetText("No completion time");
        else timeText.SetText(TimeSpan.FromSeconds(time).ToString());
        requiredHeatText.SetText("Required Heat: " + heatRequirements[currentSection]);
        requiredTimeText.SetText("Required Time: " + TimeSpan.FromSeconds(timeRequirements[currentSection]));
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
}
