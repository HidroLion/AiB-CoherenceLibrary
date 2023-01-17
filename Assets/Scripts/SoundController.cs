using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] float soundTime;

    AudioSource audioSource;
    BreathInput breathInput;
    int state;

    float t;
    bool delay;
    int audioSelect;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        breathInput = GetComponent<BreathInput>();

        t = 0;
        delay = false;
        state = 0;
    }

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Q) && !Input.GetKeyDown(KeyCode.E))
        {
            delay = true;
            audioSelect = 1;
            PlaySound(audioClips[0]);
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            PlaySound(audioClips[3]);
        }

        if (Input.GetKeyDown(KeyCode.E) && !Input.GetKeyDown(KeyCode.Q))
        {
            delay = true;
            audioSelect = 2;
            PlaySound(audioClips[0]);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            PlaySound(audioClips[3]);
        }
        */

        if (breathInput.BreathValue == -1 && (state == 0 || state == 1))
        {
            t = 0;
            audioSource.PlayOneShot(audioClips[0], 1);
            audioSelect = 4;
            state = -1;

            delay = true;
        }

        if (breathInput.BreathValue == 1 && (state == 0 || state == -1))
        {
            t = 0;
            audioSource.PlayOneShot(audioClips[2], 1);
            audioSelect = 5;
            state = 1;

            delay = true;
        }

        if(breathInput.BreathValue == 0)
        {
            t = 0;
            state = 0;
            delay = false;
        }

        if (delay)
        {
            t += Time.deltaTime;
            if (t >= soundTime)
            {
                t = 0;
                delay = false;
                PlaySound(audioClips[audioSelect]);
            }
        }

    }

    void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
