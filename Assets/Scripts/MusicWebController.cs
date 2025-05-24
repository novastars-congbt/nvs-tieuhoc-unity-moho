using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicWebController : MonoBehaviour
{
    [SerializeField]
    Text textMusic;
    [SerializeField]
    Text textTime;
    string url;

    public void SetMusic(string strMusic, string strTime, string urlMusic)
    {
        textMusic.text = "Link bài nhạc:\n" + strMusic;
        textTime.text = strTime;
        url = urlMusic;
    }

    public void BtnPlayMusic()
    {
        Application.OpenURL(url);
    }
}
