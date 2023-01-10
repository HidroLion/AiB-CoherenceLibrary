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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            audioSource.PlayOneShot(audioClips[0], 1);
            //audioSource.PlayOneShot(audioClips[1], 0.5f);
            audioSelect = 4;
            delay = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            audioSource.PlayOneShot(audioClips[2], 1);
            //audioSource.PlayOneShot(audioClips[3], 0.5f);
            audioSelect = 5;
            delay = true;
        }
    }

    void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
