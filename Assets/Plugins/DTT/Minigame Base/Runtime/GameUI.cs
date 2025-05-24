using DTT.MinigameBase.LevelSelect;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.MinigameBase.UI
{
    /// <summary>
    /// Standardized user interface for minigames.
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        /// <summary>
        /// The popup used for pausing and finishing.
        /// </summary>
        //[SerializeField]
        public GamePopupUI _popup;

        /// <summary>
        /// The button for pausing the game.
        /// </summary>
        [SerializeField]
        private Button _pauseButton;

        /// <summary>
        /// The game object on which the minigame is placed.
        /// </summary>
        [SerializeField]
        private GameObject _minigameGameObject;

        /// <summary>
        /// The minigame instance.
        /// </summary>
        private IMinigame _minigame;
        
        /// <summary>
        /// The part of the minigame that can be restarted.
        /// </summary>
        private IRestartable _restartable;
        
        /// <summary>
        /// The part of the minigame that will let us know when it's finished.
        /// </summary>
        private IFinishedable _finishedable;
        
        /// <summary>
        /// The level select currently active.
        /// </summary>
        private LevelSelectHandlerBase _levelSelectHandler;

        public bool isWin;
        public Action actionNext;
        public Action actionRestart;

        /// <summary>
        /// Retrieves interface implementations.
        /// </summary>
        private void Awake()
        {
            _minigame = _minigameGameObject.GetComponent<IMinigame>();
            _restartable = _minigameGameObject.GetComponent<IRestartable>();
            _finishedable = _minigameGameObject.GetComponent<IFinishedable>();
            _levelSelectHandler = FindObjectOfType<LevelSelectHandlerBase>();
        }

        /// <summary>
        /// Subscribes to events.
        /// </summary>
        private void OnEnable()
        {
            CancelInvoke("RestartGame");
            Invoke("RestartGame", 0.1f);
            //RestartGame();
            _pauseButton.onClick.AddListener(PauseGame);
            _finishedable.Finished += FinishGame;
            _popup.ResumeButtonPressed += ResumeGame;
            _popup.RestartButtonPressed += ActionRestart;
            //_popup.RestartButtonPressed += RestartGame;
            _popup.HomeButtonPressed += QuitGame;
            _popup.NextButtonPressed += ActionNext;
            //_popup.NextButtonPressed += RestartGame;
            Debug.LogError("=========== enable gameui");
        }

        /// <summary>
        /// Removes events.
        /// </summary>
        private void OnDisable()
        {
            _pauseButton.onClick.RemoveListener(PauseGame);
            _finishedable.Finished -= FinishGame;
            _popup.ResumeButtonPressed -= ResumeGame;
            _popup.RestartButtonPressed -= ActionRestart;
            //_popup.RestartButtonPressed -= RestartGame;
            _popup.HomeButtonPressed -= QuitGame;
            _popup.NextButtonPressed -= ActionNext;
            //_popup.NextButtonPressed -= RestartGame;
            Debug.LogError("=========== disable gameui");
        }

        /// <summary>
        /// Sets the UI in a state for when the game finishes.
        /// </summary>
        private void QuitGame()
        {
            Application.Quit();
        }
        private void FinishGame()
        {
            _popup.Show(true);
            if (isWin)
            {
                _popup.SetTitleToFinished();
                _popup.EnableResumeButton(false);
                _popup.EnableRestartButton(true);
                _popup.EnableNextButton(true);
                _popup.EnableHomeButton(false);
                if (MiniGameEndController.instance != null) MiniGameEndController.instance.PlayParticle(MiniGameEndController.instance.particleMiniWin, MiniGameEndController.instance.audioParticleMiniWin);
            }
            else
            {
                _popup.SetTitleToLose();
                _popup.EnableResumeButton(false);
                _popup.EnableRestartButton(true);
                _popup.EnableNextButton(false);
                _popup.EnableHomeButton(false);
            }
        }

        /// <summary>
        /// Sets the UI in a state for when the game restarts.
        /// </summary>
        public void RestartGame()
        {
            if (MiniGameEndController.instance != null)
            {
                MiniGameEndController.instance.isWin = false;
            }
            isWin = false;

            _minigame.Continue();
            
            _restartable.Restart();
            
            _popup.Show(false);

            Debug.LogError("============= Restart Game");
        }

        /// <summary>
        /// Sets the UI in a state for when the game resumes.
        /// </summary>
        private void ResumeGame()
        {
            _minigame.Continue();
            _popup.Show(false);
        }

        /// <summary>
        /// Sets the UI in a state for when the game resumes.
        /// </summary>
        private void PauseGame()
        {
            _minigame.Pause();
            _popup.Show(true);
            _popup.SetTitleToPaused();
            _popup.EnableResumeButton(true);
            _popup.EnableRestartButton(true);
            _popup.EnableNextButton(false);
        }

        /// <summary>
        /// Sets the UI in a state for when the game goes back to home.
        /// </summary>
        private void ToHome()
        {
            _popup.Show(false);
            _levelSelectHandler.ShowLevelSelect();
        }

        private void ActionNext()
        {
            if (MiniGameEndController.instance != null)
            {
                if (MiniGameEndController.instance.isWin)
                {
                    _popup.Show(false);
                    MiniGameEndController.instance.ShowGameEnd();
                }
                else
                {
                    gameObject.SetActive(false);
                    gameObject.SetActive(true);
                }
            }
            else
            {
                gameObject.SetActive(false);
                gameObject.SetActive(true);
            }
            actionNext?.Invoke();
        }
        private void ActionRestart()
        {
            actionRestart?.Invoke();
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }
}