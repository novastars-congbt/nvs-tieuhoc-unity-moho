using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    [SerializeField]
     string videoFileName;
    [SerializeField]
     VideoPlayer videoPlayer;

    private void Start() {
        PlayVideo();
    }

    public void PlayVideo() {
        if (videoPlayer) {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            videoPlayer.url = videoPath;
            videoPlayer.Play();
        }
    }
}
