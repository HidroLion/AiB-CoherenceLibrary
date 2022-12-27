using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] float soundTime;

    AudioSource audioSource;
    float t;
    bool delay;
    int audioSelect;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        t = 0;
        delay = false;
    }

    void Update()
    {
        if (delay)
        {
            t += Time.deltaTime;
            if(t >= soundTime)
            {
                t = 0;
                delay = false;
                PlaySound(audioClips[audioSelect]);
            }
        }

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
    }

    void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
