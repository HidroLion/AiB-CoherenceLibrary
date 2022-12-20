using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            audioSource.clip = audioClips[0];
            audioSource.Play();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            audioSource.clip = audioClips[1];
            audioSource.Play();
        }
    }
}
