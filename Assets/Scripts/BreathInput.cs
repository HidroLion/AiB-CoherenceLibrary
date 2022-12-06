using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathInput : MonoBehaviour
{
    //Default Values
    float breathValue;
    //Breathing Values
    [SerializeField] float frecuency;

    //Reference to the Breath Propierties for another Scripts
    public float BreathValue { get => breathValue; set => breathValue = value; }
    public float Frecuency { get => frecuency; set => frecuency = value; }
}
