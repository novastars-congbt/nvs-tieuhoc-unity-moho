using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BellController : MonoBehaviour
{
    [SerializeField]
     int index;
    //[SerializeField]
    // Image frameBell;
    private void OnEnable()
    {
        DataParam.actionBtnBell += Display;
        Display();
    }

    private void OnDisable()
    {
        DataParam.actionBtnBell -= Display;
    }

    void Display()
    {
        //if (index == TimerMenuController.instance.currentBell) frameBell.sprite = TimerMenuController.instance.spsFrame[1];
        //else frameBell.sprite = TimerMenuController.instance.spsFrame[0];
    }

    public void BtnBell()
    {
        TimerMenuPopUp.instance.currentBell = index;
        DataParam.actionBtnBell();
    }
}
