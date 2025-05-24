using DTT.MiniGame.Anagram.UI;
using System;
using System.Collections;
using DTT.MinigameBase;
using UnityEngine;
using DTT.MinigameBase.UI;
using DTT.MinigameBase.Timer;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using static DTT.MiniGame.Anagram.AnagramConfig;

namespace DTT.MiniGame.Anagram
{
    /// <summary>
    /// Handles the game loop of the anagram game.
    /// </summary>
    public class AnagramManager : MonoBehaviour, IMinigame<AnagramConfig, AnagramResult>, IFinishedable, IRestartable
    {
        /// <summary>
        /// Default config of the game.
        /// </summary>
        [SerializeField]
        [LabelText("Data")]
        [Tooltip("Default config of the game.")]
        private AnagramConfig _config;

        /// <summary>
        /// UI component of the game.
        /// </summary>
        [SerializeField]
        [Tooltip("UI component of the game.")]
        private AnagramUI _anagramUI;

        /// <summary>
        /// Timer of the game.
        /// </summary>
        [SerializeField]
        [Tooltip("Timer component")]
        private Timer _timer;

        /// <summary>
        /// Whether the game should start once Awake is called.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether the game should start once Awake is called.")]
        private bool _startOnAwake;

        /// <summary>
        /// Invoked when a word has been filled in.
        /// </summary>
        public event Action<AnagramResult> Finish;

        /// <summary>
        /// Invoked when all letters are filled into the field.
        /// </summary>
        public event Action<string> FilledWord;

        /// <summary>
        /// Invoked when the game successfully started.
        /// </summary>
        public event Action Started;

        /// <summary>
        /// Invoked when the game is finished.
        /// </summary>
        public event Action Finished;

        /// <summary>
        /// Whether the game is currently paused or not.
        /// </summary>
        public bool IsPaused => _isPaused;

        /// <summary>
        /// Whether the game is currently active.
        /// </summary>
        public bool IsGameActive => _isGameActive;

        /// <summary>
        /// Possible anagrams for the current game.
        /// </summary>
        public AnagramCollection CurrentAnagrams => _currentAnagrams;

        /// <summary>
        /// Whether the game is currently paused or not.
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// Whether the game is currently active.
        /// </summary>
        private bool _isGameActive;

        /// <summary>
        /// Number of words in the stage.
        /// </summary>
        [SerializeField][ReadOnly]
        public int amount;

        /// <summary>
        /// Handles generating a random list of anagrams.
        /// </summary>
        private AnagramGenerator _generator = new AnagramGenerator();

        /// <summary>
        /// Possible anagrams for the current game.
        /// </summary>
        private AnagramCollection _currentAnagrams = new AnagramCollection();

        /// <summary>
        /// GameUI reference.
        /// </summary>
        [SerializeField]
        private GameUI gameui;

        /// <summary>
        /// List of displayed words.
        /// </summary>
        [SerializeField][ReadOnly]
        public List<string> words = new List<string>();

        /// <summary>
        /// Starts the game.
        /// </summary>
        // private IEnumerator Start()
        // {
        //     yield return new WaitForEndOfFrame();
        //     if (_startOnAwake)
        //         StartGame();
        // }

        /// <summary>
        /// Adds listeners.
        /// </summary>
        private void OnEnable(){
            _anagramUI.WordComplete += CheckCompletion;
            //amount = 0;
            //words.Clear();
            // if (MiniGameEndController.instance != null)
            // {
            //     MiniGameEndController.instance.actionRestartGame -= ActionRestartGame;
            //     MiniGameEndController.instance.actionRestartGame += ActionRestartGame;
            // }
            gameui.actionNext += ActionNextLevel;
            gameui.actionRestart += ActionRestartLevel;
            //StopGame();
            //StartGame();
        }

        /// <summary>
        /// Removes listeners.
        /// </summary>
        private void OnDisable()
        {
            _anagramUI.WordComplete -= CheckCompletion;
            gameui.actionNext -= ActionNextLevel;
            gameui.actionRestart -= ActionRestartLevel;
        }

        /// <summary>
        /// Starts the game using the default config.
        /// </summary>
        public void StartGame() => StartGame(_config);

        /// <summary>
        /// Starts the game.
        /// </summary>
        /// <param name="config">Config to be used for the game.</param>
        public void StartGame(AnagramConfig config)
        {
            _config = config;
            _isPaused = false;

            _timer.Begin();

            if(amount == 0)_generator.Reset();
            _currentAnagrams = _generator.Generate(config);
            
            if (_currentAnagrams.Count == 0)
                return;

            _anagramUI.GenerateUI(_currentAnagrams[0]);

            _isGameActive = true;
            Started?.Invoke();
        }

        /// <summary>
        /// Pauses the game, stops the timer and disables interaction.
        /// </summary>
        public void Pause()
        {
            if (!IsGameActive)
                return;

            _isPaused = true;
            _timer.Pause();
            _anagramUI.SetInteractable(false);
        }

        /// <summary>
        /// Continues the game from the paused state.
        /// </summary>
        public void Continue()
        {
            if (!IsGameActive || !_isPaused)
                return;

            _isPaused = false;
            _timer.Resume();
            _anagramUI.SetInteractable(true);
        }

        /// <summary>
        /// Stops the game immediatly.
        /// </summary>
        public void ForceFinish()
        {
            Finish?.Invoke(new AnagramResult("", _anagramUI.LettersMoved, _timer.TimePassed.Seconds));
            Ending();
            StopGame();
        }

        /// <summary>
        /// Restarts the game.
        /// </summary>
        public void Restart()
        {
            StopGame();
            StartGame();
        }

        private void Ending(){
            if (MiniGameEndController.instance != null){
                MiniGameEndController.instance.totalAnswer = words.Count;
                for (int i = 0; i < words.Count; i++){
                    MiniGameEndController.instance.SetTextForAnswer(i, words[i]);
                }
                MiniGameEndController.instance.isWin = true;
                words.Clear();
                //MiniGameEndController.instance.Play("win", gameui.transform.position);
            }
        }
        /// <summary>
        /// Checks if the word is valid or not.
        /// Invokes the finish event.
        /// </summary>
        /// <param name="word">Completed word.</param>
        private void CheckCompletion(string word)
        {
            FilledWord?.Invoke(word);

            if (!_currentAnagrams.Contains(word))
                return;

            // Play audio for the completed word
            var wordAudioPair = _config.WordAudioPairs.Find(pair => pair.word == word);
                //if(MiniGameEndController.instance != null)
                    //if (wordAudioPair != null && wordAudioPair.audioClip != null)
                    //    StartCoroutine(MiniGameEndController.instance.Play("win", gameui.transform.position, wordAudioPair.audioClip));
                    //else 
                    //    StartCoroutine(MiniGameEndController.instance.Play("win", gameui.transform.position));

            gameui.isWin = true;
            //Finished?.Invoke();
            //Finish?.Invoke(new AnagramResult(word, _anagramUI.LettersMoved, _timer.TimePassed.Seconds));
            StopGame();
            words.Add(word);
            if (amount == _config.WordAudioPairs.Count - 1){
                //amount += 1;
                //words.Add(word);
                Ending();
            }
            //else{
            //    amount += 1;
            //    words.Add(word);
            //}  
            if (gameui.isWin) StartCoroutine(DelayFinished(word));
            else
            {
                Finished?.Invoke();
                Finish?.Invoke(new AnagramResult(word, _anagramUI.LettersMoved, _timer.TimePassed.Seconds));
            }
        }

        IEnumerator DelayFinished(string word)
        {
            if (_config.WordAudioPairs[amount].audioClip != null)
            {
                if (MiniGameEndController.instance != null)
                {
                    MiniGameEndController.instance.PlayAudio(_config.WordAudioPairs[amount].audioClip);
                    yield return new WaitForSeconds(_config.WordAudioPairs[amount].audioClip.length);
                }
            }
            else yield return null;
            Finished?.Invoke();
            Finish?.Invoke(new AnagramResult(word, _anagramUI.LettersMoved, _timer.TimePassed.Seconds));
        }

        /// <summary>
        /// Stops the interactions and timer of the game.
        /// </summary>
        private void StopGame()
        {
            _timer.Stop();
            _isGameActive = false;
            _anagramUI.SetInteractable(false);
        }

        void ActionNextLevel()
        {
            if (amount < _config.WordAudioPairs.Count - 1) amount++;
            else amount = 0;
        }

        void ActionRestartLevel()
        {

        }

        void ActionNextGame()
        {

        }

        void ActionRestartGame()
        {
            amount = 0;
        }
    }
}