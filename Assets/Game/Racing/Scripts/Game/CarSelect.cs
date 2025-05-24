using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.DuaXe
{
    public class CarSelect : MonoBehaviour
    {
        [SerializeField] private Button _selectBtn;
        [SerializeField] private Image _carNumber;
        [SerializeField] private Image _carImg;

        #region Unity Funcion
        private void Start()
        {
            _selectBtn.onClick.AddListener(Select);
        }
        #endregion

        #region Public Method
        public void SetupCarSelect(Sprite carNumberSprite, Sprite carSprite)
        {
            _carNumber.sprite = carNumberSprite;
            _carImg.sprite = carSprite;
            _carImg.GetComponent<RectTransform>().sizeDelta = carSprite.rect.size;
        }
        #endregion

        #region Private Method
        private void Select()
        {
            GameManager.Instance.CurrentCarSprite = _carImg.sprite;
        }
        #endregion
    }
}