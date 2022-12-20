using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathWarning : MonoBehaviour
{
    ManualBreathing controller;
    bool warning;
    [SerializeField] Animator animator;

    private void Start()
    {
        warning = false;
        controller = GetComponent<ManualBreathing>();
    }

    private void Update()
    {
        if(controller.Wait && !warning)
        {
            warning = true;
            animator.SetTrigger("On");
        }

        if(!controller.Wait && warning)
        {
            warning = false;
            animator.SetTrigger("Off");
        }
    }
}
