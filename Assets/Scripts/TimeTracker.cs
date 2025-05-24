using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TimeTracker : UIProperties
{
    // Start is called before the first frame update
    private void Start()
    {
        StartJobScheduler();
    }

    private const string expiredTimeKey = "ExpiredTime";

    private void ScheduledJob()
    {
        ShowExpiredPopUp();
    }

    // Coroutine to wait for the specified amount of time
    private IEnumerator ScheduleJob(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        ScheduledJob();
    }

    // Method to start the coroutine
    private void StartJobScheduler()
    {
        if (PlayerPrefs.HasKey(expiredTimeKey))
        {
            long lastimeLogin = Convert.ToInt64(PlayerPrefs.GetString(expiredTimeKey));
            DateTime savedDateTime = new DateTime(lastimeLogin);
            DateTime currentDateTime = DateTime.Now;

            if (savedDateTime > currentDateTime) {
                TimeSpan difference = savedDateTime - currentDateTime;
                double elapsedSeconds = difference.TotalSeconds;
                Debug.Log("Start coroutine");
                StartCoroutine(ScheduleJob((float) elapsedSeconds));
            }
            else {
                ScheduledJob();
            }
        }
    }

}
