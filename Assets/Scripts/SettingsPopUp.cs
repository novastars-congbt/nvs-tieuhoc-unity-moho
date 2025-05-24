using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPopUp : PopupProperties
{
    GameController gameController;

    private void Awake()
    {
        if (gameController == null) gameController = GameController.instance;
    }
    public void BtnTutorial()
    {
        gameController.BtnTech();
    }

    public void BtnExit()
    {
        gameController.ShowCloseAppPanel();    
    }
}
