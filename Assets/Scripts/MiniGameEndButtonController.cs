using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MiniGameEndButtonController : MonoBehaviour
{
    Canvas canvasDisplay;
    Canvas canvas;
    Canvas canvasEnd;
    MiniGameController miniGameController;
    [Header("Màn hình hiển thị")]
    GameObject displayGameEnd;
    [Header("Cài đặt màn hình chơi game")]
    Button btnBackFromGamePlay;
    [Header("Cài đặt màn hình kết thúc")]
    Button btnHome;
    Button btnNextGame;
    Button btnRestartGame;
    Button btnSkipGame;
    public Action actionNextGame;
    public Action actionRestartGame;
    public Action actionSkipGame;

    private void OnEnable()
    {
        if (miniGameController == null) miniGameController = MiniGameController.instance;
        SetCamera();
        AttachAllObject();
        AddListenerAllButton();
        SetGameEnd();
    }

    private void OnDisable() {
        //miniGameController = null;
    }

    void SetCamera() {
        canvasDisplay = transform.root.FindDeepChild<Canvas>("CanvasDisplay");
        canvas = transform.FindDeepChild<Canvas>("Canvas");
        canvasEnd = transform.FindDeepChild<Canvas>("CanvasEnd");
        Camera camera;
        if (!canvasDisplay) camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        else camera = canvasDisplay.worldCamera;
        if (canvas != null) {
            canvas.worldCamera = camera;
            canvas.sortingOrder = 2;
        }
        if (canvasEnd != null) canvasEnd.worldCamera = camera;
        
    }

    void AttachAllObject()
    {
        AttachObject(ref displayGameEnd, "displayGameEnd");
        AttachObject(ref btnBackFromGamePlay, "btnBackFromGamePlay");
        AttachObject(ref btnHome, "btnHome");
        AttachObject(ref btnNextGame, "btnNextGame");
        AttachObject(ref btnRestartGame, "btnRestartGame");
        AttachObject(ref btnSkipGame, "btnSkipGame");
    }

    void AddListenerAllButton()
    {
        AddListenerButton(ref btnBackFromGamePlay, BtnBackFromGamePlay);
        AddListenerButton(ref btnHome, BtnHome);
        AddListenerButton(ref btnNextGame, BtnNextGame);
        AddListenerButton(ref btnRestartGame, BtnRestartGame);
        AddListenerButton(ref btnSkipGame, BtnSkipGame);
    }

    void AddListenerButton(ref Button button, UnityAction action)
    {
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    void AddListenerButton<T0>(ref Button button, UnityAction<T0> action, T0 value)
    {
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => action(value));
    }

    void AttachObject(ref GameObject obj, string nameObjFind)
    {
        if (obj == null)
        {
            string str1 = nameObjFind.Substring(0, 1);
            string str2 = nameObjFind.Substring(1);
            str1 = str1.ToUpper();
            nameObjFind = str1 + str2;
            obj = transform.FindDeepChild<Transform>(nameObjFind).gameObject;
        }
    }

    void AttachObject(ref Transform obj, string nameObjFind)
    {
        if (obj == null)
        {
            string str1 = nameObjFind.Substring(0, 1);
            string str2 = nameObjFind.Substring(1);
            str1 = str1.ToUpper();
            nameObjFind = str1 + str2;
            obj = transform.FindDeepChild<Transform>(nameObjFind);
        }
    }

    void AttachObject<T>(ref T obj, string nameObjFind) where T : Component
    {
        //if (obj == null)
        //{
        string str1 = nameObjFind.Substring(0, 1);
        string str2 = nameObjFind.Substring(1);
        str1 = str1.ToUpper();
        nameObjFind = str1 + str2;
        Debug.LogError("=================== T " + nameObjFind);
        obj = transform.FindDeepChild<T>(nameObjFind);
        //}
    }

    #region GameStart

    #endregion

    #region GameLevel

    #endregion

    #region GamePlay
    public void ShowGamePlay()
    {
        gameObject.SetActive(true);
        displayGameEnd.SetActive(false);
    }
    public void BtnBackFromGamePlay()
    {
        if (miniGameController == null) return;
        if (miniGameController.haveDisplayGameLevel) miniGameController.ShowGameLevel();
        else if (miniGameController.haveDisplayGameStart) miniGameController.ShowGameStart();
        
    }
    #endregion

    #region GameEnd

    void SetGameEnd()
    {
        btnHome.gameObject.SetActive(miniGameController && (miniGameController.haveDisplayGameStart || miniGameController.haveDisplayGameLevel));
        btnNextGame.gameObject.SetActive(miniGameController && (miniGameController.haveDisplayGameLevel || miniGameController.levels.Length > 1));
        btnSkipGame.gameObject.SetActive(GameController.instance);
    }
    public void BtnHome()
    {
        if (miniGameController == null) return;
        if (miniGameController.haveDisplayGameStart) miniGameController.ShowGameStart();
        else if (miniGameController.haveDisplayGameLevel) miniGameController.ShowGameLevel();
    }

    public void BtnNextGame()
    {
        if (actionNextGame != null)
        {
            Debug.LogError("============= co action next");
            actionNextGame?.Invoke();
        }
        gameObject.SetActive(false);
        if (miniGameController != null)
        {
            miniGameController.currentLevel++;
            Debug.LogError("============= currentLevel " + miniGameController.currentLevel);
            if (miniGameController.currentLevel > miniGameController.levels.Length - 1) miniGameController.currentLevel = 0;
            Debug.LogError("============= currentLevel " + miniGameController.currentLevel);
            miniGameController.ShowGamePlay();
        }
        else ShowGamePlay();
    }

    public void BtnRestartGame()
    {
        actionRestartGame?.Invoke();
        gameObject.SetActive(false);
        if (miniGameController != null) miniGameController.ShowGamePlay();
        else ShowGamePlay();
    }

    public void BtnSkipGame() {
        if (GameController.instance == null) return;
        GameController.instance.BtnForward();
    }
    #endregion
}
