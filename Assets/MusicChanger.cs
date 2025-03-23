using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChanger : MonoBehaviour
{
    public static MusicChanger Instance { get; private set; }

    [SerializeField]
    private AudioClip levelMusic;
    [SerializeField]
    private AudioClip bossMusic;
    [SerializeField]
    private AudioClip endMusic;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void SwapToBossTrack()
    {
        if (audioSource != null)
        {
            audioSource.clip = bossMusic;
            audioSource.Play();
        }
    }

    public void PlayEndMusic()
    {
        audioSource.clip = endMusic;
        audioSource.Play();


    }
}
