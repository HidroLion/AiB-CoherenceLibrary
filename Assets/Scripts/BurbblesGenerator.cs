using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BurbblesGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ParticleSystem particle;
    [SerializeField] BreathInput breathInput;
    [SerializeField] AnimationCurve particlesRate;

    [Header("Variables")]
    [SerializeField] float emissionMulti;
    [SerializeField] float outRadius;
    [SerializeField] float inRadius;

    private void Update()
    {
        var emission = particle.emission;
        emission.rateOverTime = particlesRate.Evaluate(breathInput.BreathValue) * emissionMulti;

        var main = particle.main;
        main.startSpeed = breathInput.BreathValue;

        var shape = particle.shape;
        var velocityLifetime = particle.velocityOverLifetime;
        if(breathInput.BreathValue <= 0)
        {
            shape.rotation = Vector3.up * 180;
            velocityLifetime.y = 0;
            shape.radius = inRadius;
        }
        else if (breathInput.BreathValue > 0)
        {
            shape.rotation = Vector3.zero;
            velocityLifetime.y = 1;
            shape.radius = outRadius;
        }
    }

}
