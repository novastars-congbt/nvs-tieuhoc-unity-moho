using DTT.MinigameBase.Timer;
using UnityEngine;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// The timer used for the hangman minigame.
    /// </summary>
    public class HangmanTimer : Timer
    {
        /// <summary>
        /// The game service.
        /// </summary>
        [SerializeField]
        private HangmanService _service;
        
        /// <summary>
        /// Starts listening for gameplay events.
        /// </summary>
        private void OnEnable()
        {
            _service.Started += Begin;
            _service.Paused += OnPause;
            _service.Finished += Stop;
        }

        /// <summary>
        /// Called when the pause state of the game changes to either stop or resume the timer.
        /// </summary>
        /// <param name="value">The new state value.</param>
        private void OnPause(bool value)
        {
            if(value)
                Stop();
            else
                Resume();
        }

        /// <summary>
        /// Stops listening to gameplay events.
        /// </summary>
        private void OnDisable()
        {
            _service.Started -= Begin;
            _service.Paused -= OnPause;
            _service.Finished -= Stop;
        }
    }
}
