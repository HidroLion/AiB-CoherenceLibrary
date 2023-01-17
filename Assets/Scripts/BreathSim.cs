using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathSim : MonoBehaviour
{
    float timer;
    [SerializeField] BreathInput breath;
    [SerializeField] AnimationCurve breathCurve;
    [SerializeField] float frecuency;

    // Update is called once per frame
    private void Update()
    {
        timer += Time.deltaTime * frecuency;

        if (timer > 1)
            timer = 0;

        breath.BreathValue = breathCurve.Evaluate(timer);
    }
}
