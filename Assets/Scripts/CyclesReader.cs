using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclesReader : MonoBehaviour
{
    float[] breathTimes;
    List<float[]> timesList;
    BreathInput breathInput;

    int state;
    float timer;

    public float[] BreathTimes { get => breathTimes; set => breathTimes = value; }
    public List<float[]> TimesList { get => timesList; set => timesList = value; }
    public int State { get => state; }

    private void Start()
    {
        breathInput = GetComponent<BreathInput>();
        state = 0;

        BreathTimes = new float[4];
        TimesList = new List<float[]>();
    }

    private void Update()
    {
        switch (state)
        {
            case -1: //Keep Breathing After Breath In
                TimeCount();
                if (breathInput.BreathValue >= 0.1f)
                {
                    BreathTimes[1] = timer;
                    timer = 0;
                    state = 3;
                }
                break;

            case -2: //Keep Breathing After Breath Out
                TimeCount();
                if (breathInput.BreathValue <= -0.1f)
                {
                    BreathTimes[3] = timer;
                    timer = 0;
                    state = -3;
                }
                break;

            case -3: //Save Data > Restart the Cycl
                TimesList.Add(BreathTimes);
                BreathTimes = new float[4];
                state = 1;
                break;

            case 0: //Start Cycle = Breath In
                if(breathInput.BreathValue < -0.1f)
                {
                    timer = 0;
                    state = 1;
                }
                break;

            case 1: //Is Breathing In
                TimeCount();
                if(breathInput.BreathValue >= -0.1f)
                {
                    BreathTimes[0] = timer;
                    timer = 0;
                    state = 2;
                }
                break;

            case 2: //Stop Breathing In > Transition To Keep or Breath Out
                if(breathInput.BreathValue >= 0.1f)
                {
                    BreathTimes[1] = 0f;
                    state = 3;
                }
                else if(breathInput.BreathValue == 0)
                {
                    state = -1;
                }
                break;

            case 3: //Is Breathing Out
                TimeCount();
                if(breathInput.BreathValue < 0.1)
                {
                    BreathTimes[2] = timer;
                    timer = 0;
                    state = 4;
                }
                break;

            case 4: //Stop Breathing Out > Transition To Keep or Breath In
                if (breathInput.BreathValue <= -0.1f)
                {
                    BreathTimes[3] = 0f;
                    state = -3;
                }
                else if (breathInput.BreathValue == 0)
                {
                    state = -2;
                }
                break;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            string[] listText = new string[timesList.Count];
            for (int i = 0; i < listText.Length; i++)
            {
                listText[i] =
                    TimesList[i][0].ToString() + " " + TimesList[i][1].ToString() + " " + TimesList[i][2].ToString() + " " + TimesList[i][3].ToString();

                Debug.Log("[Datos " + i + "] " + listText[i]);
            }
        }
#endif
    }

    void TimeCount()
    {
        timer += Time.deltaTime;
    }
}
