using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController instance;

    [SerializeField]
     AudioSource soundTimer;
    public AudioSource[] soundMusic;

    private void Awake()
    {
        if (instance == null) instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void PlaySoundTimer(AudioClip audioClip)
    {
        soundTimer.PlayOneShot(audioClip);
    }

}
