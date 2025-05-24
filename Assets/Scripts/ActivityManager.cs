using Assets.Diamondhenge.HengeVideoPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
//using VInspector.Libs;

public class ActivityManager : MonoBehaviour
{
    public PlayableDirector Timeline;
    [SerializeField]
     GameObject[] videoOffline;
    [SerializeField]
     GameObject[] videoOnline;
    GameController gameController;

    private void Awake()
    {
        if (gameController == null) gameController = GameController.instance;
        //SetOnlyEvent();
        for (int i = 0; i < videoOffline.Length; i++)
        {
            videoOffline[i].SetActive(false);
        }

        for (int i = 0; i < videoOnline.Length; i++)
        {
            videoOnline[i].SetActive(false);
        }

        if (gameController != null) gameController.videos.Clear();

        for (int i = 0; i < videoOffline.Length; i++)
        {
            if (gameController != null)
            {
                if (!gameController.isWebGL)
                {
                    gameController.videos.Add(videoOffline[i].GetComponent<HengeVideo>());
                    videoOffline[i].SetActive(true);
                }
            }
            else
            {
#if !UNITY_WEBGL
                 
                videoOffline[i].SetActive(true);
#endif
            }
        }



        for (int i = 0; i < videoOnline.Length; i++)
        {
            if (gameController != null)
            {
                if (gameController.isWebGL)
                {
                    gameController.videos.Add(videoOnline[i].GetComponent<HengeVideo>());
                    videoOnline[i].SetActive(true);
                }
            }
            else
            {
#if UNITY_WEBGL
                videoOnline[i].SetActive(true);
#endif
            }

        }

    }

    //void SetOnlyEvent()
    //{
    //    EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
    //    AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();
    //    if (eventSystems.Length > 1)
    //    {
    //        for (int i = 0; i < eventSystems.Length; i++)
    //        {
    //            if (eventSystems[i].transform.parent != null) eventSystems[i].enabled = false;
    //            else eventSystems[i].enabled = true;
    //        }
    //    }

    //    if (audioListeners.Length > 1)
    //    {
    //        for (int i = 0; i < audioListeners.Length; i++)
    //        {
    //            if (audioListeners[i].transform.parent != null) audioListeners[i].enabled = false;
    //            else audioListeners[i].enabled = true;
    //        }
    //    }
    //}

    public void BtnChooseSecondaryActivity(int index)
    {
        gameController.indexActivityInSecondary[gameController.IndexSecondaryActivity()] = index;
        //gameController.secondaryActivities.Find(x => x.idActive == gameController.currentActivity).indexCurrentActivity = index;
        gameController.BtnForward();
    }
}
