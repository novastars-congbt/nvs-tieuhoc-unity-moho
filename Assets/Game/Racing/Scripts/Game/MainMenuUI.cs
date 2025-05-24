using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.DuaXe
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Image _gameTitleSprite;
        [SerializeField] private Image _mainMenuBackGroundSprite;
        [SerializeField] private Image _startButtonSprite;
        [SerializeField] private Image _settingButtonSprite;
        [Space]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingButton;

        #region Unity Funcion
        private void Start()
        {
            _mainMenuBackGroundSprite.sprite = UIManager.Instance.MainMenuBackGroundSprite;

            _gameTitleSprite.GetComponent<RectTransform>().sizeDelta = UIManager.Instance.GameTitleSprite.rect.size;
            _gameTitleSprite.sprite = UIManager.Instance.GameTitleSprite;

            _startButtonSprite.GetComponent<RectTransform>().sizeDelta = UIManager.Instance.PlaySprite.rect.size;
            _startButtonSprite.sprite = UIManager.Instance.PlaySprite;

            _settingButtonSprite.GetComponent<RectTransform>().sizeDelta = UIManager.Instance.SettingSprite.rect.size;
            _settingButtonSprite.sprite = UIManager.Instance.SettingSprite;

            _playButton.onClick.AddListener(PlayGame);
            _settingButton.onClick.AddListener(OpenSetting);

            _settingButton.gameObject.SetActive(UIManager.Instance.TurnOnSetting);
        }
        #endregion

        #region Private Method
        private void PlayGame()
        {
            var spawnClickVFX = FeedbackManager.Instance.SpawnClickButtonVFX();
            if (spawnClickVFX)
            {
                spawnClickVFX.transform.position = new Vector2(_playButton.transform.position.x, _playButton.transform.position.y);
            }

            FeedbackManager.Instance.PlayButtonClickSFX();
            BaseGameUI.Instance.GoToChooseCar();
        }

        private void OpenSetting()
        {
            var spawnClickVFX = FeedbackManager.Instance.SpawnClickButtonVFX();
            if (spawnClickVFX)
            {
                spawnClickVFX.transform.position = new Vector2(_settingButton.transform.position.x, _settingButton.transform.position.y);
            }

            FeedbackManager.Instance.PlayButtonClickSFX();
        }
        #endregion
    }
}