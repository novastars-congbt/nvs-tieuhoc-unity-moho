using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.DuaXe
{
    public class SelectCarMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject _carSelectPrefabs;
        [SerializeField] private Transform _carSelectParrentTrans;

        [SerializeField] private Image _carSelectBackground;
        [SerializeField] private Image __continueImg;
        [SerializeField] private Button _continueBtn;

        #region Unity Funcion
        private void Awake()
        {
            SetupCarSelect();
        }

        private void Start()
        {
            _carSelectBackground.sprite = UIManager.Instance.ChooseCarBackgroundSprite;
            __continueImg.sprite = UIManager.Instance.ContinueSprite;
            _continueBtn.onClick.AddListener(PlayGame);
        }
        #endregion

        #region Private Method
        private void PlayGame()
        {
            var spawnClickVFX = FeedbackManager.Instance.SpawnClickButtonVFX();
            if (spawnClickVFX)
            {
                spawnClickVFX.transform.position = new Vector2(_continueBtn.transform.position.x, _continueBtn.transform.position.y);
            }

            FeedbackManager.Instance.PlayButtonClickSFX();
            BaseGameUI.Instance.GoToGame();
        }

        private void SetupCarSelect()
        {
            var carSelects = UIManager.Instance.CarsSelectSprites;
            var carNumbers = UIManager.Instance.CarsSelectNumbers;

            for (int carSpriteIndex = 0; carSpriteIndex < carSelects.Count; carSpriteIndex++)
            {
                var carSelect = Instantiate(_carSelectPrefabs, _carSelectParrentTrans).GetComponent<CarSelect>();
                carSelect.SetupCarSelect(carNumbers[carSpriteIndex], carSelects[carSpriteIndex]);
            }
        }
        #endregion
    }
}