using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseAppPopUp : PopupProperties
{
    public void BtnYes()
    {
        Application.Quit();
    }

    public void BtnNo()
    {
        rect.GetChild(0).DOScale(new Vector3(0f, 0f), 0.25f).OnComplete(() => {
            base.CloseMe();
            SetRect(true);
        });
    }
}
