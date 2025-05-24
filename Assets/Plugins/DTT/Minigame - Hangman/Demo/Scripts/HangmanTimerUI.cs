using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// The timer user interface behaviour.
    /// </summary>
    public class HangmanTimerUI : MonoBehaviour
    {
        /// <summary>
        /// The hangman service.
        /// </summary>
        [SerializeField]
        private HangmanService _service;
        
        /// <summary>
        /// The text to display the time on.
        /// </summary>
        [SerializeField]
        private Text _text;
        
        /// <summary>
        /// The timer used to display.
        /// </summary>
        private HangmanTimer _timer;

        /// <summary>
        /// The hangman settings used for the game.
        /// </summary>
        private HangmanSettings _settings;
        
        /// <summary>
        /// The refresh interval in seconds.
        /// </summary>
        private const int INTERVAL_SECONDS = 1;
        
        /// <summary>
        /// Starts listening for the game to start.
        /// </summary>
        private void Awake()
        {
            _timer = GetComponent<HangmanTimer>();
            _service.Started += OnGameStart;
        }

        /// <summary>
        /// Called when the game is started to reset the text.
        /// </summary>
        private void OnGameStart() => _text.text = TimeSpan.FromSeconds(_service.Settings.TimeConstrained).ToString("mm':'ss");

        /// <summary>
        /// Starts updating the timer text.
        /// </summary>
        //private void Start() => StartCoroutine(UpdateTimerText());

        private void OnEnable() => StartCoroutine(UpdateTimerText());

        /// <summary>
        /// Updates the timer text each interval.
        /// </summary>
        private IEnumerator UpdateTimerText()
        {
            WaitForSeconds waitForInterval = new WaitForSeconds(INTERVAL_SECONDS);
            while (enabled)
            {
                yield return waitForInterval;
                
                string leftoverTime = (TimeSpan.FromSeconds(_service.Settings.TimeConstrained) - _timer.TimePassed).ToString("mm':'ss");
                _text.text = leftoverTime;
            }
        }
    }
}
