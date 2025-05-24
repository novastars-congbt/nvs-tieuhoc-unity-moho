using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
public class ExpiredPopUp : PopupProperties
{
    [SerializeField]
     Button graceButton;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Graced")) {
            int hadGraced = PlayerPrefs.GetInt("Graced", 0);
            if (hadGraced == 1 && graceButton != null) {
                    graceButton.interactable = false;
            }
        }
    }
    private float twoDaysInSeconds = 2 * 24 * 60 * 60;
    public void GracePeriod()
    {
        DateTime currentDate = DateTime.Now;
        DateTime expiredDate = currentDate.AddDays(2);
        long timestamp = expiredDate.Ticks;
        PlayerPrefs.SetString("ExpiredTime", timestamp.ToString());
        PlayerPrefs.SetInt("Graced", 1);
        PlayerPrefs.Save();
        
        StartCoroutine(ScheduleJob((float) twoDaysInSeconds));
        BtnClose();
    }

    public void BackToLogin() {
        SceneManager.LoadScene("Login");
    }

    private void ScheduledJob()
    {
        SceneManager.LoadScene("Login");
    }

    // Coroutine to wait for the specified amount of time
    private IEnumerator ScheduleJob(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        ScheduledJob();
    }
}
