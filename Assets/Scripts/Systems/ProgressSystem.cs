using System.Collections.Generic;
using UnityEngine;

public class ProgressSystem : MonoBehaviour
{
    [Header("Progression")] 
    public int currentSection;
    public int currentSubSection;
    public static int CURRENT_SECTION;
    [Header("Heat")]
    [SerializeField]private List<bool> isCorrupted;
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
    private Timer timer;
    private CorruptAbilities _corruptAbilities;
    private DialogManager _dialog;
    
    // Start is called before the first frame update
    void Start()
    {
        timer = GameObject.FindWithTag("Timer").GetComponent<Timer>();
        _corruptAbilities = GameObject.FindWithTag("Player").GetComponent<CorruptAbilities>();
        _dialog = GameObject.FindWithTag("DialogManager").GetComponent<DialogManager>();
        amountModifiers = _corruptAbilities.isCorrupted.Count;
        isCorrupted = new List<bool>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanPlayerProgress()
    {
        return timer.timeForCurrentSection[currentSection] <= timeRequirements[currentSection] &&
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

    private void SetHeatModifiersStatus()
    {
        for (int i = 0; i < isCorrupted.Count; i++)
        {
            _corruptAbilities.isCorrupted[i] = isCorrupted[i];
        }
    }

    public void SetStatusOfHeat(bool status, int id)
    {
         isCorrupted[id] = status;
    }
}
