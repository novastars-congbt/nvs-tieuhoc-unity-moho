using DTT.MinigameBase;
using DTT.MinigameBase.Timer;
using System;
using DTT.MinigameBase.UI;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections;
//using Sirenix.OdinInspector.Editor.ActionResolvers;

namespace DTT.OrderingWords
{
    /// <summary>
    /// Manager class for the ordering words minigame, handles starting, finishing, pausing and resuming the game.
    /// </summary>
    public class GameManager : MonoBehaviour, IMinigame<GameSettings, OrderingWordsResult>, IRestartable, IFinishedable
    {
        /// <summary>
        /// The current settings for the minigame.
        /// </summary>
        [LabelText("Data")]
        public GameSettings _gameSettings;

        /// <summary>
        /// Reference to the ordering words controller.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the ordering words controller")]
        private OrderingWordsBoard _board;

        /// <summary>
        /// Reference to the game timer.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the play button")]
        private Timer _timer;

        /// <summary>
        /// Is called when the game has started.
        /// </summary>
        public event Action Started;

        /// <summary>
        /// Is called when the game has been Paused;
        /// </summary>
        public event Action Paused;

        /// <summary>
        /// Is called when the game has finished.
        /// </summary>
        public event Action<OrderingWordsResult> Finish;
        
        /// <summary>
        /// Is called when the game has finished.
        /// </summary>
        public event Action Finished;

        /// <summary>
        /// Is called when the level has been finished;
        /// </summary>
        public event Action LevelEnded;

        /// <summary>
        /// Is true when the game is paused.
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Is true when the game has started and isn't finished.
        /// </summary>
        public bool IsGameActive => !IsPaused;

        /// <summary>
        /// The current level for the game.
        /// </summary>
        [SerializeField]
        private int _currentLevel = 0;
        
        [SerializeField]
        public GameUI gameUI;
        [SerializeField]
        private AudioSource _audio;
        [SerializeField] 
        private AudioClip _audioClipCorrect;
        [SerializeField][ReadOnly]
        public List<string> words = new List<string>();

        /// <summary>
        /// Subscribe to events.
        /// </summary>
        private void OnEnable(){
            _board.CorrectAnswerGiven += FinishLevel;
            //StartGame(_gameSettings);
            //StartGame(_gameSettings);
            // if (MiniGameEndController.instance != null)
            // {
            //     MiniGameEndController.instance.actionRestartGame -= ActionRestartGame;
            //     MiniGameEndController.instance.actionRestartGame += ActionRestartGame;
            // }
            gameUI.actionNext += ActionNextLevel;
            gameUI.actionRestart += ActionRestartLevel;
        }

        /// <summary>
        /// Unsubscribe to events.
        /// </summary>
        private void OnDisable()
        {
            _board.CorrectAnswerGiven -= FinishLevel;
            gameUI.actionNext -= ActionNextLevel;
            gameUI.actionRestart -= ActionRestartLevel;
        }

        /// <summary>
        /// Start Ordering Words Minigame.
        /// </summary>
        public void StartGame(GameSettings settings)
        {
            _timer.Stop();
            _timer.Begin();
            IsPaused = false;
            _board.Clear();
            _gameSettings = settings;
            _currentLevel = settings.StartOnLevel;
            StartLevel(_currentLevel);
        }

        /// <summary>
        /// Stops the game activities and timer.
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
            Paused?.Invoke();
            _timer.Stop();
            _board.SetUserInput(false, false);
        }

        /// <summary>
        /// Continues the game.
        /// </summary>
        public void Continue()
        {
            _timer.Resume();
            _board.SetUserInput(true, false);
            IsPaused = false;
        }

        /// <summary>
        /// Restarts the current game.
        /// </summary>
        public void Restart()
        {
            //words.Clear();
            //_currentLevel = 0;
            _board.Clear();
            _timer.Begin();
            StartLevel(_currentLevel);
        }

        /// <summary>
        /// Starts the current level of the minigame.
        /// </summary>
        /// <param name="level">The level of the game to set up</param>
        public void StartLevel(int level)
        {
            Debug.LogError("Start Level " + level);
            _currentLevel = level;
            _board.IncorrectColor = _gameSettings.IncorrectColor;
            _board.SetSentence(_gameSettings.LevelSettings[level].Sentence, _gameSettings.LevelSettings[level].DraggableWords, _gameSettings.PossibleWordColor);
            Started?.Invoke();
        }

        /// <summary>
        /// Finished the current level.
        /// </summary>
        public void FinishLevel()
        {
            //LevelEnded?.Invoke();
            _board.SetUserInput(false, true);

            gameUI.isWin = true;
            _timer.Pause();
            //Finish?.Invoke(new OrderingWordsResult(_timer.TimePassed.Seconds, _gameSettings.LevelSettings.Length));
            //Finished?.Invoke();
            words.Add(_gameSettings.LevelSettings[_currentLevel].Sentence);
            if (_currentLevel >= _gameSettings.LevelSettings.Length - 1)
            {
                Ending();
            }
            /*if (gameUI.isWin)*/ StartCoroutine(DelayFinished());
            // else
            // {
            //     Finish?.Invoke(new OrderingWordsResult(_timer.TimePassed.Seconds, _gameSettings.LevelSettings.Length));
            //     Finished?.Invoke();
            // }
            //if (MiniGameEndController.instance != null) StartCoroutine(MiniGameEndController.instance.Play("win", gameUI.transform.position, _gameSettings.LevelSettings[_currentLevel].Audio));


        }

        IEnumerator DelayFinished()
        {
            if (_audioClipCorrect != null)
            {
                _audio.PlayOneShot(_audioClipCorrect);
                yield return new WaitForSeconds(_audioClipCorrect.length);
            }
            yield return new WaitForSeconds(1);
            if (_gameSettings.LevelSettings[_currentLevel].Audio != null)
            {
                _audio.PlayOneShot(_gameSettings.LevelSettings[_currentLevel].Audio);
                yield return new WaitForSeconds(_gameSettings.LevelSettings[_currentLevel].Audio.length);
            }
            Finish?.Invoke(new OrderingWordsResult(_timer.TimePassed.Seconds, _gameSettings.LevelSettings.Length));
            Finished?.Invoke();
        }

        private void Ending(){
            if (MiniGameEndController.instance != null){
                MiniGameEndController.instance.totalAnswer = words.Count;
                for (int i = 0; i < words.Count; i++){
                    MiniGameEndController.instance.SetTextForAnswer(i, words[i]);
                }
                MiniGameEndController.instance.isWin = true;
                words.Clear();
                //MiniGameEndController.instance.Play("win", gameUI.transform.position);
            }
        }
        /// <summary>
        /// Proceeds to the next level of the game.
        /// </summary>
        public void NextLevel()
        {
            // if (_currentLevel >= _gameSettings.LevelSettings.Length - 1)
            //     return;
            
            // _currentLevel++;
            _board.Clear();
            StartLevel(_currentLevel);
        }

        /// <summary>
        /// Finishes the current game.
        /// </summary>
        public void ForceFinish()
        {
            FinishLevel();
            if (_currentLevel < _gameSettings.LevelSettings.Length - 1)
            {
                Ending();
                _timer.Pause();
                Finish?.Invoke(new OrderingWordsResult(_timer.TimePassed.Seconds, _gameSettings.LevelSettings.Length));
                Finished?.Invoke();
            }
        }

        void ActionNextLevel()
        {
            if (_currentLevel < _gameSettings.LevelSettings.Length - 1) _currentLevel++;
            else _currentLevel = 0;
        }

        void ActionRestartLevel()
        {

        }

        void ActionNextGame()
        {

        }

        void ActionRestartGame()
        {
            _currentLevel = 0;
            Debug.LogError("================= co restart");
        }
    }
}

