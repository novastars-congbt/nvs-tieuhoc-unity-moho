using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PopupProperties : MonoBehaviour
{
    Vector2 scale1 = new Vector2(1f, 1f);
    Vector2 scale2 = new Vector2(0f, 0f);
    public RectTransform rect;
    [HideInInspector]
    public DataController dataController;
    //[HideInInspector]
    //public MusicController soundController;
    //[HideInInspector]
    //public Camera cam;
    //[HideInInspector]
    //public float cameraX;

    private void Awake()
    {
        //soundController = MusicController.instance;
        //dataController = DataController.instance;
    }
    public void SetRect(bool effect)
    {
        if (effect) rect.GetChild(0).localScale = scale2;
        rect.transform.localScale = scale1;
        rect.offsetMin = Vector3.zero;
        rect.offsetMax = Vector3.zero;
        rect.anchorMax = Vector3.one;
        rect.anchorMin = Vector3.zero;
        rect.pivot = Vector3.one / 2;
        rect.localPosition = Vector3.zero;
        //dataController = DataController.instance;
        //soundController = MusicController.instance;
        //cam = Camera.main;
        //cameraX = Screen.height / 2;
        //Debug.Log("-------------------- x  = " + cameraX);
    }

    public virtual void BtnClose()
    {
        gameObject.SetActive(false);
    }

    public virtual void OpenMe()
    {
        gameObject.SetActive(true);   
    }

    public virtual void OpenMe1()
    {
        gameObject.SetActive(true);
        rect.GetChild(0).DOScale(new Vector3(1f, 1f, 0), 0.25f);
    }

    public virtual void CloseMe()
    {
        gameObject.SetActive(false);
    }
}
