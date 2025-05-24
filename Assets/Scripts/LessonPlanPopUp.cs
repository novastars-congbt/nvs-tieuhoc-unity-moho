using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LessonPlanPopUp : PopupProperties
{
    [SerializeField]
     TextMeshProUGUI text;
    [SerializeField]
     Image img;
    public GameObject frame;
    public static string pathTextAsset;
    public static Sprite spr;
    private void OnEnable()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(pathTextAsset);
        text.text = textAsset.text;
        if (spr != null)
        {
            img.sprite = spr;
            img.gameObject.SetActive(true);
        }
        else img.gameObject.SetActive(false);
        


    }

    public override void CloseMe()
    {
        base.CloseMe();
        ListActivityController.instance.currentPlanLesson = -1;
        DataParam.actionBtnPlanLesson();
    }
    
}
