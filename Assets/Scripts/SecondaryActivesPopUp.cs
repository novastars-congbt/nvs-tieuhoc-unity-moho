using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryActivesPopUp : MonoBehaviour
{
    GameController gameController;
    private void Awake()
    {
        if (gameController == null) gameController = GameController.instance;
    }
    public void BtnChooseSecondaryActivity(int index) {
        gameController.indexActivityInSecondary[gameController.IndexSecondaryActivity()] = index;
        //gameController.secondaryActivities.Find(x => x.idActive == gameController.currentActivity).indexCurrentActivity = index;
        gameController.BtnForward();
    }
}
