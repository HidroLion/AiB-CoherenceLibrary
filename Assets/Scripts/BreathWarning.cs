using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathWarning : MonoBehaviour
{
    BreathInput breathInput;
    CyclesReader cyclesReader;
    bool warning;
    float t;
    int state;

    [SerializeField] Animator animator;
    [SerializeField] float maxTime;

    private void Start()
    {
        breathInput = GetComponent<BreathInput>();
        cyclesReader = GetComponent<CyclesReader>();

        t = 0;
        warning = false;
        state = 0;
    }

    private void Update()
    {
        if (breathInput.BreathValue == -1 && (state == 1 || state == 0 || state == 3))
        {
            animator.SetTrigger("Off");

            warning = true;
            state = 2;
            t = 0;
        }

        if (breathInput.BreathValue == 1 && (state == 2 || state == 0 || state == 3))
        {
            animator.SetTrigger("Off");

            warning = true;
            state = 1;
            t = 0;
        }

        if (breathInput.BreathValue == 0 && warning)
        {
            animator.SetTrigger("Off");

            t = 0;
            state = 3;
            warning = false;
        }

        if (warning)
        {
            t += Time.deltaTime;

            if(state == 2)
            {
                cyclesReader.BreathTimes[0] = t;
            }
            else if(state == 1)
            {
                cyclesReader.BreathTimes[2] = t;
            }

            if(t >= maxTime)
            {
                warning = false;
                animator.SetTrigger("On");
            }
        }
        else if(!warning && state == 3)
        {
            t += Time.deltaTime;
            cyclesReader.BreathTimes[1] = t;
        }
    }
}
