using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data;
using static Data.TypeClass.TypeLesson;

public class MusicPlayController : MonoBehaviour
{
    [SerializeField]
    Text textMusic;
    [SerializeField]
    Text textTime;
    [SerializeField]
    Slider sliderMusic;
    [SerializeField]
    Image imgPlayPause;
    [SerializeField]
    Image imgLoop;
    public AudioSource audioSource;

    [SerializeField]
    Sprite[] sprLoop;

    bool pause;
    bool replay;
    public bool loop;
    string strTimeMusic;
    string strTimeMaxMusic;
    GameController gameController;

    private void OnEnable()
    {
        if (gameController == null) gameController = GameController.instance;
        pause = audioSource.playOnAwake;
        BtnPause();
        loop = !loop;
        BtnLoop();
    }

    private void FixedUpdate()
    {
        SetSliderAccordingToTime();
    }

    public void SetMusic(AudioClip audioClip, bool isLoop)
    {
        audioSource.clip = audioClip;
        loop = isLoop;
        sliderMusic.value = 0;
        sliderMusic.maxValue = Mathf.Floor(audioClip.length);
        strTimeMusic = GameController.instance.SetTextTimeVideo(sliderMusic.value);
        strTimeMaxMusic = GameController.instance.SetTextTimeVideo(audioClip.length);
        textMusic.text = audioClip.name + " - " + strTimeMaxMusic;
        textTime.text = strTimeMusic + " / " + strTimeMaxMusic;
    }

    public void SetSliderAccordingToTime()
    {
        if (!audioSource.isPlaying) return;
        sliderMusic.value = audioSource.time;
        strTimeMusic = gameController.SetTextTimeVideo(sliderMusic.value);
        textTime.text = strTimeMusic + " / " + strTimeMaxMusic;

        if (audioSource.time >= Mathf.Floor(audioSource.clip.length))
        {
            if (!pause)
            {
                if (loop)
                {
                    pause = true;
                    replay = true;
                    BtnPause();
                }
                else
                {
                    BtnPause();
                    replay = true;
                    imgPlayPause.sprite = gameController.sprPlayPause[2];
                }
            } 
        }
    }
    public void SetTimeAccordingToSlider()
    {
        if (sliderMusic.value >= sliderMusic.maxValue) sliderMusic.value = sliderMusic.maxValue;
        else replay = false;
        CancelInvoke("BtnPause");
        audioSource.time = sliderMusic.value;
        strTimeMusic = gameController.SetTextTimeVideo(sliderMusic.value);
        textTime.text = strTimeMusic + " / " + strTimeMaxMusic;
        if (!replay)
        {
            pause = true;
            BtnPause();
            Invoke("BtnPause", 0.1f);
        }
    }

    public void ActionPause()
    {
        pause = false;
        BtnPause();
    }

    public void BtnPause()
    {
        //Debug.Log("=================== pause");
        if (!pause)
        {
            imgPlayPause.sprite = gameController.sprPlayPause[0];
            audioSource.Pause();
            pause = true;
        }
        else
        {
            imgPlayPause.sprite = gameController.sprPlayPause[1];
            audioSource.Play();
            pause = false;
        }

        if (replay)
        {
            replay = false;
            audioSource.time = 0;
            audioSource.Play();
        }
    }

    public void BtnLoop() 
    {
        if (loop)
        {
            loop = false;
            imgLoop.sprite = sprLoop[0];
        }
        else
        {
            loop = true;
            imgLoop.sprite = sprLoop[1];
        }
    }
}
