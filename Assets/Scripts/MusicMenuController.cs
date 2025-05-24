using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicMenuController : MonoBehaviour
{
    public static MusicMenuController instance;

    public int currentSong;
    [SerializeField]
     float currentTime;
    [SerializeField]
     Text textSong;
    [SerializeField]
     TextMeshProUGUI textTimeSong;
    [SerializeField]
     Transform listSong;

    [SerializeField]
     Slider sliderTimeSong;

    GameController gameController;
    MusicController musicController;

    public Sprite[] spsFrame;
    [SerializeField]
     SongController[] song;

    private void Awake()
    {
        if (instance == null) instance = this;
        if (gameController == null) gameController = GameController.instance;
        if (musicController == null) musicController = gameController.musicController;
        
        SetCurrentSong();
        for (int i = 0; i < gameController.data.typeMusics.Length; i++)
        {
            song[i].index = i;
            song[i].textSong.text = gameController.data.typeMusics[i].name + " - " + SetTime(gameController.data.typeMusics[i].time);
            song[i].gameObject.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        if (gameController.musicController.soundMusic[currentSong].isPlaying)
        {
            currentTime += Time.fixedDeltaTime;
        }
        SetSliderAccordingToTime();
    }

    string SetTime(float time)
    {
        int minute = (int)time / 60;
        int second = (int)time % 60;
        if (second < 10)
        {
            return "" + minute + ":0" + second;
        }
        else
        {
            return "" + minute + ":" + second;
        }
    }

    public void SetCurrentSong()
    {
        textSong.text = gameController.data.typeMusics[currentSong].name;
        currentTime = 0;
        sliderTimeSong.maxValue = gameController.data.typeMusics[currentSong].time;
        textTimeSong.text = gameController.SetTextTimeVideo(gameController.data.typeMusics[currentSong].time);
    }

    public void SetSliderAccordingToTime()
    {
        sliderTimeSong.value = currentTime;
       
    }

    public void SetTimeAccordingToSlider(float value)
    {
        if (Mathf.Abs(value - currentTime) < 1) return;
        currentTime = value;
        musicController.soundMusic[currentSong].Stop();
        musicController.soundMusic[currentSong].time = currentTime;
        musicController.soundMusic[currentSong].Play();
    }

    bool tweenMusic;
    public void BtnMusic() 
    {
        if (!tweenMusic)
        {
            listSong.DOScaleY(1f, 0.25f);
            tweenMusic = true;
        }
        else
        {
            listSong.DOScaleY(0, 0.25f);
            tweenMusic = false;
        }
    }

    public void BtnPlay()
    {
        if (!musicController.soundMusic[currentSong].isPlaying)
        {
            musicController.soundMusic[currentSong].Play();

        }
        else
        {
            musicController.soundMusic[currentSong].Pause();
        }
    }

    public void BtnStop()
    {
        musicController.soundMusic[currentSong].Stop();
        currentTime = 0;
    }
}
