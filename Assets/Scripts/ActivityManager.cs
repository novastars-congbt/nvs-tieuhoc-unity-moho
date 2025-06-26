using Assets.Diamondhenge.HengeVideoPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Video;
//using VInspector.Libs;

public class ActivityManager : MonoBehaviour
{
    public PlayableDirector Timeline;
    public VideoPlayer videoPlayer;
    [SerializeField]
    RawImage rawImage;
    RenderTexture renderTextureInst;
    [SerializeField]
     GameObject[] videoOffline;
    [SerializeField]
     GameObject[] videoOnline;
    GameController gameController;
    private void Awake()
    {
        if (gameController == null) gameController = GameController.instance;
        Debug.LogError("================ " + System.IO.Path.Combine(Application.streamingAssetsPath, gameController.data.typeClasses[gameController.currentClass].name, gameController.data.typeClasses[gameController.currentClass].typeLessons[gameController.currentLesson].name, gameController.data.typeClasses[gameController.currentClass].typeLessons[gameController.currentLesson].typeActives[gameController.currentActivity].name + ".mp4"));
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, gameController.data.typeClasses[gameController.currentClass].name, gameController.data.typeClasses[gameController.currentClass].typeLessons[gameController.currentLesson].name, gameController.data.typeClasses[gameController.currentClass].typeLessons[gameController.currentLesson].typeActives[gameController.currentActivity].name + ".mp4");
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += PlayVideo;
        //StartCoroutine(PlayVideo());
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

    void PlayVideo(VideoPlayer source) {
        Debug.LogError("=============== PlayVideo");
        renderTextureInst = new RenderTexture((int)videoPlayer.width, (int)videoPlayer.height, 32);
        videoPlayer.targetTexture = renderTextureInst;
        rawImage.color = Color.white;
        rawImage.texture = renderTextureInst;
        videoPlayer.Play();
    }

    IEnumerator PlayVideo() {
        yield return null;
        while(!videoPlayer.isPrepared)
        {
            yield return null;
        }
        renderTextureInst = new RenderTexture((int)videoPlayer.width, (int)videoPlayer.height, 32);
        videoPlayer.targetTexture = renderTextureInst;
        rawImage.color = Color.white;
        rawImage.texture = renderTextureInst;
        videoPlayer.Play();
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
