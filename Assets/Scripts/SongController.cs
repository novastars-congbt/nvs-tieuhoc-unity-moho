using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongController : MonoBehaviour
{
    public int index;
    [SerializeField]
     Image frameSong;
    public Text textSong;

    MusicMenuController musicMenuController;
    private void OnEnable()
    {
        if (musicMenuController == null) musicMenuController = MusicMenuController.instance;
        DataParam.actionBtnSong += Display;
        Display();
    }

    private void OnDisable()
    {
        DataParam.actionBtnSong -= Display;
    }

    void Display()
    {
        if (index == musicMenuController.currentSong) frameSong.sprite = musicMenuController.spsFrame[1];
        else frameSong.sprite = musicMenuController.spsFrame[0];
    }

    public void BtnSong()
    {
        musicMenuController.BtnStop();
        musicMenuController.currentSong = index;
        musicMenuController.SetCurrentSong();
        DataParam.actionBtnSong();
    }
}
