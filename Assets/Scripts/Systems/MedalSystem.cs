using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MedalSystem : MonoBehaviour
{
    [SerializeField] List<string> times;
    [SerializeField] private List<String> texts;
    [SerializeField] private List<Color> colors;
    [SerializeField] private TMP_Text textBox;
    [SerializeField] private TMP_Text timeText;
    private float highScore;
    
    // Start is called before the first frame update
    void Start()
    {
       ResetMedal();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMedal(float time)
    {
        if (time > highScore) return;
        var reqTime = times[ProgressSystem.CURRENT_SECTION].Split(",");
        if (time < Convert.ToSingle(reqTime[3])) SetMedal(3, Convert.ToSingle(reqTime[3]));
        else if (time < Convert.ToSingle(reqTime[2])) SetMedal(2,Convert.ToSingle(reqTime[3]));
        else if (time < Convert.ToSingle(reqTime[1])) SetMedal(1,Convert.ToSingle(reqTime[2]));
        else if (time < Convert.ToSingle(reqTime[0])) SetMedal(0,Convert.ToSingle(reqTime[1]));
        else SetMedal(5,Convert.ToSingle(reqTime[0]));
        timeText.SetText(Timer.GetTimeString(time));
    }

    public void ResetMedal()
    {
        textBox.SetText(texts[4].ToString());
        GetComponent<Renderer>().material.color = colors[4];
        highScore = Mathf.Infinity;
        timeText.SetText("-");

    }

    private void SetMedal(int type, float nextReqTime)
    {
        GetComponent<Renderer>().material.color = colors[type];
        String s = type < 3 ? Timer.GetTimeString(nextReqTime) : "";
        textBox.SetText(texts[type] + '\n' + s);
    }
}
