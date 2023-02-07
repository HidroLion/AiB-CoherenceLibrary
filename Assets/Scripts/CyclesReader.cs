using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclesReader : MonoBehaviour
{
    float[] breathTimes;
    List<float[]> timesList;

    public float[] BreathTimes { get => breathTimes; set => breathTimes = value; }
    public List<float[]> TimesList { get => timesList; set => timesList = value; }

    private void Start()
    {
        BreathTimes = new float[4];
        TimesList = new List<float[]>();
    }
}
