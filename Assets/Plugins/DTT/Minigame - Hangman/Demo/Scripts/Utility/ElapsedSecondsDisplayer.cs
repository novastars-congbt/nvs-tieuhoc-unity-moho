using System;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// Displays the elapsed amount of seconds for the hangman game service.
    /// </summary>
    public class ElapsedSecondsDisplayer : MonoBehaviour
    {
        /// <summary>
        /// The game service.
        /// </summary>
        [SerializeField]
        private HangmanService _service;

        /// <summary>
        /// The graphic displaying the text.
        /// </summary>
        private Text _textGraphic;

        /// <summary>
        /// The text graphic component.
        /// </summary>
        private void Awake() => _textGraphic = GetComponent<Text>();
        
        /// <summary>
        /// Displays the elapsed amount of seconds on the text graphic
        /// if a time constrained is used for the game.
        /// </summary>
        private void Update()
        {
            HangmanSettings settings = _service.Settings;
            if (!settings.UsesTimeConstrained)
                return;
            
            TimeSpan timeLeft = TimeSpan.FromSeconds(settings.TimeConstrained) - TimeSpan.FromSeconds(_service.ElapsedSeconds);
            _textGraphic.text = timeLeft.ToString("mm\\:ss");
        }
    }
}