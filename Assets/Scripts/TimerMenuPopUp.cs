using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerMenuPopUp : PopupProperties {
    public static TimerMenuPopUp instance;
    [SerializeField]
     int defaultMinuteDisplay;
    [SerializeField]
     int defaultSecondRun;
    [SerializeField]
     int time;
    public int currentBell;
    [SerializeField]
     float currentTime = 120f;
    [SerializeField]
     bool isAuto;
    [SerializeField]
     bool isLoop;
    [SerializeField]
     TextMeshProUGUI textTimer;
    [SerializeField]
     TextMeshProUGUI textUnitTime;

    [SerializeField]
     GameObject btnTimerUp;
    [SerializeField]
     GameObject btnTimerDown;
    [SerializeField]
     GameObject btnStart;
    [SerializeField]
     GameObject btnCountDown;

    [SerializeField]
     AudioSource audioSource;

    [SerializeField]
     TMP_InputField[] inputField;

    [SerializeField]
     AudioClip[] audioClip;

    bool isPlay = false;

    private void OnEnable() {
        if (isAuto) {
            BtnCountDown();
        }
        else 
        {
            if (btnTimerDown != null) btnTimerDown.gameObject.SetActive(true);
            if (btnTimerUp != null) btnTimerUp.gameObject.SetActive(true);
            btnStart.gameObject.SetActive(true);
            btnCountDown.gameObject.SetActive(false);
            time = defaultMinuteDisplay;
            textTimer.text = "" + time;
            textUnitTime.text = "phút";
        }
    }

    private void OnDisable() {
        CancelInvoke("CountDown");
    }

    private void Awake() {
        if (instance == null) instance = this;
    }

    private void Update() {

    }

    public void SetTextTimer() {
        int[] value = new int[4];
        int minute = (int)currentTime / 60;
        int second = (int)currentTime % 60;
        value[0] = minute / 10;
        value[1] = minute % 10;
        value[2] = second / 10;
        value[3] = second % 10; 
        for (int i = 0; i < inputField.Length; i++)
        {
            inputField[i].text = value[i].ToString();
        }
    }

    public void SetTime() {
        int[] value = new int[4];
        for (int i = 0; i < inputField.Length; i++)
        {
            if (inputField[i].text == "") inputField[i].text = "0";
            value[i] = Int32.Parse(inputField[i].text);
        }
        currentTime = value[0] * 600 + value[1] * 60 + value[2] * 10 + value[3];
    }

    public void BtnTimerUp() {
        time++;
        textTimer.text = "" + time;
    }

    public void BtnTimerDown() {
        if (time <= 1) return;
        time--;
        textTimer.text = "" + time;
    }

    public void BtnStart() {
        audioSource.PlayOneShot(audioClip[0]);
        if (btnTimerDown != null) btnTimerDown.SetActive(false);
        if (btnTimerUp != null) btnTimerUp.SetActive(false);
        btnStart.SetActive(false);
        btnCountDown.SetActive(true);
    }

    public void BtnCountDown() {
        textUnitTime.text = "giây";
        time = defaultSecondRun;
        textTimer.text = "" + time;
        audioSource.PlayOneShot(audioClip[2]);
        CancelInvoke("CountDown");
        InvokeRepeating("CountDown", 1f, 1f);
    }

    void CountDown() {
        if (time > 0) {
            audioSource.PlayOneShot(audioClip[2]);
            time--;
            textTimer.text = "" + time;
        }
        if (time <= 0) {
            if (isAuto) {
                audioSource.PlayOneShot(audioClip[3]);
            } else {
                audioSource.PlayOneShot(audioClip[1]);
            }
            CancelInvoke("CountDown");
            if (isLoop) Invoke("BtnCountDown", 1f);
        }
    }

    public override void BtnClose() {
        base.BtnClose();
    }

    public void BtnPlay() {
        if (!isPlay) {
            isPlay = true;
        } else {
            isPlay = false;
        }
    }

    public void BtnStop() {
        isPlay = false;
        currentTime = 120f;
        SetTextTimer();
    }

    public void BtnMinute(int index) {
        currentTime = index * 60f;
        SetTextTimer();
    }

    public void End() {
        Debug.Log("================== end");
    }
}
