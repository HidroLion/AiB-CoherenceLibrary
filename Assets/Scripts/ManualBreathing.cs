using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualBreathing : MonoBehaviour
{
    [SerializeField] BreathInput breathInput;
    [SerializeField] float emissionMultiplier;
    [SerializeField] float deltaSpeed;
    [SerializeField] float keepTime;

    float inputValue;
    float timer;
    bool wait;

    public bool Wait { get => wait; set => wait = value; }

    private void Start()
    {
        inputValue = 0f;
        timer = 0f;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q) && !Wait)
        {
            inputValue -= deltaSpeed * Time.deltaTime * emissionMultiplier;
            inputValue = Mathf.Clamp(inputValue, -1, 1);

            timer += Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E) && !Wait)
        {
            inputValue += deltaSpeed * Time.deltaTime * emissionMultiplier;
            inputValue = Mathf.Clamp(inputValue, -1, 1);

            timer += Time.deltaTime;
        }

        if (timer >= keepTime)
        {
            inputValue = 0;
            Wait = true;
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            inputValue = 0;
            Wait = false;
            timer = 0;
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            inputValue = 0;
            Wait = false;
            timer = 0;
        }

        breathInput.BreathValue = inputValue;
    }
}
