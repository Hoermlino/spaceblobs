using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RoundTimer : MonoBehaviour
{

    
    public float TotalTime;
    public float RemainingTime;
    public Slider TimeSlider;
    public TextMeshProUGUI TimeNumberDisplay;

    // Start is called before the first frame update
    void Start()
    {
        RemainingTime = TotalTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(RemainingTime > 0)
        {
            // Timer did not run out yet
            RemainingTime -= Time.deltaTime;
            TimeSlider.maxValue = TotalTime;
            TimeSlider.value = TotalTime - RemainingTime;
            TimeNumberDisplay.text = RemainingTime.ToString("#.0");


        }
        else
        {
            // Timer ran out - do not display the timer anymore
            GameObject RoundTimerGO = GameObject.FindWithTag("RoundTimerTag");
            RoundTimerGO.SetActive(false);
            
        }
    }




}
