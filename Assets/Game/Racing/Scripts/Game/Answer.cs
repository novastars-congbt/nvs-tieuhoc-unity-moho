using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.DuaXe
{
    public class Answer : MonoBehaviour
    {
        [Header("UI Element")]
        [SerializeField] private TextMeshProUGUI _answerStringTMP;
        [SerializeField] private Button _answerBtn;

        private int _roadNumber;
        #region Public Method
        public void Setup(bool answer, string answerString, Action callBackAction)
        {
            _answerBtn.onClick.RemoveAllListeners();
            _answerBtn.onClick.AddListener(() => GameController.Instance.RunToRoad(_roadNumber, answer));
            _answerBtn.onClick.AddListener(() => callBackAction.Invoke());
            _answerStringTMP.text = answerString;
        }

        public void SetupRoadNumber(int roadNumber)
        {
            _roadNumber = roadNumber;
        }
        #endregion
    }
}