using Novastars.MiniGame.LatBai;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.DuaXe
{
    public class GameplayUI : MonoBehaviour
    {
        [Header("Start CountDown")]
        [SerializeField] private Image _countDownImg;
        [SerializeField] private List<Sprite> countDownSprite = new List<Sprite>();

        [Header("UI Element")]
        [SerializeField] private TextMeshProUGUI _timerTMP;
        [SerializeField] private Image _gameplayBackground;
        [SerializeField] private Image _gameplayForeground;
        [SerializeField] private Image _answerA_Img;
        [SerializeField] private Image _answerB_Img;


        [Header("Result Object")]
        [SerializeField] private ResultPopup _resultPopup;

        [Header("Guide")]
        [SerializeField] private TextMeshProUGUI _guideTMP;

        [Header("Q&A UI Element")]
        [SerializeField] private GameObject _QAGroup;
        [SerializeField] private TextMeshProUGUI _questionTMP;
        [SerializeField] private Answer _answerA;
        [SerializeField] private Answer _answerB;

        [Space]
        [SerializeField] private Transform _answer1_Road;
        [SerializeField] private Transform _answer2_Road;
        [SerializeField] private Transform _answer3_Road;

        private DateTime _currenttimerCounter;
        private DateTime _startTimerCounter;
        private bool _isRunning = false;

        #region Unity Funcion
        private void OnEnable()
        {
            //if(_isRunning)
            RestartGame();
            StartGame();
        }
        private void StartGame(){
            FeedbackManager.Instance.PlayBackgroundMusic();
            StartCoroutine(SetupCourotine());
            StartCoroutine(StartCountDown());

            _QAGroup.SetActive(false);
        }

        private void Start()
        {
            _guideTMP.text = DataManager.Instance.GuideString;
            _gameplayBackground.sprite = UIManager.Instance.GameplayBackgroundSprite;
            _gameplayForeground.sprite = UIManager.Instance.GameplayForegroundSprite;
            _answerA_Img.sprite = UIManager.Instance.AAnswerSprite;
            _answerB_Img.sprite = UIManager.Instance.BAnswerSprite;
        }

        private void Update()
        {
            if (_isRunning) CountUptimer();
        }
        #endregion

        #region Public Method
        public void ShowResult()
        {
            _resultPopup.gameObject.SetActive(true);
            _resultPopup.ShowResult(_timerTMP.text);
            _timerTMP.gameObject.SetActive(false);
        }

        public void RestartGame()
        {
            _resultPopup.gameObject.SetActive(false);
            _timerTMP.gameObject.SetActive(true);
            _timerTMP.text = "00:00";
            _isRunning = false;

            GameManager.Instance.IsGameOver = false;
            GameManager.Instance.CurrentQuestionIndex = 0;
            GameController.Instance.SetupDefault();
            FeedbackManager.Instance.DestroyAll();

            //StartCoroutine(StartCountDown());
        }

        public void ShowQA(int roadNumber)
        {
            SetupAnswerPos(roadNumber);
            _QAGroup.SetActive(true);

            var currentQAIndex = GameManager.Instance.CurrentQuestionIndex;
            var currentQADatas = GameManager.Instance.CurrentQuestionDatas;
            var randomQA_Oders = new List<int>();

            if (GameManager.Instance.IsSufferAnswerOn)
            {
                while (randomQA_Oders.Count < 2)
                {
                    var number = UnityEngine.Random.Range(0, 2);
                    if (!randomQA_Oders.Contains(number)) randomQA_Oders.Add(number);
                }

                _answerA.Setup(currentQADatas[currentQAIndex].GetAnswer(randomQA_Oders[0]).Item1, currentQADatas[currentQAIndex].GetAnswer(randomQA_Oders[0]).Item2, OffQA_Group);
                _answerB.Setup(currentQADatas[currentQAIndex].GetAnswer(randomQA_Oders[1]).Item1, currentQADatas[currentQAIndex].GetAnswer(randomQA_Oders[1]).Item2, OffQA_Group);
            }
            else
            {
                _answerA.Setup(currentQADatas[currentQAIndex].GetAnswer(0).Item1, currentQADatas[currentQAIndex].GetAnswer(0).Item2, OffQA_Group);
                _answerB.Setup(currentQADatas[currentQAIndex].GetAnswer(1).Item1, currentQADatas[currentQAIndex].GetAnswer(1).Item2, OffQA_Group);
            }

            _questionTMP.text = currentQADatas[currentQAIndex].QuestionString;
        }
        #endregion

        #region Private Method
        private void OffQA_Group() => _QAGroup.SetActive(false);
        private IEnumerator SetupCourotine()
        {
            yield return new WaitUntil(() => GameManager.Instance.DoneSetupConfig);

            GameController.Instance.SetupCar();
            GameManager.Instance.SetupQuestionData();
        }

        private IEnumerator StartCountDown()
        {
            GameManager.Instance.IsGameOver = false;
            _countDownImg.gameObject.SetActive(true);

            var countDown = 2;
            while (countDown >= 0)
            {
                _countDownImg.sprite = countDownSprite[countDown];
                yield return new WaitForSeconds(1f);

                countDown--;
            }

            _isRunning = true;
            _startTimerCounter = DateTime.Now;

            _countDownImg.gameObject.SetActive(false);
            GameController.Instance.StartRace();
        }

        private void CountUptimer()
        {
            if (GameManager.Instance.IsGameOver) return;

            _currenttimerCounter = DateTime.Now;

            var timer = _currenttimerCounter - _startTimerCounter;
            _timerTMP.text = timer.ToString("mm\\:ss");

            //_timerTMP.text = timer.ToString(@"mm\:ss");
        }

        private void SetupAnswerPos(int roadNumber)
        {
            switch (roadNumber)
            {
                case 1:
                    _answerA.transform.position = _answer2_Road.position; _answerA.SetupRoadNumber(2);
                    _answerB.transform.position = _answer3_Road.position; _answerB.SetupRoadNumber(3);
                    break;
                case 2:
                    _answerA.transform.position = _answer1_Road.position; _answerA.SetupRoadNumber(1);
                    _answerB.transform.position = _answer3_Road.position; _answerB.SetupRoadNumber(3);
                    break;
                case 3:
                    _answerA.transform.position = _answer1_Road.position; _answerA.SetupRoadNumber(1);
                    _answerB.transform.position = _answer2_Road.position; _answerB.SetupRoadNumber(2);
                    break;
                default: break;
            }
        }
        #endregion
    }
}