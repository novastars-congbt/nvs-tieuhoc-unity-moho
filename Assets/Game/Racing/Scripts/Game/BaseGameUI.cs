using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.DuaXe
{
    public class BaseGameUI : MonoBehaviour
    {
        private static BaseGameUI _instance;
        public static BaseGameUI Instance => _instance;

        [SerializeField] private GameObject _mainMenuPannel;
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private GameObject _chooseMenuPannel;
        [SerializeField] private GameObject _gamePlayPannel;

        [SerializeField] private GameObject _completeMissionPannel;
        [SerializeField] private TextMeshProUGUI _completeQuestionTMP;

        [Header("Script Reference")]
        [SerializeField] private MainMenuUI _mainMenuUI;
        [SerializeField] private SelectCarMenuUI _selectCarMenuUI;
        [SerializeField] private GameplayUI _gameplayUI;

        #region Unity Funcion
        private void OnEnable(){
            //if (MiniGameController.instance != null)
            //{
            //    MiniGameController.instance.actionRestartGame += ActionRestartGame;
            //}
            StartGame();
        }
        private void Awake()
        {
            _instance = this;
        }
        private void Start(){
            StartGame();
        }
        private void StartGame()
        {
            SetupMissionPannel();

            _completeMissionPannel.SetActive(UIManager.Instance.TurnOnDiscuss);
            _mainMenuPannel.SetActive(UIManager.Instance.TurnOnMainMenu);
            _gamePlayPannel.SetActive(!UIManager.Instance.TurnOnMainMenu && !UIManager.Instance.TurnOnChooseCar);

            if (!UIManager.Instance.TurnOnMainMenu)
            {
                _chooseMenuPannel.SetActive(UIManager.Instance.TurnOnChooseCar);
            }else
            {
                _chooseMenuPannel.SetActive(false);
            }
        }
        #endregion

        #region Public Method
        public GameplayUI GetGameplayUILogic() => _gameplayUI;

        public void GoToChooseCar()
        {
            _mainMenuPannel.SetActive(false);
            _chooseMenuPannel.SetActive(UIManager.Instance.TurnOnChooseCar);
            _gamePlayPannel.SetActive(!UIManager.Instance.TurnOnChooseCar);
        }

        public void GoToGame()
        {
            _mainMenuPannel.SetActive(false);
            _chooseMenuPannel.SetActive(false);
            _gamePlayPannel.SetActive(true);
        }
        #endregion

        #region Private Method
        private void SetupMissionPannel()
        {
            var questionDatas = DataManager.Instance.dataQuestions.QuestionDatas;

            StringBuilder pannelText = new StringBuilder();
            for (int questionIndex = 0; questionIndex < questionDatas.Count; questionIndex++)
            {
                pannelText.AppendLine($"<color=red><b>{questionIndex + 1}.</b></color> {questionDatas[questionIndex].QuestionString}");

                if (UIManager.Instance.TurnOnDiscussAnswer)
                {
                    pannelText.AppendLine($"<color=green><b>A.</b></color> {questionDatas[questionIndex].AnswerAString}");
                    pannelText.AppendLine($"<color=green><b>B.</b></color> {questionDatas[questionIndex].AnswerBString}");
                    pannelText.AppendLine();
                }
            }

            _completeQuestionTMP.text = pannelText.ToString();
        }
        #endregion

        void ActionRestartGame()
        {
            _mainMenuPannel.SetActive(false);
            _chooseMenuPannel.SetActive(true);
        }
    }
}