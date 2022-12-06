using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BurbblesGenerator : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;

    [SerializeField] BreathInput breathInput;
    [SerializeField] AnimationCurve particlesRate;

    [SerializeField] float emissionMulti;

    private void Update()
    {
        var emission = particle.emission;
        emission.rateOverTime = breathInput.BreathValue * emissionMulti;
    }

}
