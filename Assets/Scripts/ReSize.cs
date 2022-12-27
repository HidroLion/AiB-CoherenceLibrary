using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSize : MonoBehaviour
{
    [SerializeField] BreathInput breathInput;
    [SerializeField] float sizeMultipliyer;
    [SerializeField] AnimationCurve curve;

    float sizeValue;

    void Update()
    {
        sizeValue = curve.Evaluate(breathInput.BreathValue);
        transform.localScale = Vector3.one * sizeMultipliyer * sizeValue;
    }
}
