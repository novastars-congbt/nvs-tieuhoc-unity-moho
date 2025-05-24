using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.LatBai
{
    public class BaseGameUI : MonoBehaviour
    {
        private static BaseGameUI _instance;
        public static BaseGameUI Instance => _instance;

        [SerializeField] private Image _backGroundSprite;

        [SerializeField] private GameObject _mainMenuPannel;
        [SerializeField] private GameObject _gamePlayPannel;

        [SerializeField] private GameObject _completeMissionPannel;
        [SerializeField] private TextMeshProUGUI _completeMissionTMP;

        #region Unity Funcion
        private void Awake()
        {
            _instance = this;

            _mainMenuPannel.SetActive(true);
            _gamePlayPannel.SetActive(false);
        }

        private void OnEnable()
        {
            GoToMenu();
        }

        private void Start()
        {
            _backGroundSprite.sprite = UIManager.Instance.BackGroundSprite;
            _completeMissionPannel.SetActive(UIManager.Instance.TurnOnMissonOutGame);

            SetupMissionPannel();

            if (FeatureManager.Instance.PassMainMenu) GoToGame();
        }
        #endregion

        #region Public Method
        public void GoToGame()
        {
            _mainMenuPannel.SetActive(false);
            _gamePlayPannel.SetActive(true);

            FeedbackManager.Instance.PlayBackgroundMusic();
        }

        public void GoToMenu()
        {
            _mainMenuPannel.SetActive(true);
            _gamePlayPannel.SetActive(false);
        }
        #endregion

        #region Private Method
        private void SetupMissionPannel()
        {
            var missionDatas = DataManager.Instance.MissionDatas;

            StringBuilder pannelText = new StringBuilder();
            for (int missionIndex = 0; missionIndex < missionDatas.Count; missionIndex++)
            {
                pannelText.AppendLine($"<color=red><b>{missionIndex + 1}.</b></color> {missionDatas[missionIndex].MissionQuestion}");
            }

            _completeMissionTMP.text = pannelText.ToString();
        }
        #endregion
    }
}