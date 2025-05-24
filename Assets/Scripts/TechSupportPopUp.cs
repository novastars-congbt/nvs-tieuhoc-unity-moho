using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechSupportPopUp : PopupProperties
{
    public static Sprite spr;
    [SerializeField]
     Image img;

    private void OnEnable()
    {
        img.sprite = spr;
    }

    public new void BtnClose()
    {
        rect.GetChild(0).DOScale(new Vector3(0f, 0f), 0.25f).OnComplete(() => {
            base.CloseMe();
            SetRect(true);
        });
    }
}
