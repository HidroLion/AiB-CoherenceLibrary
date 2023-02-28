using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIViewer : MonoBehaviour
{
    [SerializeField] Text[] timerTexts;
    [SerializeField] CyclesReader cycleReader;

    private void Update()
    {
        if (cycleReader.State == -3)
            UpdateText();
    }

    void UpdateText()
    {
        for (int i = 0; i < 4; i++)
        {
            timerTexts[i].text = cycleReader.BreathTimes[i].ToString();
        }       
    }
}
