using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.LatBai
{
    public class SettingPopup : MonoBehaviour
    {
        [SerializeField] private Button _checkShufferButton;
        [SerializeField] private GameObject _checkedSuffer;
        [SerializeField] private GameObject _checkShufferGroup;

        [Space]
        [SerializeField] private Slider _cardNumberSlider;
        [SerializeField] private TextMeshProUGUI _cardNumberTMP;
        [SerializeField] private GameObject _cardNumberGroup;

        private int minCardNumber = 4;
        private int maxCardNumber = 20;
        public bool isSufferOn = true;

        #region Unity Funcion
        private void Start()
        {
            SetupToggle();
            SetupSlider();

            _checkShufferGroup.SetActive(UIManager.Instance.TurnOnSetting_SufferCard);
            _cardNumberGroup.SetActive(UIManager.Instance.TurnOnSetting_NumberCard);
        }
        #endregion

        #region Private Method
        private void SetupToggle()
        {
            _checkedSuffer.SetActive(isSufferOn);
            _checkShufferButton.onClick.AddListener(SetSufferOnOff);

            GameManager.Instance.IsSufferOn = isSufferOn;
        }

        private void SetSufferOnOff()
        {
            isSufferOn = !isSufferOn;
            _checkedSuffer.SetActive(isSufferOn);

            GameManager.Instance.IsSufferOn = isSufferOn;
        }

        private void SetupSlider()
        {
            int currentMaxCard = DataManager.Instance.dataCard.Cards.Length;
            int currentDefautCard = DataManager.Instance.dataCard.defaultCardAmount;
            if (currentDefautCard <= 0) currentDefautCard = currentMaxCard * 2;
            GameManager.Instance.CurrentCardAmount = currentDefautCard / 2;
            _cardNumberTMP.text = currentDefautCard.ToString();

            _cardNumberSlider.value = currentDefautCard / 2;
            _cardNumberSlider.minValue = minCardNumber / 2;
            _cardNumberSlider.maxValue = currentMaxCard > maxCardNumber / 2 ? maxCardNumber / 2 : currentMaxCard;

            _cardNumberSlider.onValueChanged.AddListener(OnCardNumberChange);

            GameManager.Instance.DoneSetupConfig = true;

            gameObject.SetActive(false);
        }

        private void OnCardNumberChange(float cardNumber)
        {
            GameManager.Instance.CurrentCardAmount = (int)cardNumber;
            _cardNumberTMP.text = ((int)(cardNumber * 2)).ToString();
        }
        #endregion
    }
}