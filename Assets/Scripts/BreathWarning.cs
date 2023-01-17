using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathWarning : MonoBehaviour
{
    BreathInput breathInput;
    bool warning;
    float t;
    int state;

    [SerializeField] Animator animator;
    [SerializeField] float maxTime;

    private void Start()
    {
        breathInput = GetComponent<BreathInput>();
        t = 0;
        warning = false;
    }

    private void Update()
    {
        if(breathInput.BreathValue == 1 && (state == -1 || state == 0))
        {
            animator.SetTrigger("Off");
            warning = true;
            state = 1;
            t = 0;
        }
        if (breathInput.BreathValue == -1 && (state == 1 || state == 0))
        {
            animator.SetTrigger("Off");
            warning = true;
            state = -1;
            t = 0;
        }

        if (breathInput.BreathValue == 0 && warning)
        {
            animator.SetTrigger("Off");
            state = 0;
            warning = false;
        }

        if (warning)
        {
            t += Time.deltaTime;
            if(t >= maxTime)
            {
                warning = false;
                animator.SetTrigger("On");
            }
        }
    }
}
