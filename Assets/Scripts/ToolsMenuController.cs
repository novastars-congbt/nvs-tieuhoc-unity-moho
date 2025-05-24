using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolsMenuController : MonoBehaviour
{
    [SerializeField]
     Transform TimerMenu;
    [SerializeField]
     Transform MusicMenu;
    [SerializeField]
     Button btnOffBlackScreen;

    bool isOpenMusic = false;
    public void BtnMusicControl()
    {
        if (!isOpenMusic)
        {
            MusicMenu.gameObject.SetActive(true);
            isOpenMusic = true;
        }
        else if (isOpenMusic && !MusicController.instance.soundMusic[MusicMenuController.instance.currentSong].isPlaying)
        {
            MusicMenu.gameObject.SetActive(false);
            isOpenMusic = false;
        }
    }

    bool isOpenTimer = false;
    public void BtnTimer()
    {
        if (!isOpenTimer)
        {
            TimerMenu.gameObject.SetActive(true);
            isOpenTimer = true;
        }
        else
        {
            TimerMenu.gameObject.SetActive(false);
            isOpenTimer = false;
        }
    }

    public void BtnBlackScreen()
    {
        btnOffBlackScreen.gameObject.SetActive(true);
    }

    public void BtnOffBlackScreen()
    {
        btnOffBlackScreen.gameObject.SetActive(false);
    }
}
