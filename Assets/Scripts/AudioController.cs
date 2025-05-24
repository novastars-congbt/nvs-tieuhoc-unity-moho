using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField]
     AudioMixer _audio;
    [SerializeField]
     GameObject ON;
    [SerializeField]
     GameObject OFF;
    [SerializeField]
     Slider slider;

    public void SetVolume(float vol)
    {
        _audio.SetFloat("vol",  81 * vol - 80);
    }

    public void PlusVolume()
    {
        slider.value += 10;
        if (slider.value >= slider.maxValue) slider.value = slider.maxValue;
        _audio.SetFloat("vol", slider.value);
    }

    public void MinusVolume()
    {
        slider.value -= 10;
        if (slider.value <= slider.minValue) slider.value = slider.minValue;
        _audio.SetFloat("vol", slider.value);
    }

    public void On()
    {
        //AudioListener.volume = 0;
        SetVolume(slider.minValue);
        if (ON != null) ON.SetActive(false);
        if (OFF != null) OFF.SetActive(true);
    }

    public void Off()
    {
        //AudioListener.volume = 1;
        SetVolume(slider.value);
        if (OFF != null) OFF.SetActive(false);
        if (ON != null) ON.SetActive(true);
    }
}
