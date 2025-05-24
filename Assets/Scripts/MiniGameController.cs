using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using Sirenix.Utilities;
using Unity.VisualScripting;

[DefaultExecutionOrder(-1)]
public class MiniGameController : MonoBehaviour
{
    public static MiniGameController instance;
    Canvas canvasDisplay;
    [LabelText("Tự động cập nhật cấp độ")]
    [SerializeField]
    bool isAutoUpdateLevel = true;
    [LabelText("Số lượng cấp độ")]
    [SerializeField]
    [Min(1)]
    int numberOfLevels = 1;
    [LabelText("Danh sách cấp độ")]
    public Level[] levels;
    [Header("Màn hình hiển thị")]
    [SerializeField]
    [LabelText("Ảnh nền màn hình bắt đầu")]
    Sprite sprBgStart;
    [SerializeField]
    [LabelText("Ảnh nền màn hình cấp độ")]
    Sprite sprBgLevel;
    [SerializeField]
    [LabelText("Ảnh nền màn hình kết thúc")]
    Sprite sprBgEnd;
    GameObject displayGameStart;
    GameObject displayGameLevel;
    GameObject displayGamePlay;
    //GameObject displayGameEnd;
    [Header("Cài đặt màn hình bắt đầu")]
    [LabelText("Có màn hình bắt đầu")]
    public bool haveDisplayGameStart;
    [SerializeField]
    [LabelText("Có tuỳ chỉnh")]
    bool haveCustom;
    [SerializeField]
    [LabelText("Có hướng dẫn")]
    bool haveGuide;
    Image bgGameStart;
    Button btnStartGame;
    Button btnCustom;
    Button btnGuide;
    public Action actionCustom;
    [Header("Tuỳ chỉnh")]
    [LabelText("Ngẫu nhiên câu hỏi")]
    public bool isRandomQuestion;
    [LabelText("Ngẫu nhiên câu trả lời")]
    public bool isRandomAnswer;
    [LabelText("Tổng số câu hỏi hiện tại")]
    public int totalQuestion;
    [SerializeField]
    [LabelText("Số câu hỏi tối thiểu")]
    int minQuestion = 1;
    [SerializeField]
    [LabelText("Số câu hỏi tối đa")]
    int maxQuestion;
    GameObject customPopUp;
    GameObject checkRandomQuestion;
    GameObject unCheckRandomQuestion;
    GameObject checkRandomAnswer;
    GameObject unCheckRandomAnswer;
    Slider sliderQuestion;
    InputField inputFieldQuestion;
    [Header("Hướng dẫn")]
    [SerializeField]
    [LabelText("Kiểu hướng dẫn")]
    TypeGuild typeGuide;
    [SerializeField]
    [LabelText("Văn bản hướng dẫn")]
    TextAsset[] textAssetGuide;
    [SerializeField]
    [LabelText("Ảnh hướng dẫn")]
    Sprite[] sprGuide;
    GameObject guidePopUp;
    Text textGuide;
    Image imgGuide;
    [SerializeField]
    [LabelText("Video hướng dẫn trên Window")]
    GameObject[] videoOnlineGuide;
    [SerializeField]
    [LabelText("Video hướng dẫn trên WebGL")]
    GameObject[] videoOffilneGuide;
    int currentGuide;
    [Header("Cài đặt màn hình cấp độ")]
    [SerializeField]
    [LabelText("Có màn hình cấp độ")]
    public bool haveDisplayGameLevel;
    [SerializeField]
    [LabelText("Có ảnh cấp độ")]
    bool haveImageLevel;
    [SerializeField]
    [LabelText("Có đánh số thứ tự cấp độ")]
    bool haveNumberedLevel;
    [SerializeField]
    [LabelText("Có tên cấp độ")]
    bool haveNameLevel;
    [SerializeField]
    [LabelText("Ảnh nền khung tiêu đề")]
    Sprite[] sprFrameTitleLevel;
    [SerializeField]
    [LabelText("Ảnh nền khung đánh số cấp độ")]
    Sprite[] sprFrameNumberedLevel;
    Image bgGameLevel;
    Button btnBackFromGameLevel;
    [HideInInspector]
    public int currentLevel = 0;
    public Action<int> actionPlayLevel;
    [SerializeField]
    [LabelText("Prefab cấp độ")]
    GameObject prefabLevel;
    Transform listLevel;

    // [Header("Cài đặt màn hình chơi game")]
    // Button btnBackFromGamePlay;
    // [Header("Cài đặt màn hình kết thúc")]
    // [SerializeField]
    // [LabelText("Kiểu kết thúc")]
    // TypeEnd typeEnd;
    // [SerializeField]
    // [LabelText("Có chơi tiếp")]
    // bool haveNextGame;
    // [SerializeField]
    // [LabelText("Có tổng kết")]
    // bool haveFade;
    // Image bgGameEnd;
    // Button btnHome;
    // Button btnNextGame;
    // Button btnRestartGame;
    // Button btnSkipGame;
    // [HideInInspector]
    // public Transform displayGameEndHidden;
    // public Action actionNextGame;
    // public Action actionRestartGame;
    // public Action actionSkipGame;
    // [Header("Kết thúc thường")]
    // GameObject frameEndStar;
    // [Header("Kết thúc tính điểm")]
    // [LabelText("Văn bản tính điểm")]
    // [SerializeField]
    // string strEndScore = "<b><i><size=40>Bạn đã đạt được\n</size><size=100>xxx\n</size><size=35>điểm</size></i></b>";
    // GameObject frameEndScore;
    // Text textEndScore;
    // [HideInInspector]
    // public int score;
    // [Header("Kết thúc đáp án")]
    // GameObject frameEndAnswer;
    // [SerializeField]
    // [LabelText("Có ảnh đáp án")]
    // bool haveImageAnswer;
    // [SerializeField]
    // [LabelText("Có chữ đáp án")]
    // bool haveTextAnswer;
    // [SerializeField]
    // [LabelText("Ảnh nền khung chữ đáp án")]
    // Sprite[] sprFrameTextAnswer;
    // [SerializeField]
    // [LabelText("Prefab câu trả lời")]
    // GameObject prefabAnswer;
    // Transform listAnswer;
    // [SerializeField]
    // [LabelText("Số câu trả lời mặc định")]
    // int defautNumberOfAnswer = 5;
    // [HideInInspector]
    // public int totalAnswer;
    // [Header("Cài đặt thắng")]
    // [LabelText("Có màn hình thắng giữa các câu hỏi")]
    // public bool haveBigWin;
    // [HideInInspector]
    // public bool isWin;
    // [Header("Hiệu ứng")]
    // [LabelText("Hiệu ứng chọn")]
    // public ParticleSystem particleChoose;
    // [LabelText("Hiệu ứng trả lời đúng")]
    // public ParticleSystem particleCorrect;
    // [LabelText("Hiệu ứng trả lời sai")]
    // public ParticleSystem particleWrong;
    // [LabelText("Hiệu ứng thắng")]
    // public ParticleSystem particleWin;
    // [LabelText("Hiệu ứng thắng to")]
    // public ParticleSystem particleBigWin;
    // [LabelText("Hiệu ứng thua")]
    // public ParticleSystem particleLose;
    // [LabelText("Hiệu ứng mở rương")]
    // public ParticleSystem particleOpenTreasure;
    [Header("Âm thanh")]
    [HideInInspector]
    public AudioSource audioSourceOther;
    // [LabelText("Âm thanh chọn")]
    // public AudioClip audioParticleChoose;
    // [LabelText("Âm thanh trả lời đúng")]
    // public AudioClip audioParticleCorrect;
    // [LabelText("Âm thanh trả lời sai")]
    // public AudioClip audioParticleWrong;
    // [LabelText("Âm thanh thắng")]
    // public AudioClip audioParticleWin;
    // [LabelText("Âm thanh thắng to")]
    // public AudioClip audioParticleBigWin;
    // [LabelText("Âm thanh thua")]
    // public AudioClip audioParticleLose;
    // [LabelText("Âm thanh mở rương")]
    // public AudioClip audioParticleOpenTreasure;
    // [LabelText("Âm thanh kết thúc game")]
    // public AudioClip audioGameEnd;

    enum TypeGuild
    {
        text, image, video
    }

    [Serializable]
    public struct Level {
        public string nameLevel;
        public Sprite sprLevel;
        public GameObject prefabGame;
    }

    // enum TypeEnd
    // {
    //     normal, score, answer
    // }

    private void OnEnable()
    {
        Debug.LogError("===================== minigamecontroller");
        if (instance == null) instance = this;
        SetCamera();
        AttachAllObject();
        AddListenerAllButton();
        if (prefabLevel == null) SetPrefabLevel();
        //if (prefabAnswer == null) SetPrefabAnswer();

        if (haveDisplayGameStart) SetGameStart();
        if (haveDisplayGameLevel) SetGameLevel();
        //SetGameEnd();

        if (haveDisplayGameStart) ShowGameStart();
        else if (haveDisplayGameLevel) ShowGameLevel();
        else ShowGamePlay();
        
    }

    private void OnDisable()
    {
        instance = null;
    }

    #if UNITY_EDITOR
private void OnValidate()
{
    if (numberOfLevels < 1) numberOfLevels = 1;
    Array.Resize(ref levels, numberOfLevels);
    displayGamePlay = transform.FindDeepChild<Transform>("DisplayGamePlay").gameObject;
    if (!isAutoUpdateLevel) return;
    if (!levels[0].prefabGame && displayGamePlay.transform.childCount > 0) levels[0].prefabGame = displayGamePlay.transform.GetChild(0).gameObject; 
    for (int i = 0; i < displayGamePlay.transform.childCount; i++) {
        if (i == 0) continue;
        displayGamePlay.transform.GetChild(i).gameObject.SetActive(false);
    }
    for (int i = 0; i < levels.Length; i++) {
        if (i == 0) continue;
        GameObject game = GetGameAutoUpdate(i);
        game.name = game.name.Replace("(Clone)", "_"+(i + 1));
        game.SetActive(true);
        levels[i].prefabGame = game;
    }
}
#endif

    void SetCamera()
    {
        canvasDisplay = transform.FindDeepChild<Canvas>("CanvasDisplay");
        if (canvasDisplay.worldCamera) return;
        canvasDisplay.worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        canvasDisplay.sortingOrder = 1;
    }

    void AttachAllObject()
    {
        AttachObject(ref displayGameStart, "displayGameStart");
        AttachObject(ref displayGameLevel, "displayGameLevel");
        AttachObject(ref displayGamePlay, "displayGamePlay");
        //AttachObject(ref displayGameEnd, "displayGameEnd");
        AttachObject(ref bgGameStart, "bgGameStart");
        AttachObject(ref bgGameLevel, "bgGameLevel");
        //AttachObject(ref bgGameEnd, "bgGameEnd");
        AttachObject(ref btnStartGame, "btnStartGame");
        AttachObject(ref btnCustom, "btnCustom");
        AttachObject(ref btnGuide, "btnGuide");
        AttachObject(ref customPopUp, "customPopUp");
        AttachObject(ref checkRandomQuestion, "checkRandomQuestion");
        AttachObject(ref unCheckRandomQuestion, "unCheckRandomQuestion");
        AttachObject(ref checkRandomAnswer, "checkRandomAnswer");
        AttachObject(ref unCheckRandomAnswer, "unCheckRandomAnswer");
        AttachObject(ref sliderQuestion, "sliderQuestion");
        AttachObject(ref inputFieldQuestion, "inputFieldQuestion");
        AttachObject(ref guidePopUp, "guidePopUp");
        AttachObject(ref textGuide, "textGuide");
        AttachObject(ref imgGuide, "imgGuide");
        AttachObject(ref btnBackFromGameLevel, "btnBackFromGameLevel");
        AttachObject(ref listLevel, "listLevel");
        // AttachObject(ref btnBackFromGamePlay, "btnBackFromGamePlay");
        // AttachObject(ref btnHome, "btnHome");
        // AttachObject(ref btnNextGame, "btnNextGame");
        // AttachObject(ref btnRestartGame, "btnRestartGame");
        // AttachObject(ref displayGameEndHidden, "displayGameEndHidden");
        // AttachObject(ref frameEndStar, "frameEndStar");
        // AttachObject(ref frameEndScore, "frameEndScore");
        // AttachObject(ref textEndScore, "textEndScore");
        // AttachObject(ref frameEndAnswer, "frameEndAnswer");
        // AttachObject(ref listAnswer, "listAnswer");
        AttachObject(ref audioSourceOther, "audioSourceOther");
    }

    void AddListenerAllButton()
    {
        AddListenerButton(ref btnStartGame, BtnStartGame);
        AddListenerButton(ref btnCustom, BtnCustom);
        AddListenerButton(ref btnGuide, BtnGuide);
        AddListenerButton(ref btnBackFromGameLevel, BtnBackFromGameLevel);
        // AddListenerButton(ref btnBackFromGamePlay, BtnBackFromGamePlay);
        // AddListenerButton(ref btnHome, BtnHome);
        // AddListenerButton(ref btnNextGame, BtnNextGame);
        // AddListenerButton(ref btnRestartGame, BtnRestartGame);
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
        //Debug.LogError("=================== T " + nameObjFind);
        obj = transform.FindDeepChild<T>(nameObjFind);
        //}
    }

    void ShowGame(GameObject gameObject)
    {
        switch (gameObject.name) {
            case "DisplayGameStart":
                displayGameStart.SetActive(true);
                displayGameLevel.SetActive(false);
                displayGamePlay.SetActive(false);
                //displayGameEnd.SetActive(false);
                break;
            case "DisplayGameLevel":
                displayGameStart.SetActive(haveDisplayGameStart);
                displayGameLevel.SetActive(true);
                displayGamePlay.SetActive(false);
                //displayGameEnd.SetActive(false);
                break;
            case "DisplayGamePlay":
                displayGameStart.SetActive(haveDisplayGameStart);
                displayGameLevel.SetActive(haveDisplayGameLevel);
                displayGamePlay.SetActive(true);
                //displayGameEnd.SetActive(false);
                break;
            // case "DisplayGameEnd":
            //     displayGameStart.SetActive(haveDisplayGameStart);
            //     displayGameLevel.SetActive(haveDisplayGameLevel);
            //     displayGamePlay.SetActive(haveFade);
            //     displayGameEnd.SetActive(true);
            //     break;
        }
    }

    #region Audio
    public void PlayAudio(AudioClip audioClip)
    {
        if (audioClip != null) audioSourceOther.PlayOneShot(audioClip);
    }
    #endregion


    #region Particle
    public void PlayParticle(ParticleSystem particle, AudioClip audioClip)
    {
        if (particle != null) particle.Play();
        PlayAudio(audioClip);
    }

    public void PlayParticle(ParticleSystem particle, AudioClip audioClip, Vector3 posTarget)
    {
        if (particle != null) particle.transform.position = new Vector3(posTarget.x, posTarget.y, particle.transform.position.z);
        PlayParticle(particle, audioClip);
    }

    // public IEnumerator Play(string e, Vector3 p, AudioClip c = null)
    // {
    //     switch (e)
    //     {
    //         case "choose":
    //             PlayParticle(particleChoose, audioParticleChoose, p);
    //             break;
    //         case "win":
    //             PlayParticle(particleWin, audioParticleWin, p);
    //             break;
    //         case "big":
    //             PlayParticle(particleBigWin, audioParticleBigWin, p);
    //             break;
    //         case "correct":
    //             PlayParticle(particleCorrect, audioParticleCorrect, p);
    //             break;
    //         case "wrong":
    //             PlayParticle(particleWrong, audioParticleWrong, p);
    //             break;
    //         case "lose":
    //             PlayParticle(particleLose, audioParticleLose, p);
    //             break;
    //     }

    //     yield return new WaitForSeconds(1f);

    //     if (c != null)
    //     {
    //         audioSourceOther.PlayOneShot(c);
    //         yield return new WaitForSeconds(1f);
    //     }
    // }
    #endregion

    #region GameStart

    public void ShowGameStart()
    {
        ShowGame(displayGameStart);
    }

    void SetGameStart()
    {
        if (sprBgStart) bgGameStart.sprite = sprBgStart;
        btnCustom.gameObject.SetActive(haveCustom);
        btnGuide.gameObject.SetActive(haveGuide);
        customPopUp.SetActive(false);
        guidePopUp.SetActive(false);
        if (haveGuide)
        {
            currentGuide = 0;
            textGuide.gameObject.SetActive(false);
            imgGuide.gameObject.SetActive(false);
            foreach (GameObject video in videoOffilneGuide) video.SetActive(false);
            foreach (GameObject video in videoOnlineGuide) video.SetActive(false);
        }
        if (haveCustom)
        {
            checkRandomQuestion.SetActive(isRandomQuestion);
            unCheckRandomAnswer.SetActive(!isRandomQuestion);
            checkRandomAnswer.SetActive(isRandomAnswer);
            unCheckRandomAnswer.SetActive(!isRandomAnswer);
            if (sliderQuestion.minValue != minQuestion) sliderQuestion.minValue = minQuestion;
            if (sliderQuestion.maxValue != maxQuestion) sliderQuestion.maxValue = maxQuestion;
            if (totalQuestion < minQuestion) totalQuestion = minQuestion;
            sliderQuestion.value = totalQuestion;
            inputFieldQuestion.text = totalQuestion + "";
        }
    }

    public void SetQuestion(float question)
    {
        totalQuestion = (int)question;
        inputFieldQuestion.text = totalQuestion + "";
    }

    bool successEditQuestion;
    int totalEditQuestion;
    public void EndEditQuestion()
    {
        if (inputFieldQuestion.text != "")
        {
            successEditQuestion = int.TryParse(inputFieldQuestion.text, out totalEditQuestion);
            if (successEditQuestion && totalEditQuestion > 0)
            {
                totalQuestion = totalEditQuestion;
            }
        }
    }

    public void BtnRandomQuestion(bool randomQuestion)
    {
        isRandomQuestion = randomQuestion;
        checkRandomQuestion.SetActive(randomQuestion);
        unCheckRandomQuestion.SetActive(!randomQuestion);
    }

    public void BtnRandomAnswer(bool randomAnswer)
    {
        isRandomAnswer = randomAnswer;
        checkRandomAnswer.SetActive(randomAnswer);
        unCheckRandomAnswer.SetActive(!randomAnswer);
    }

    public void BtnStartGame()
    {
        if (!haveDisplayGameLevel) ShowGamePlay();
        else ShowGameLevel();
    }

    public void BtnCustom()
    {
        customPopUp.SetActive(true);
    }

    public void BtnGuide()
    {
        guidePopUp.SetActive(true);
        switch (typeGuide)
        {
            case TypeGuild.text:
                textGuide.gameObject.SetActive(true);
                textGuide.text = textAssetGuide[currentGuide].text;
                break;
            case TypeGuild.image:
                imgGuide.gameObject.SetActive(true);
                imgGuide.sprite = sprGuide[currentGuide];
                break;
            case TypeGuild.video:
#if !UNITY_WEBGL
                    for (int i = 0; i < videoOffilneGuide.Length; i++)
                    {
                        if (i == currentGuide) videoOffilneGuide[i].SetActive(true);
                        else videoOffilneGuide[i].SetActive(false);
                    }
#else
                for (int i = 0; i < videoOnlineGuide.Length; i++)
                {
                    if (i == currentGuide) videoOnlineGuide[i].SetActive(true);
                    else videoOnlineGuide[i].SetActive(false);
                }
#endif
                break;
        }
    }

    #endregion

    #region GameLevel

    public void ShowGameLevel()
    {
        ShowGame(displayGameLevel);
    }

    void SetPrefabLevel() {
        GameObject _prefabLevel = Resources.Load<GameObject>("PrefabGames/Level");
        prefabLevel = Instantiate(_prefabLevel, listLevel);
        Transform imageLevel = prefabLevel.transform.FindDeepChild<Transform>("ImageLevel");
        if (haveImageLevel) imageLevel.gameObject.SetActive(true);
        else imageLevel.gameObject.SetActive(false);
        Transform frameTitleLevel = prefabLevel.transform.FindDeepChild<Transform>("FrameTitleLevel");
        if (haveNumberedLevel || haveNameLevel) {
            frameTitleLevel.gameObject.SetActive(true);
            Transform frameNumberedLevel = frameTitleLevel.FindDeepChild<Transform>("FrameNumberedLevel");
            if (haveNumberedLevel) frameNumberedLevel.gameObject.SetActive(true);
            else frameNumberedLevel.gameObject.SetActive(false);

            Transform textNameLevel = frameTitleLevel.FindDeepChild<Transform>("TextNameLevel");
            if (haveNameLevel) textNameLevel.gameObject.SetActive(true);
            else textNameLevel.gameObject.SetActive(false);
        }
        else frameTitleLevel.gameObject.SetActive(false);
    }

    void SetGameLevel()
    {
        if (sprBgLevel) bgGameLevel.sprite = sprBgLevel;
        btnBackFromGameLevel.gameObject.SetActive(haveDisplayGameStart);
        foreach (Transform level in listLevel) {
            level.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < levels.Length; i++)
        {
            GameObject level = GetLevel(i);
            if (haveImageLevel) level.transform.FindDeepChild<Image>("ImageLevel").sprite = levels[i].sprLevel;
            if (haveNumberedLevel || haveNameLevel) {
                if (sprFrameTitleLevel.Length > 0) level.transform.FindDeepChild<Image>("FrameTitleLevel").sprite = sprFrameTitleLevel[i % sprFrameTitleLevel.Length];
                if (haveNumberedLevel) {
                    if (sprFrameNumberedLevel.Length > 0) level.transform.FindDeepChild<Image>("FrameNumberedLevel").sprite = sprFrameNumberedLevel[i % sprFrameNumberedLevel.Length];
                    level.transform.FindDeepChild<Text>("TextNumberedLevel").text = (i + 1) + "";
                } 
                if (haveNameLevel) level.transform.FindDeepChild<Text>("TextNameLevel").text = levels[i].nameLevel;
            }
            level.SetActive(true);
            Button button = level.GetComponent<Button>();
            AddListenerButton(ref button, BtnPlayLevel, i);
        }
    }

    public GameObject GetLevel(int index)
    {
        if (prefabLevel == null) prefabLevel = Resources.Load<GameObject>("PrefabGames/Level");
        if (index <= listLevel.childCount - 1) return listLevel.GetChild(index).gameObject;
        else
        {
            GameObject level = Instantiate(prefabLevel, listLevel);
            return level;
        }
    }

    public GameObject GetGameAutoUpdate(int index) {
        if (index <= displayGamePlay.transform.childCount - 1) return displayGamePlay.transform.GetChild(index).gameObject;
        else
        {
            GameObject game = Instantiate(levels[0].prefabGame, displayGamePlay.transform);
            return game;
        }
    }

    public void SetTextForLevel(int index, string str)
    {
        GetLevel(index).transform.FindDeepChild<Text>("TextLevel").text = str;
    }

    public void SetImageForLevel(int index, Sprite spr)
    {
        GetLevel(index).transform.FindDeepChild<Image>("ImageLevel").sprite = spr;
    }

    public void BtnBackFromGameLevel()
    {
        ShowGameStart();
    }

    public void BtnPlayLevel(int index)
    {
        //actionPlayLevel?.Invoke(index);
        currentLevel = index;
        ShowGamePlay();
    }

    #endregion

    #region GamePlay
    public void ShowGamePlay()
    {
        ShowGame(displayGamePlay);
        SetGame(levels[currentLevel].prefabGame);
    }

    void SetGame(GameObject prefabGame) {
        bool notNeedInstantiate = false;
        foreach (Transform transform in displayGamePlay.transform) {
            if (transform.name == prefabGame.name) {
                transform.gameObject.SetActive(true);
                notNeedInstantiate = true;
            } 
            else transform.gameObject.SetActive(false);
        }
        
        GameObject game;
        if(!notNeedInstantiate) { 
            game = Instantiate(prefabGame);
            game.name = prefabGame.name;
            Canvas canvasGame = game.transform.FindDeepChild<Canvas>("Canvas");
            canvasGame.worldCamera = canvasDisplay.worldCamera;
            canvasGame.sortingOrder = canvasDisplay.sortingOrder + 1;
            // Transform particle = game.transform.FindDeepChild<Transform>("Particle");
            // float oldLossyScaleX = particle.lossyScale.x;
            // Vector3 oldPostion = particle.position;
            game.transform.SetParent(displayGamePlay.transform);
            // game.transform.localPosition = Vector3.zero;
            // game.transform.localScale = Vector3.one;
            // game.transform.FindDeepChild<RectTransform>("Canvas").localPosition = Vector3.zero;
            // particle.localScale = particle.localScale * oldLossyScaleX/particle.lossyScale.x;
            // particle.position = oldPostion;
            // particle.localPosition = new Vector3(particle.localPosition.x, particle.localPosition.y, 0);
            foreach (Transform transform in game.transform) {
                Debug.LogError("============== " + transform.name);
                if (transform.GetComponent<Canvas>() != null) { 
                    RectTransform rectTransform = transform.GetComponent<RectTransform>();
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
                    rectTransform.localScale = Vector3.one;
                }
            }
        }
    }

    public void BtnBackFromGamePlay()
    {
        if (displayGameLevel.activeSelf) ShowGameLevel();
        else if (displayGameStart.activeSelf) ShowGameStart();
    }

    #endregion

    #region GameEnd

    // public void ShowGameEnd()
    // {
    //     ShowGame(displayGameEnd);
    //     PlayAudio(audioGameEnd);
    //     if (displayGameEndHidden != null && displayGameEndHidden.GetComponent<CanvasGroup>() != null) displayGameEndHidden.GetComponent<CanvasGroup>().alpha = 1;
    //     switch (typeEnd)
    //     {
    //         case TypeEnd.answer:
    //             for (int i = 0; i < listAnswer.childCount; i++)
    //             {
    //                 listAnswer.GetChild(i).gameObject.SetActive(false);
    //             }
    //             break;
    //     }
    //     switch (typeEnd)
    //     {
    //         case TypeEnd.score:
    //             textEndScore.text = strEndScore;
    //             textEndScore.text = textEndScore.text.Replace("xxx", "" + score);
    //             textEndScore.text = textEndScore.text.Replace("\\n", "\n");
    //             break;
    //         case TypeEnd.answer:
    //             for (int i = 0; i < totalAnswer; i++)
    //             {
    //                 GameObject answer = GetAnswer(i);
    //                 if (haveTextAnswer) {
    //                     if (sprFrameTextAnswer.Length > 0) answer.transform.FindDeepChild<Image>("FrameTextAnswer").sprite = sprFrameTextAnswer[i % sprFrameTextAnswer.Length];
    //                 }
    //                 answer.SetActive(true);
    //             }
    //             break;
    //     }

    //     if (!haveBigWin) PlayParticle(particleWin, audioParticleWin);
    //     else PlayParticle(particleBigWin, audioParticleBigWin);
    //     if (haveFade) StartCoroutine(FadeDisplayGameEndHidden());
    //     //StartCoroutine(PlayParticleWin(1.5f));
    // }

    // void SetPrefabAnswer() {
    //     GameObject _prefabAnswer = Resources.Load<GameObject>("PrefabGames/Answer");
    //     prefabAnswer = Instantiate(_prefabAnswer, listAnswer);
    //     Transform imageAnswer = prefabAnswer.transform.FindDeepChild<Transform>("ImageAnswer");
    //     if (haveImageAnswer) imageAnswer.gameObject.SetActive(true);
    //     else imageAnswer.gameObject.SetActive(false);
    //     Transform frameTextAnswer = prefabAnswer.transform.FindDeepChild<Transform>("FrameTextAnswer");
    //     if (haveTextAnswer) frameTextAnswer.gameObject.SetActive(true);
    //     else frameTextAnswer.gameObject.SetActive(false);
    // }

    // void SetGameEnd()
    // {
    //     if (sprBgEnd) bgGameEnd.sprite = sprBgEnd;
    //     btnHome.gameObject.SetActive(haveDisplayGameStart || haveDisplayGameLevel || levels.Length > 1);
    //     if (levels.Length > 1) btnNextGame.gameObject.SetActive(true);
    //     else btnNextGame.gameObject.SetActive(false);
    //     frameEndStar.SetActive(false);
    //     frameEndScore.SetActive(false);
    //     frameEndAnswer.SetActive(false);
    //     switch (typeEnd)
    //     {
    //         case TypeEnd.normal:
    //             frameEndStar.SetActive(true);
    //             break;
    //         case TypeEnd.score:
    //             frameEndScore.SetActive(true);
    //             break;
    //         case TypeEnd.answer:
    //             frameEndAnswer.SetActive(true);
    //             break;
    //     }
    // }

    // public GameObject GetAnswer(int index)
    // {
    //     if (index <= listAnswer.childCount - 1) return listAnswer.GetChild(index).gameObject;
    //     else
    //     {
    //         GameObject answer = Instantiate(prefabAnswer, listAnswer);
    //         return answer;
    //     }
    // }

    // public void SetTextForAnswer(int index, string str)
    // {
    //     GetAnswer(index).transform.FindDeepChild<Text>("TextAnswer").text = str;
    // }

    // public void SetImageForAnswer(int index, Sprite spr)
    // {
    //     GetAnswer(index).transform.FindDeepChild<Image>("ImageAnswer").sprite = spr;
    // }

    // IEnumerator FadeDisplayGameEndHidden()
    // {
    //     Debug.LogError("============== co fade");
    //     if (haveBigWin) yield return new WaitForSeconds(audioParticleBigWin.length + 1);
    //     else yield return new WaitForSeconds(audioParticleWin.length + 1);
    //     CanvasGroup canvasGroup = displayGameEndHidden.GetComponent<CanvasGroup>();
    //     DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, 0.5f);
    // }

    // public void BtnHome()
    // {
    //     if (displayGameStart.activeSelf) ShowGameStart();
    //     else if (displayGameLevel.activeSelf) ShowGameLevel();
    // }

    // public void BtnNextGame()
    // {
    //     if (actionNextGame != null)
    //     {
    //         Debug.LogError("============= co action next");
    //         actionNextGame?.Invoke();
    //     }
    //     currentLevel++;
    //     if (currentLevel > levels.Length - 1) currentLevel = 0;
    //     displayGamePlay.SetActive(false);
    //     ShowGamePlay();
    // }

    // public void BtnRestartGame()
    // {
    //     actionRestartGame?.Invoke();
    //     displayGamePlay.SetActive(false);
    //     ShowGamePlay();
    // }
    #endregion
}
