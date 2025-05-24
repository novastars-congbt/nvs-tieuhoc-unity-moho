using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.DuaXe
{
    public class SettingPopup : MonoBehaviour
    {
        [SerializeField] private Button _checkShufferQuetstionButton;
        [SerializeField] private Button _checkShufferAnswerButton;
        [SerializeField] private GameObject _checkedSufferQuestion;
        [SerializeField] private GameObject _checkedSufferAnswer;

        [Space]
        [SerializeField] private Slider _quesitonNumberSlider;
        [SerializeField] private TextMeshProUGUI _questionNumberTMP;
        [SerializeField] private Slider _speedSlider;
        [SerializeField] private TextMeshProUGUI _speedTMP;


        [Space]
        [SerializeField] private GameObject _sufferQuestionGroup;
        [SerializeField] private GameObject _sufferAnswerGroup;
        [SerializeField] private GameObject _questionSliderGroup;
        [SerializeField] private GameObject _speedSliderGroup;

        private readonly int minQuestionNumber = 2;
        private readonly int maxQuestionNumber = 20;
        private readonly float minSpeed = 2;
        private readonly float maxSpeed = 6;
        private bool isSufferQuestionOn = true;
        private bool isSufferAnswerOn = true;

        #region Unity Funcion
        private void Start()
        {
            _sufferQuestionGroup.SetActive(UIManager.Instance.TurnOnSufferQuestion);
            _sufferAnswerGroup.SetActive(UIManager.Instance.TurnOnSufferAnswer);
            _questionSliderGroup.SetActive(UIManager.Instance.TurnOnSliderAmount);
            _speedSliderGroup.SetActive(UIManager.Instance.TurnOnSliderAmount);

            GameManager.Instance.CurrentCarSprite = UIManager.Instance.DefaultCarSelectSprite;

            SetupToggle();
            SetupSlider();
        }
        #endregion

        #region Private Method
        private void SetupToggle()
        {
            _checkedSufferQuestion.SetActive(isSufferQuestionOn);
            _checkShufferQuetstionButton.onClick.AddListener(SetSufferQuestionOnOff);
            _checkedSufferAnswer.SetActive(isSufferAnswerOn);
            _checkShufferAnswerButton.onClick.AddListener(SetSufferAnswerOnOff);

            GameManager.Instance.IsSufferQuestionOn = isSufferQuestionOn;
            GameManager.Instance.IsSufferAnswerOn = isSufferAnswerOn;
        }

        private void SetSufferQuestionOnOff()
        {
            isSufferQuestionOn = !isSufferQuestionOn;
            _checkedSufferQuestion.SetActive(isSufferQuestionOn);

            GameManager.Instance.IsSufferQuestionOn = isSufferQuestionOn;
        }

        private void SetSufferAnswerOnOff()
        {
            isSufferAnswerOn = !isSufferAnswerOn;
            _checkedSufferAnswer.SetActive(isSufferAnswerOn);

            GameManager.Instance.IsSufferAnswerOn = isSufferAnswerOn;
        }

        private void SetupSlider()
        {
            int currentMaxQuestion = DataManager.Instance.dataQuestions.QuestionDatas.Count;

            GameManager.Instance.CurrentQuestionAmount = DataManager.Instance.dataQuestions.DefaultQuesitonAmount;

            _questionNumberTMP.text = DataManager.Instance.dataQuestions.DefaultQuesitonAmount.ToString();
            _quesitonNumberSlider.value = DataManager.Instance.dataQuestions.DefaultQuesitonAmount;
            _quesitonNumberSlider.minValue = minQuestionNumber;
            _quesitonNumberSlider.maxValue = currentMaxQuestion > maxQuestionNumber ? maxQuestionNumber : currentMaxQuestion;

            float spd = GameController.Instance._foregroundSpeed*10;
            _speedTMP.text = spd.ToString();
            _speedSlider.value = spd;
            _speedSlider.minValue = minSpeed;
            _speedSlider.maxValue = maxSpeed;

            _quesitonNumberSlider.onValueChanged.AddListener(OnQuestionNumberChange);
            _speedSlider.onValueChanged.AddListener(OnSpeedChange);

            GameManager.Instance.DoneSetupConfig = true;

            gameObject.SetActive(false);
        }

        private void OnQuestionNumberChange(float cardNumber)
        {
            GameManager.Instance.CurrentQuestionAmount = (int)cardNumber;
            _questionNumberTMP.text = ((int)(cardNumber)).ToString();
        }
        private void OnSpeedChange(float cardNumber)
        {
            GameController.Instance._foregroundSpeed = (int)cardNumber;
            _speedTMP.text = ((int)(cardNumber)).ToString();
        }
        #endregion
    }
}