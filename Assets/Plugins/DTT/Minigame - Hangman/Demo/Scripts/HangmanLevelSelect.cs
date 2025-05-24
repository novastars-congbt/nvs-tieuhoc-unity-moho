using DTT.MinigameBase.LevelSelect;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// Handles level select for the hangman minigame.
    /// </summary>
    public class HangmanLevelSelect : LevelSelectHandler<HangmanSettings, HangmanResult, HangmanService>
    {
        /// <summary>
        /// The configurations to use for each level.
        /// </summary>
        [SerializeField]
        private HangmanSettings[] _configs;

        /// <summary>
        /// Fired when the level is exited.
        /// </summary>
        [FormerlySerializedAs("LevelExit")]
        [SerializeField]
        private UnityEvent _levelExit;

        /// <summary>
        /// Fired when the level is entered.
        /// </summary>
        [FormerlySerializedAs("levelEnter")]
        [SerializeField]
        private UnityEvent _levelEnter;

        /// <summary>
        /// The current active settings for the level.
        /// </summary>
        private HangmanSettings _currentSettings;

        /// <summary>
        /// Starts listening to level selection events.
        /// </summary>
        private void Start()
        {
            // Invoke serialized unity events to avoid hangman service to 
            // be tightly coupled with the level selection. 
            LevelSelectOpened += _levelExit.Invoke;
            LevelSelectClosed += _levelEnter.Invoke;
        }

        /// <summary>
        /// Returns the configuration for the given level.
        /// </summary>
        /// <param name="levelNumber">The level number.</param>
        /// <returns>The settings to use.</returns>
        protected override HangmanSettings GetConfig(int levelNumber)
        {
            HangmanSettings selectedSettings;
            if (levelNumber > _configs.Length)
                selectedSettings = _configs[_configs.Length - 1];
            else
                selectedSettings = _configs[levelNumber - 1];

            _currentSettings = selectedSettings;
            return _currentSettings;
        }

        /// <summary>
        /// Calculates the score for a hangman game result.
        /// </summary>
        /// <param name="result">The hangman game result.</param>
        /// <returns>The calculated score.</returns>
        protected override float CalculateScore(HangmanResult result)
            => result.finishedPhrase ? 1f - (result.wrongGuesses / (float)_currentSettings.Lives) : 0.0f;
    }
}
