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
        emission.rateOverTime = particlesRate.Evaluate(breathInput.BreathValue) * emissionMulti;

        var main = particle.main;
        main.startSpeed = breathInput.BreathValue;

        var shape = particle.shape;
        if(breathInput.BreathValue <= 0)
        {
            shape.rotation = Vector3.up * 180;
            shape.radius = 1;
        }
        else if (breathInput.BreathValue > 0)
        {
            shape.rotation = Vector3.zero;
            shape.radius = 2;
        }
    }

}
