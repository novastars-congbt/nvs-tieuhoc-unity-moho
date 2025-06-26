using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static UnityEngine.InputSystem.InputAction;
using System;
using Assets.Diamondhenge.HengeVideoPlayer;
using UnityEngine.Video;
// using CrazyMinnow.SALSA;
// using VInspector.Libs;

public class GameController : UIProperties
{
    public static GameController instance;
    public Data data;
    public MusicController musicController;
    public bool isWebGL = false;
    [SerializeField]
    bool debug = false;

    public int currentClass;
    public int currentLesson;
    public int currentActivity;
    public int currentStep;
    public int currentSlide;

    [SerializeField]
    float valueZoomDefault;
    float valueZoom = 1f;
    //int currentTimePause;

    public bool isSecondaryActivity = false;

    [SerializeField]
     ListActivityController listActivity;
    public ListStepController listStep;
    [SerializeField]
    AudioController sound;
    [SerializeField]
    MusicPlayController musicPlayController;
    [SerializeField]
    MusicWebController musicWebController;
    [SerializeField]
     Transform tools;

    [SerializeField]
     Slider sliderTimeVideo;
    [SerializeField]
     Slider sliderFakeTimeVideo;
    [SerializeField]
    Slider sliderZoom;

    [SerializeField]
     TextMeshProUGUI textLesson;
    [SerializeField]
     TextMeshProUGUI textCurrentActivity;
    //[SerializeField]
    // TextMeshProUGUI textTimeVideo;
    //[SerializeField]
    // TextMeshProUGUI textElapsedTime;
    [SerializeField]
     TextMeshProUGUI textBtnForward;

    [SerializeField]
    Button btnClose;
    [SerializeField]
     Button btnPlayPause;
    [SerializeField]
     Button btnLock;
    [SerializeField]
     Button btnLocked;
    [SerializeField]
     Button btnCloseActivity;
    [SerializeField]
     Button btnForward;
    [SerializeField]
     Button btnForwardLong;
    [SerializeField]
     Button btnBack;
    [SerializeField]
    Texture2D[] textureMouse;
    [SerializeField]
    InputActionAsset PlayerInput;
    Action actionBtnOpen;
    Action actionBtnClose;
    public PlayableDirector currentTimeline;
    public VideoPlayer currentVideoPlayer;

    public Data.TypeClass.TypeLesson.TypeActive typeActive;
    ActivityManager _activity;
    //[SerializeField]
    //List <ActivityManager> activities = new List<ActivityManager>();
    //public List<SecondaryActivityManager> secondaryActivities = new List<SecondaryActivityManager>();

    [SerializeField]
    Sprite[] sprTech;
    public Sprite[] sprPlayPause;
    [SerializeField]
    List<Transform> timeLineStop = new List<Transform>();
    List<float> timePause = new List<float>();
    public List<int> indexActivityInSecondary = new List<int>();

    public List<HengeVideo> videos = new List<HengeVideo>();

    DOTweenAnimation tween1;
    DOTweenAnimation tween2;

    public class SecondaryActivityManager
    {
        public int idActive;
        public int indexCurrentActivity;
        public ActivityManager activity;
    }
    private void Awake()
    {
        
        //Application.targetFrameRate = 30;
        Debug.unityLogger.logEnabled = debug;
        Input.multiTouchEnabled = false;
        if (instance == null) instance = this;
        SetInputAction();
        if (!isWebGL)
        {
            data = DataController.instance.data;
            if (musicController == null) musicController = MusicController.instance;
            currentClass = MenuController.currentClass;
            currentLesson = MenuController.currentLesson;
            btnClose.gameObject.SetActive(true);
        }
        else
        {
            btnClose.gameObject.SetActive(false);
        }
        textLesson.text = "Lớp " + (currentClass + 1) + ": " + data.typeClasses[currentClass].typeLessons[currentLesson].text;
        //Debug.LogError("======================== second = " + data.typeClasses[currentClass].typeLessons[currentLesson].typeSecondaryActives.Length);
        //Debug.LogError("======================== second2 = " + indexActivtyInSecondary.Count);
        for (int i = 0; i < data.typeClasses[currentClass].typeLessons[currentLesson].typeSecondaryActives.Length; i++) indexActivityInSecondary.Add(0);
        isSecondaryActivity = false;
        tween1 = btnForward.GetComponent<DOTweenAnimation>();
        tween2 = btnForwardLong.GetComponent<DOTweenAnimation>();
        //CreateListActivity();
        CreateActivity();
        SetCurrentActivity();
        SetSlider();
        //SetTimeLineStop();
        listActivity.OnAwake();
        listStep.OnAwake();
    }

    ActivityManager activity;
    //SecondaryActivityManager secondaryActivity;
    string pathTemp;

    //bool delayKeySpace;
    //bool delayKeyPageUp;
    //bool delayKeyPageDown;

    void OnApplicationQuit()
    {
        PlayPause.performed -= ActionPlayPause;
        Back.performed -= ActionBack;
        Forward.performed -= ActionForward;
        OnOffSound.performed -= ActionSound;
        OpenClose.performed -= ActionOpenClose;
        PlusSound.performed -= ActionPlusSound;
        MinusSound.performed -= ActionMinusSound;
        Zoom.performed -= ActionZoom;
    }

    private void FixedUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.Space) && !delayKeySpace)
        //{
        //    delayKeySpace = true;
        //    BtnPause();
        //    StartCoroutine(DelayKeySpace());
        //}
        //if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.PageUp)) && !delayKeyPageUp)
        //{
        //    delayKeyPageUp = true;
        //    BtnForward();
        //    StartCoroutine(DelayKeyUp());
        //}
        //if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.PageDown)) && !delayKeyPageDown)
        //{
        //    delayKeyPageDown = true;
        //    BtnBack();
        //    StartCoroutine(DelayKeyDown());
        //}
        
        if (currentTimeline != null && currentVideoPlayer != null) {
            //Debug.LogErrorFormat("================= {0}, {1}", currentVideoPlayer.time, currentTimeline.time);
            if (Mathf.Abs((float)currentVideoPlayer.time - (float)currentTimeline.time) > 0.1f) {
                //Debug.LogError("================= set time");
                currentTimeline.time = currentVideoPlayer.time;
            }
        }
        if (/*currentTimeline != null &&*/ currentVideoPlayer != null && !replay) SetSliderAccordingToTime();
        if (sliderZoom.value > 1 && !IsPointerOverMainCanvas()) {
            if (Input.GetMouseButton(0)) Cursor.SetCursor(textureMouse[1], Vector2.zero, CursorMode.ForceSoftware);
            else Cursor.SetCursor(textureMouse[0], Vector2.zero, CursorMode.ForceSoftware);
        }
        else Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
    }

    InputActionMap Teacher;
    InputAction PlayPause;
    InputAction Back;
    InputAction Forward;
    InputAction OnOffSound;
    InputAction OpenClose;
    InputAction PlusSound;
    InputAction MinusSound;
    InputAction Zoom;

    void SetInputAction()
    {
        Teacher = PlayerInput.FindActionMap("Teacher");
        PlayPause = Teacher.FindAction("PlayPause");
        Back = Teacher.FindAction("Back");
        Forward = Teacher.FindAction("Forward");
        OnOffSound = Teacher.FindAction("OnOffSound");
        OpenClose = Teacher.FindAction("OpenClose");
        PlusSound = Teacher.FindAction("PlusSound");
        MinusSound = Teacher.FindAction("MinusSound");
        Zoom = Teacher.FindAction("Zoom");
        PlayPause.Enable();
        Back.Enable();
        Forward.Enable();
        OnOffSound.Enable();
        OpenClose.Enable();
        PlusSound.Enable();
        MinusSound.Enable();
        Zoom.Enable();
        if (PlayPause != null) Debug.LogError("=================== co playpause");
        PlayPause.performed += ActionPlayPause;
        Back.performed += ActionBack;
        Forward.performed += ActionForward;
        OnOffSound.performed += ActionSound;
        OpenClose.performed += ActionOpenClose;
        PlusSound.performed += ActionPlusSound;
        MinusSound .performed += ActionMinusSound;
        Zoom.performed += ActionZoom;
    }

    void ActionPlayPause(CallbackContext context)
    {
        BtnPause(); 
    }

    void ActionBack(CallbackContext context)
    {
        if (!btnBack.interactable) return;
        BtnBack();
    }

    void ActionForward(CallbackContext context)
    {
        if (!btnForward.interactable || !btnForwardLong.interactable) return;
        BtnForward();
    }

    bool isHaveSound = true;
    void ActionSound(CallbackContext context)
    {
        if (isHaveSound)
        {
            Debug.LogError("sound off");
            isHaveSound = false;
            sound.On();
        }
        else
        {
            Debug.LogError("sound on");
            isHaveSound = true;
            sound.Off();
        }
    }
    // Debug panel methods
    public void SimulateKeyPress(string key)
    {
        switch(key)
        {
            case "ArrowLeft":
                ActionBack(new CallbackContext());
                break;
            case "ArrowRight":
                ActionForward(new CallbackContext());
                break;
            case "Space":
                ActionPlayPause(new CallbackContext());
                break;
            case "End":
                // Skip to end logic
                break;
            case "Home":
                // Return to start logic  
                break;
        }
    }

    public void SetGameSpeed(float speed)
    {
        Time.timeScale = speed;
    }

    void ActionOpenClose(CallbackContext context)
    {
        if (btnOpenPanel == null) return;
        Debug.LogError("============== action");
        if (objPanel == null || !objPanel.gameObject.activeInHierarchy)
        {
            Debug.LogError("============== open");
            btnOpenPanel.onClick.Invoke();
            //Invoke("SetBtnClosePanel", 0.1f);
        }
        else
        {
            Debug.LogError("============== close");
            btnClosePanel.onClick.Invoke();
        }
    }

    void ActionPlusSound(CallbackContext context)
    {
        sound.PlusVolume();
    }

    void ActionMinusSound(CallbackContext context) 
    { 
        sound.MinusVolume(); 
    }

    void ActionZoom(CallbackContext context)
    {
        if (!sliderZoom.transform.parent.gameObject.activeSelf) sliderZoom.transform.parent.gameObject.SetActive(true);
        if (sliderZoom.value < valueZoomDefault) SetZoom(valueZoom);
        else SetZoom(1);
    }

    bool IsPointerOverMainCanvas()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        if (results.Find(x => x.gameObject.transform.root.CompareTag("MainCanvas")).gameObject != null) return true;
        return false;
    }

    //IEnumerator DelayKeySpace()
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    delayKeySpace = false;
    //}

    //IEnumerator DelayKeyUp()
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    delayKeyPageUp = false;
    //}

    //IEnumerator DelayKeyDown()
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    delayKeyPageDown = false;
    //}

    //void CreateListActivity()
    //{
    //    for (int i = 0; i < data.typeClasses[currentClass].typeLessons[currentLesson].typeActives.Length; i++)
    //    {
    //        activities.Add(new ActivityManager());
    //    }
        
    //    for (int i = 0; i < data.typeClasses[currentClass].typeLessons[currentLesson].typeSecondaryActives.Length; i++)
    //    {
    //        if (data.typeClasses[currentClass].typeLessons[currentLesson].typeSecondaryActives[i].typeActives.Length > 0) secondaryActivities.Add(new SecondaryActivityManager());
    //        secondaryActivities[secondaryActivities.Count - 1].idActive = data.typeClasses[currentClass].typeLessons[currentLesson].typeSecondaryActives[i].idActive;
    //    }
    //}

    int indexCurrentSecondaryActivity;

    public void CreateActivity()
    {
        if (_activity != null) Destroy(_activity.gameObject);
        //for (int i = 0; i < activities.Count; i++)
        //{
        //    if (activities[i] != null) activities[i].gameObject.SetActive(false);
        //}

        //for (int i = 0; i < secondaryActivities.Count; i++)
        //{
        //    if (secondaryActivities[i].activity != null) secondaryActivities[i].activity.gameObject.SetActive(false);
        //}

        if (!isSecondaryActivity) {
            typeActive = data.typeClasses[currentClass].typeLessons[currentLesson].typeActives[currentActivity];
            //if (activities[currentActivity] == null)
            //{
            //    pathTemp = "Prefabs/" + data.typeClasses[currentClass].name + "/" + data.typeClasses[currentClass].typeLessons[currentLesson].name + "/" + typeActive.name;
            //    activity = Resources.Load<ActivityManager>(pathTemp);
            //    activities[currentActivity] = Instantiate(activity);
            //}
            //activities[currentActivity].gameObject.SetActive(true);
            //currentTimeline = activities[currentActivity].Timeline;
        }
        else
        {
            //indexCurrentSecondaryActivity = secondaryActivities.FindIndex(x => x.idActive == currentActivity);
            //typeActive = data.typeClasses[currentClass].typeLessons[currentLesson].typeSecondaryActives[indexCurrentSecondaryActivity].typeActives[secondaryActivities[indexCurrentSecondaryActivity].indexCurrentActivity];
            //if (secondaryActivities[indexCurrentSecondaryActivity].activity == null)
            //{
            //    pathTemp = "Prefabs/" + data.typeClasses[currentClass].name + "/" + data.typeClasses[currentClass].typeLessons[currentLesson].name + "/" + typeActive.name;
            //    activity = Resources.Load<ActivityManager>(pathTemp);
            //    secondaryActivities[indexCurrentSecondaryActivity].activity = Instantiate(activity);
            //}
            //secondaryActivities[indexCurrentSecondaryActivity].activity.gameObject.SetActive(true);
            //currentTimeline = secondaryActivities[indexCurrentSecondaryActivity].activity.Timeline;
            int indexSecondaryActivity = IndexSecondaryActivity();
            typeActive = data.typeClasses[currentClass].typeLessons[currentLesson].typeSecondaryActives[indexSecondaryActivity].typeActives[indexActivityInSecondary[indexSecondaryActivity]];
        }

        pathTemp = "Prefabs/" + data.typeClasses[currentClass].name + "/" + data.typeClasses[currentClass].typeLessons[currentLesson].name + "/" + typeActive.name;
        Debug.LogError("================= " + pathTemp);
        activity = Resources.Load<ActivityManager>(pathTemp);
        _activity = Instantiate(activity);
        if (_activity.Timeline != null && _activity.Timeline.playableAsset != null) currentTimeline = _activity.Timeline;
        currentVideoPlayer = _activity.videoPlayer;
        if (currentVideoPlayer != null) currentVideoPlayer.prepareCompleted += SetTimeVideo;
        content = _activity.transform.FindDeepChild<Transform>("Content");
        Debug.LogError("============ content = " + content);
    }
    public int IndexSecondaryActivity()
    {
        for (int i = 0; i < data.typeClasses[currentClass].typeLessons[currentLesson].typeSecondaryActives.Length; i++)
        {
            if (data.typeClasses[currentClass].typeLessons[currentLesson].typeSecondaryActives[i].idActive == currentActivity) return i;
        }
        return -1;
    }

    public void SetCurrentActivity()
    {
        //currentTimeline.time = 0;
        //pause = false;
        //pauseTimeLine = false;
        replay = false;
        PlayDotweenBtnForward();
        //if (currentStep == 0 && currentSlide == 0) pause = false;
        //else pause = true;
        //pause = false;
        //Invoke("BtnPause", 0.1f);
        //currentTimeline = _activity.Timeline;
        SetTextSoLong(textCurrentActivity, typeActive.text, 28);
#if UNITY_WEBGL && !UNITY_EDITOR
        SendDebugSceneInfo();
#endif
    }

    // Debug: Send camera and audio info to JS
    public void SendDebugSceneInfo()
    {
        // Camera info
        var cameras = GameObject.FindObjectsOfType<Camera>();
        List<string> cameraNames = new List<string>();
        foreach (var cam in cameras)
        {
            if (cam.enabled)
                cameraNames.Add(cam.name + (cam.tag == "MainCamera" ? " (Main)" : ""));
        }

        // Audio info
        var audios = GameObject.FindObjectsOfType<AudioSource>();
        List<string> audioNames = new List<string>();
        foreach (var audio in audios)
        {
            if (audio.enabled && audio.isPlaying)
                audioNames.Add(audio.gameObject.name + $" (vol: {audio.volume:0.00})");
        }

        string cameraList = string.Join("|", cameraNames);
        string audioList = string.Join("|", audioNames);

#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalCall("updateDebugSceneInfo", cameraList, audioList);
#endif
        //textCurrentActivity.text = typeActive.text;
        
        //if (currentTimeline != null)
        //{
            //SetTimePause();
            //btnPause.gameObject.SetActive(true);
            //sliderTimeVideo.gameObject.SetActive(true);
            //textTimeVideo.gameObject.SetActive(true);
            /*if (currentStep == 0)*/ /*sliderTimeVideo.maxValue = typeActive.time;*/
            //else sliderTimeVideo.maxValue = typeActive.typeSteps[currentStep].time - typeActive.typeSteps[currentStep - 1].time;
            //sliderFakeTimeVideo.maxValue = sliderTimeVideo.maxValue;
            //sliderTimeVideo.value = 0;

            //currentTimePause = 0;
            /*if (currentStep == 0)*/ /*currentTimeline.time = 0;*/
            //else currentTimeline.time = typeActive.typeSteps[currentStep - 1].time;
            //SetTextTimeVideo(textTimeVideo, typeActive.time);
            //SetTimeLineStop();
        //}
        //else
        //{
            //btnPause.gameObject.SetActive(false);
            //sliderTimeVideo.gameObject.SetActive(false);
            //textTimeVideo.gameObject.SetActive(false);
        //}
        
        
        //textTimeVideo.text = "" + (int)typeActive.time / 60 + ":" + (int)typeActive.time % 60;
    }

    public void SetTextSoLong(TextMeshProUGUI text, string str, int length) 
    {
        if (str.Length < length) text.text = str;
        else
        {
            string newStr;

            do
            {
                newStr = str.Substring(0, length);
                Debug.Log(newStr);
                length--;
            } while (!newStr.EndsWith(" "));
            newStr = str.Substring(0, length);
            text.text = newStr + "...";
        }  
    }

    bool delaySetTimePause = false;
    bool isSetSliderAccordingToTime = true;
    bool isOpen = false;
    Transform objBtnOpenPanel;
    Transform objBtnClosePanel;
    Transform objPanel;
    Button btnOpenPanel;
    Button btnClosePanel;
    public void SetSlider()
    {
        //actionBtnOpen = null;
        //actionBtnClose = null;
        //activity.transform.Find("Btn_Open").GetComponent<Button>().onClick.Invoke();
        //foreach (Transform transform in activity.transform)
        //{
        //    if (transform.name == "Btn_Open")
        //    {
        //        Debug.LogError("========================");
        //        break;
        //    }
        //}
        CancelInvoke("SetOpenClosePanel");
        CancelInvoke("SetOnlyEvent");
        if (sliderZoom.transform.parent.gameObject.activeSelf) sliderZoom.transform.parent.gameObject.SetActive(false);
        SetZoom(1f);
        sliderZoom.value = 1f;
        valueZoom = valueZoomDefault;
        if (typeActive.typeSteps[currentStep].typeSlides[currentSlide].musicPlay.audioClipMusic != null) musicPlayController.SetMusic(typeActive.typeSteps[currentStep].typeSlides[currentSlide].musicPlay.audioClipMusic, typeActive.typeSteps[currentStep].typeSlides[currentSlide].musicPlay.loopMusic);
        if (typeActive.typeSteps[currentStep].typeSlides[currentSlide].musicWeb.urlMusic != null && typeActive.typeSteps[currentStep].typeSlides[currentSlide].musicWeb.urlMusic.Length > 0) musicWebController.SetMusic(typeActive.typeSteps[currentStep].typeSlides[currentSlide].musicWeb.nameMusic, typeActive.typeSteps[currentStep].typeSlides[currentSlide].musicWeb.timeMusic, typeActive.typeSteps[currentStep].typeSlides[currentSlide].musicWeb.urlMusic);
        delaySetTimePause = true;
        SetTimePause();
        // CancelInvoke("DelaySetTimeVideo");
        if (currentVideoPlayer.isPrepared) {
            currentVideoPlayer.time = timePause[0];
            if (currentTimeline != null) currentTimeline.time = timePause[0];
        }
        // else Invoke("DelaySetTimeVideo", 1f);
        //if (typeActive.typeSteps[currentStep].typeSlides[currentSlide].isCartoon)
        //{
        sliderTimeVideo.gameObject.SetActive(true);
            sliderTimeVideo.maxValue = timePause[1] - timePause[0];
            sliderTimeVideo.value = 0;
        SetBtnBackNForward();
        
        //}
        //else
        //{
        //    sliderTimeVideo.gameObject.SetActive(false);
        //}
        delaySetTimePause = false;
        replay = false;
        if (pause)
        {
            BtnPause();
        }
        PlayDotweenBtnForward();
        Invoke("SetOpenClosePanel", 0.1f);
        Invoke("SetOnlyEvent", 0.1f);
    }

    void SetTimeVideo(VideoPlayer source) {
        Debug.LogError("=============== SetTimeVideo");
        currentVideoPlayer.time = timePause[0];
        if (currentTimeline != null) {
            currentTimeline.time = timePause[0];
            currentTimeline.Play();
        } 
        //currentVideoPlayer.Play();
    }

    public void SetVolumeSound(float volume)
    {
        sound.SetVolume(volume);
        for (int i = 0; i < videos.Count; i++)
        {
            videos[i].SetVolume(volume);
        }
    }

    void SetOnlyEvent()
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();
        if (eventSystems.Length > 1)
        {
            for (int i = 0; i < eventSystems.Length; i++)
            {
                if (eventSystems[i].transform.parent != null) eventSystems[i].enabled = false;
                else eventSystems[i].enabled = true;
            }
        }

        if (audioListeners.Length > 1)
        {
            for (int i = 0; i < audioListeners.Length; i++)
            {
                if (audioListeners[i].transform.parent != null) audioListeners[i].enabled = false;
                else audioListeners[i].enabled = true;
            }
        }
    }

    void SetPanel()
    {
        if (objPanel != null) return;
        objPanel = (_activity.transform.FindDeepChild<Transform>("Panel_Preparation") != null) ? _activity.transform.FindDeepChild<Transform>("Panel_Preparation") : _activity.transform.FindDeepChild<Transform>("Panel_LessonPlan");
    }

        void SetOpenClosePanel()
    {
        objBtnOpenPanel = null;
        objPanel = null;
        objBtnClosePanel = null;
        btnOpenPanel = null;
        btnClosePanel = null;
        objBtnOpenPanel = _activity.transform.FindDeepChild<Transform>("Btn_Open");
        objBtnClosePanel = _activity.transform.FindDeepChild<Transform>("Btn_Close");
        objPanel = (_activity.transform.FindDeepChild<Transform>("Panel_Preparation") != null)? _activity.transform.FindDeepChild<Transform>("Panel_Preparation") : _activity.transform.FindDeepChild<Transform>("Panel_LessonPlan");
        if (objBtnOpenPanel != null)
        {
            btnOpenPanel = objBtnOpenPanel.GetComponent<Button>();
            btnOpenPanel.onClick.AddListener(SetPanel);
            //btnOpenPanel.onClick.AddListener(SetBtnClosePanel);
        }
        if (objBtnClosePanel != null) btnClosePanel = objBtnClosePanel.GetComponent<Button>();
        Debug.LogError("========================== btnopen = " + btnOpenPanel);
        Debug.LogError("========================== btnclose = " + btnClosePanel);
        Debug.LogError("========================== panel = " + objPanel);
    }

    IEnumerator DelayBtnForwardLong()
    {
        btnForwardLong.transform.GetComponent<ContentSizeFitter>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        btnForwardLong.transform.GetComponent<ContentSizeFitter>().enabled = true;
    }

    public string SetTextTimeVideo(float time)
    {
        string str;
        int minute = (int)time / 60;
        int second = (int)time % 60;
        if (second < 10)
        {
            str = "" + minute + ":0" + second;
        }
        else
        {
            str = "" + minute + ":" + second;
        }
        return str;
    }

    bool pauseTimeLine = false;
    float timeDelayShowTimeVideo;
    public void SetSliderAccordingToTime()
    {
        //Debug.LogErrorFormat("============= {0}, {1}, {2}, {3}", delayChangeSlider, delaySetTimePause, isTimeAccordingToSlider, currentVideoPlayer.time);
        timeDelayShowTimeVideo += Time.deltaTime;
        if (timeDelayShowTimeVideo >= 0.5f) {
            Debug.LogError("============== timevideo = " + currentVideoPlayer.time);
            timeDelayShowTimeVideo = 0;
        }
        if (!isSetSliderAccordingToTime) return;
        if (delaySetTimePause) return;
        if (sliderTimeVideo.gameObject.activeSelf) sliderTimeVideo.value = (float)currentVideoPlayer.time/*currentTimeline.time*/ - timePause[0];
        /*if (currentStep == 0)*/
        //sliderTimeVideo.value = (float)currentTimeline.time;
        //else sliderTimeVideo.value = (float)currentTimeline.time - typeActive.typeSteps[currentStep - 1].time;
        //Debug.LogError("======================== slider = " + sliderTimeVideo.value);
        //if (sliderTimeVideo.value >= sliderTimeVideo.maxValue)
        //{
        //    pause = true;
        //    currentTimeline.Pause();
        //}
        //else 
        //{
        //    if (pause)
        //    {
        //        pause = false;
        //        currentTimeline.Resume();
        //    }
        //}
        //SetTextTimeVideo(textElapsedTime, (float)currentTimeline.time);
        //if (currentTimeline.time > 0) btnPause.gameObject.SetActive(true);
        //else btnPause.gameObject.SetActive(false);
        //for (int i = 0; i < typeActive.typeSteps.Length; i++)
        //{
        //    if (currentTimeline.time < typeActive.typeSteps[i].time)
        //    {
        //        currentStep = i;
        //        listStep.SetListStep();
        //        break;
        //    }
        //}

        //for (int i = 0; i < timePause.Count; i++)
        //{
            //Debug.LogError("=============================== dcmm");
            float time;
            /*if (currentStep == 0)*/ time = (float)currentVideoPlayer.time/*currentTimeline.time*/;
        //else time = (float) currentTimeline.time - typeActive.typeSteps[currentStep - 1].time;
        //if (((i > 0 && i < timePause.Count - 1 && time >= timePause[i] && time - timePause[i] < 0.1f) || (i == timePause.Count - 1 && time >= sliderTimeVideo.maxValue)) && !pause && !pauseTimeLine)
        //{
        //    currentTimePause = i;
        //    BtnPause();
        //    return;
        //}
        if (time >= timePause[1])
        {
            if (!pause) BtnPause();
            replay = true;
            PlayDotweenBtnForward();
            btnPlayPause.transform.GetComponent<Image>().sprite = sprPlayPause[2];

            if (typeActive.typeSteps[currentStep].typeSlides[currentSlide].musicPlay.audioClipMusic != null) musicPlayController.gameObject.SetActive(true);
            else musicPlayController.gameObject.SetActive(false);
            if (typeActive.typeSteps[currentStep].typeSlides[currentSlide].musicWeb.urlMusic != null && typeActive.typeSteps[currentStep].typeSlides[currentSlide].musicWeb.urlMusic.Length > 0 && !musicPlayController.gameObject.activeSelf) musicWebController.gameObject.SetActive(true);
            else musicWebController.gameObject.SetActive(false);
        }
        else
        {
            musicPlayController.gameObject.SetActive(false);
            musicWebController.gameObject.SetActive(false);
        }

        //}

        //for (int i = 0; i < timePause.Count; i++)
        //{
        //    if ((float)currentTimeline.time >= timePause[i]) currentTimePause = i;
        //    else return;
        //}
    }

    public void SetTimeAccordingToSlider(/*float value*/)
    {
        //Debug.LogError("============== SetTimeAccordingToSlider");
        if (delaySetTimePause) return;
        if (!sliderTimeVideo.gameObject.activeSelf) return;
        if (sliderTimeVideo.value >= sliderTimeVideo.maxValue) sliderTimeVideo.value = sliderTimeVideo.maxValue;
        //float time;
        /*if (currentStep == 0)*/
        //time = Mathf.Abs(value - (float)currentTimeline.time);
        //time = Mathf.Abs(value + timePause[0] - (float)currentTimeline.time);
        //else time = Mathf.Abs(value + typeActive.typeSteps[currentStep - 1].time - (float)currentTimeline.time);
        //if (time < 0.5f) return;
        //Debug.LogError("==================== vclol luon");
        CancelInvoke("Pause");
        Debug.LogErrorFormat("============== valueSlider = {0}, timePause[0] = {1}", sliderTimeVideo.value, timePause[0]);
        currentVideoPlayer.time = sliderTimeVideo.value + timePause[0];
        if (currentTimeline != null) currentTimeline.time = sliderTimeVideo.value + timePause[0];
        if (!replay)
        {
            pause = true;
            //replay = false;
            Pause();
            Invoke("Pause", 0.5f);
        }
        PlayDotweenBtnForward();
        
        /*if (currentStep == 0)*/
        //currentTimeline.time = value;
        //else currentTimeline.time = typeActive.typeSteps[currentStep - 1].time + value;
        //for (int i = 0; i < typeActive.typeSteps.Length; i++)
        //{
        //    if (currentTimeline.time < typeActive.typeSteps[i].time) {
        //        currentStep = i;
        //        listStep.SetListStep();
        //        break;
        //    }
        //}
    }

    public void IsNotSetSliderAccordingToTime()
    {
        isSetSliderAccordingToTime = false;
    }

    public void IsSetSliderAccordingToTime()
    {
        isSetSliderAccordingToTime = true;
    }

    public void DelayIsSetSliderAccordingToTime() {
        CancelInvoke("IsSetSliderAccordingToTime");
        Invoke("IsSetSliderAccordingToTime", 0.5f);
    }

    public void SetTimeLineStop()
    {
        foreach(var timeLine in timeLineStop)
        {
            timeLine.gameObject.SetActive(false);
        }

        StartCoroutine(DelayTimeLineStop());

        //for (int i = 0; i < data.typeClasses[currentClass].typeLessons[currentClass].typeActives[currentActivity].timeline.Length; i++)
        //{
        //    sliderFakeTimeVideo.value = data.typeClasses[currentClass].typeLessons[currentClass].typeActives[currentActivity].timeline[i];
        //    timeLineStop[i].position = /*new Vector2(sliderFakeTimeVideo.handleRect.position.x, timeLineStop[i].position.y)*/ sliderFakeTimeVideo.handleRect.position;
        //    timeLineStop[i].gameObject.SetActive(true);
        //}
    }

    public void SetTimePause()
    {
        timePause.Clear();
        if (!data.typeClasses[currentClass].typeLessons[currentLesson].doneFixTimeline) timePause.Add(typeActive.typeSteps[currentStep].typeSlides[currentSlide].time);
        else
        {
            if (typeActive.typeSteps[currentStep].typeSlides[currentSlide].time == 0) timePause.Add(0);
            else timePause.Add(typeActive.typeSteps[currentStep].typeSlides[currentSlide].time + 1);
        }
        if (currentSlide == typeActive.typeSteps[currentStep].typeSlides.Length - 1)
        {
            timePause.Add(typeActive.typeSteps[currentStep].time);
        }
        else
        {
            timePause.Add(typeActive.typeSteps[currentStep].typeSlides[currentSlide + 1].time);
        }
        //timePause.Add(0);
        //float time;
        //if (currentStep == 0) time = 0;
        //else time = typeActive.typeSteps[currentStep - 1].time;
        //for (int i = 0; i < typeActive.typeSteps[currentStep].timePause.Length; i++)
        //for (int i = 0; i < typeActive.timePause.Length; i++)
        //{
        //timePause.Add(typeActive.typeSteps[currentStep].timePause[i]);
        //timePause.Add(typeActive.timePause[i]);
        //}
        //timePause.Add(typeActive.typeSteps[currentStep].time - time);
        //timePause.Add(typeActive.time);
    }

    public void BtnHome()
    {
        MenuController.stateUI = 0;
        SceneManager.LoadScene("Menu");
    }

    public void BtnLesson()
    {
        MenuController.stateUI = 1;
        SceneManager.LoadScene("Menu");
    }

    bool tweenDrop = false;
    public void BtnDropDownActivity()
    {
        if (!pause) BtnPause();
        DropDownActivity();
    }

    public void DropDownActivity()
    {
        Debug.LogError("drop down");
        if (!tweenDrop)
        {
            listActivity.transform.DOScaleY(1f, 0.25f);
            tweenDrop = true;
            btnCloseActivity.gameObject.SetActive(true);
        }
        else
        {
            listActivity.transform.DOScaleY(0, 0.25f);
            tweenDrop = false;
            btnCloseActivity.gameObject.SetActive(false);
        }
    }

    bool IsHaveSecondaryActivity()
    {
        for (int i = 0; i < data.typeClasses[currentClass].typeLessons[currentLesson].typeSecondaryActives.Length; i++)
        {
            if (data.typeClasses[currentClass].typeLessons[currentLesson].typeSecondaryActives[i].idActive == currentActivity) return true;
        }
        return false;
    }

    public void BtnPrevious()
    {
        if (currentActivity == 0) return;
        currentActivity--;
        //CreateActivity();
        SetCurrentActivity();
        DataParam.actionStep();
    }

    public void BtnBack()
    {
        //if (!currentTimeline) return;
        isSetSliderAccordingToTime = false;
        CancelInvoke("IsSetSliderAccordingToTime");
        Invoke("IsSetSliderAccordingToTime", 0.5f);
        if (currentSlide > 0)
        {
            currentSlide--;
            if (currentTimeline != null) currentTimeline.time = typeActive.typeSteps[currentStep].typeSlides[currentSlide].time;
            currentVideoPlayer.time = typeActive.typeSteps[currentStep].typeSlides[currentSlide].time;
            Debug.LogError("================= video.time = " + currentVideoPlayer.time);
            //if (pause)
            //{
            //    replay = false;
            //    BtnPause();
            //}
        }
        else
        {
            if (currentStep > 0)
            {
                currentStep--;
                currentSlide = typeActive.typeSteps[currentStep].typeSlides.Length - 1;
                //if (pause)
                //{
                //    replay = false;
                //    BtnPause();
                //}
            }
            else
            {
                //if (currentActivity == 0) return;
                
                if(isSecondaryActivity)
                {
                    isSecondaryActivity = false;
                }
                else
                {
                    currentActivity--;
                    if (IsHaveSecondaryActivity()) isSecondaryActivity = true;
                }
                CreateActivity();
                currentStep = typeActive.typeSteps.Length - 1;
                currentSlide = typeActive.typeSteps[currentStep].typeSlides.Length - 1;
                SetCurrentActivity();
                listStep.SetListStep();
                DataParam.actionBtnActivity();
            }
            DataParam.actionStep();
            
        }
        
        SetSlider();
        //SetBtnBackNForward();

        //if (currentStep > 0)
        //{
        //    currentStep--;
        //    if (pause)
        //    {
        //        currentTimeline.Resume();
        //        if (currentStep == 0) currentTimeline.time = 0;
        //        else currentTimeline.time = typeActive.typeSteps[currentStep - 1].time;
        //        currentTimeline.Pause();
        //    }
        //    else
        //    {
        //        if (currentStep == 0) currentTimeline.time = 0;
        //        else currentTimeline.time = typeActive.typeSteps[currentStep - 1].time;
        //    }
        //}
        //else
        //{
        //    if (currentActivity == 0) return;
        //    currentActivity--;
        //    currentStep = 0;
        //    CreateActivity();
        //    SetCurrentActivity();
        //    listStep.SetListStep();
        //}

        //DataParam.actionStep();

        //if (currentTimePause > 0) currentTimePause--;
        //currentTimeline.time = timePause[currentTimePause];
    }

    public void BtnForward()
    {
        //if (!currentTimeline) return;
        isSetSliderAccordingToTime = false;
        CancelInvoke("IsSetSliderAccordingToTime");
        Invoke("IsSetSliderAccordingToTime", 0.5f);
        if (currentSlide < typeActive.typeSteps[currentStep].typeSlides.Length - 1)
        {
            currentSlide++;
            if (currentTimeline != null) currentTimeline.time = typeActive.typeSteps[currentStep].typeSlides[currentSlide].time;
            currentVideoPlayer.time = typeActive.typeSteps[currentStep].typeSlides[currentSlide].time;

            //if (pause)
            //{
            //    replay = false;
            //    BtnPause();
            //}
        }
        else
        {
            
            if (currentStep < typeActive.typeSteps.Length - 1)
            {
                currentStep++;
                currentSlide = 0;
                //if (pause)
                //{
                //    replay = false;
                //    BtnPause();
                //}
            }
            else
            {
                //if (currentActivity == data.typeClasses[currentClass].typeLessons[currentLesson].typeActives.Length - 1) return;

                if (IsHaveSecondaryActivity() && !isSecondaryActivity) isSecondaryActivity = true;
                else
                {
                    currentActivity++;
                    isSecondaryActivity = false;
                }      
                CreateActivity();
                currentStep = 0;
                currentSlide = 0;
                SetCurrentActivity();
                listStep.SetListStep();
                DataParam.actionBtnActivity();
            }
            DataParam.actionStep();
            
        }
        
        SetSlider();
        //SetBtnBackNForward();

        //if (currentStep < typeActive.typeSteps.Length - 1)
        //{
        //    currentStep++;
        //    if (pause)
        //    {
        //        currentTimeline.Resume();
        //        currentTimeline.time = typeActive.typeSteps[currentStep - 1].time;
        //        currentTimeline.Pause();
        //    }
        //    else
        //     currentTimeline.time = typeActive.typeSteps[currentStep - 1].time;
        //}
        //else
        //{
        //    if (currentActivity == data.typeClasses[currentClass].typeLessons[currentLesson].typeActives.Length - 1) return;
        //    currentActivity++;
        //    currentStep = 0;
        //    CreateActivity();
        //    SetCurrentActivity();
        //    listStep.SetListStep();
        //}

        //DataParam.actionStep();
        //if (currentTimePause < timePause.Count - 1) currentTimePause++;
        //currentTimeline.time = timePause[currentTimePause];
    }

    void SetBtnBackNForward() {
        if (typeActive.typeSteps[currentStep].typeSlides[currentSlide].textBtnForward.Length > 0)
        {
            textBtnForward.text = typeActive.typeSteps[currentStep].typeSlides[currentSlide].textBtnForward;
            btnForwardLong.gameObject.SetActive(true);
            btnForward.gameObject.SetActive(false);
            StartCoroutine(DelayBtnForwardLong());

        }
        else
        {
            btnForwardLong.gameObject.SetActive(false);
            btnForward.gameObject.SetActive(true);
        }

        if (currentSlide == 0 && currentStep == 0 && currentActivity == 0)
        {
            if (IsHaveSecondaryActivity() && isSecondaryActivity) btnBack.interactable = true;
            else btnBack.interactable = false;
        }
        else btnBack.interactable = true;

        if (currentSlide == typeActive.typeSteps[currentStep].typeSlides.Length - 1
            && currentStep == typeActive.typeSteps.Length - 1
            && currentActivity == data.typeClasses[currentClass].typeLessons[currentLesson].typeActives.Length - 1) {
            if (IsHaveSecondaryActivity() && !isSecondaryActivity)
            {
                btnForward.interactable = true;
                btnForwardLong.interactable = true;
            }
            else
            {
                btnForward.interactable = false;
                btnForwardLong.interactable = false;
            }
            
            
        }
        else 
        {
            btnForward.interactable = true;
            btnForwardLong.interactable = true;
        }

        if (currentSlide == typeActive.typeSteps[currentStep].typeSlides.Length - 1
            && currentStep == typeActive.typeSteps.Length - 1)
        {
            if (IsHaveSecondaryActivity() && !isSecondaryActivity)
            {
                btnForward.gameObject.SetActive(false);
                btnForwardLong.gameObject.SetActive(false);
            }
        }


    }

    bool CurrentSliderIsGame() {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        Debug.LogError("================= Game: " + allObjects.Length);

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("_Game")) return true;
        }
        return false;
    }

    public void BtnNext()
    {
        if (currentActivity == data.typeClasses[currentClass].typeLessons[currentLesson].typeActives.Length - 1) return;
        currentActivity++;
        //CreateActivity();
        SetCurrentActivity();
        DataParam.actionStep();
    }

    bool isLock = false;

    public void BtnLock()
    {
        //if (currentTimeline == null) return;
        if (!isLock)
        {
            isLock = true;
            btnLock.gameObject.SetActive(false);
            btnLocked.gameObject.SetActive(true);
            //btnPause.gameObject.SetActive(false);
        }
        else
        {
            isLock = false;
            btnLocked.gameObject.SetActive(false);
            btnLock.gameObject.SetActive(true);
            //btnPause.gameObject.SetActive(true);
        }
        
    }

    bool tweenTools = false;

    public void BtnTools()
    {
        if (!tweenTools)
        {
            tools.DOScaleY(1f, 0.25f);
            tweenTools = true;
        }
        else
        {
            tools.DOScaleY(0, 0.25f);
            tweenTools = false;
        }
    }

    public bool pause = false;
    bool replay = false;
    public void BtnPause()
    {
        //Debug.Log("=================== pause");
        if (!pause)
        {
            isSetSliderAccordingToTime = false;
            btnPlayPause.transform.GetComponent<Image>().sprite = sprPlayPause[0];
             if (currentTimeline != null) currentTimeline.Pause();
            currentVideoPlayer.Pause();
            pause = true;
            //pauseTimeLine = true;
        }
        else
        {
            isSetSliderAccordingToTime = true;
            btnPlayPause.transform.GetComponent<Image>().sprite = sprPlayPause[1];
            if (currentTimeline != null) currentTimeline.Resume();
            currentVideoPlayer.Play();
            pause = false;
            //StartCoroutine(DelayPauseTimeLine());
        }

        if (replay)
        {
            replay = false;
            PlayDotweenBtnForward();
            if (currentTimeline != null) currentTimeline.time = timePause[0];
            currentVideoPlayer.time = timePause[0];
            isSetSliderAccordingToTime = false;
            CancelInvoke("IsSetSliderAccordingToTime");
            Invoke("IsSetSliderAccordingToTime", 0.5f);
        }

    }

    public void Pause() {
        if (!pause)
        {
            if (currentTimeline != null) currentTimeline.Pause();
            currentVideoPlayer.Pause();
            pause = true;
            //pauseTimeLine = true;
        }
        else
        {
            if (currentTimeline != null) currentTimeline.Resume();
            currentVideoPlayer.Play();
            pause = false;
            //StartCoroutine(DelayPauseTimeLine());
        }

        if (replay)
        {
            replay = false;
            PlayDotweenBtnForward();
            if (currentTimeline != null) currentTimeline.time = timePause[0];
            currentVideoPlayer.time = timePause[0];
            isSetSliderAccordingToTime = false;
            CancelInvoke("IsSetSliderAccordingToTime");
            Invoke("IsSetSliderAccordingToTime", 0.5f);
        }
    }

    public void ActionPause()
    {
        pause = false;
        BtnPause();
    }

    public void BtnReplay()
    {

    }

    public void BtnTech()
    {
        TechSupportPopUp.spr = sprTech[0];
        ShowTechSupportPanel();
    }

    public void BtnTimer() 
    {
        if (!pause) BtnPause();
        if (tweenDrop) DropDownActivity();
        ShowTimerMenuPanel();
    }

    public void BtnSpin()
    {
        if (!pause) BtnPause();
        if (tweenDrop) DropDownActivity();
        ShowSpinPanel();
    }

    public void BtnClose()
    {
        ShowCloseAppPanel();
    }

    public void BtnSetting()
    {
        if (!pause) BtnPause();
        if (tweenDrop) DropDownActivity();
        ShowSettingPanel();
    }

    bool isZoom = false;
    Transform content;

    public void SetZoom(float zoom)
    {
        if (content == null) return;
        content.transform.localScale = new Vector3(zoom, zoom, 1);
        sliderZoom.value = zoom;
        if (zoom >= valueZoomDefault) valueZoom = zoom;
    } 

    public void BtnZoom()
    {
        if (!pause) BtnPause();
        if (!sliderZoom.transform.parent.gameObject.activeSelf)
        {
            sliderZoom.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            sliderZoom.transform.parent.gameObject.SetActive(false);
        }
    }

    public void BtnPlanLesson()
    {
        //if (!pause) BtnPause();
        LessonPlanPopUp.pathTextAsset = "Text/" + data.typeClasses[currentClass].name + "/" + data.typeClasses[currentClass].typeLessons[currentLesson].name + "/" + data.typeClasses[currentClass].typeLessons[currentLesson].typeActives[listActivity.currentPlanLesson].name;
        LessonPlanPopUp.spr = data.typeClasses[currentClass].typeLessons[currentLesson].typeActives[listActivity.currentPlanLesson].sprLessonPanel;
        ShowLessonPlanPanel();
        StartCoroutine(DelayFrameLessonPlan());
    }

    void PlayDotweenBtnForward()
    {
        if (replay && !CurrentSliderIsGame())
        {
            tween1.DORestart();
            tween2.DORestart();
        }
        else
        {
            tween1.DORewind();
            tween2.DORewind();
        }
    }

    IEnumerator DelayFrameLessonPlan() {
        if (lessonPlanPanel != null) lessonPlanPanel.frame.GetComponent<ContentSizeFitter>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        if (lessonPlanPanel != null) lessonPlanPanel.frame.GetComponent<ContentSizeFitter>().enabled = true;
    }

    IEnumerator DelayTimeLineStop()
    {
        for (int i = 1; i < timePause.Count - 1; i++)
        {
            sliderFakeTimeVideo.value = timePause[i];
            yield return new WaitForSeconds(0.05f);
            timeLineStop[i - 1].position = new Vector2(sliderFakeTimeVideo.handleRect.position.x, timeLineStop[i - 1].position.y);
            timeLineStop[i - 1].gameObject.SetActive(true);
        }
    }

    IEnumerator DelayPauseTimeLine()
    {
        yield return new WaitForSeconds(0.5f);
        pauseTimeLine = false;
    }
}

public static class TransformExtensions
{
    public static T FindDeepChild<T>(this Transform parent, string targetName) where T : Component
    {
        foreach (Transform child in parent)
        {
            if (child.name == targetName)
            {
                T component = child.GetComponent<T>();
                if (component != null)
                    return component;
            }
                
            var result = child.FindDeepChild<T>(targetName);
            if (result != null)
                return result;
        }
        return null;
    }
}
