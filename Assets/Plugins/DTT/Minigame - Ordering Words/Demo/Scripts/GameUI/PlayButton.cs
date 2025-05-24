using UnityEngine;
using UnityEngine.UI;
using System;

namespace DTT.OrderingWords.Demo
{
    /// <summary>
    /// Handles the play button for the UI of the game.
    /// </summary>
    public class PlayButton : MonoBehaviour
    {
        /// <summary>
        /// Button component.
        /// </summary>
        private Button _button;

        /// <summary>
        /// Gets necessary components.
        /// </summary>
        private void Awake() => _button = GetComponent<Button>();

        /// <summary>
        /// Adds listeners.
        /// </summary>
        private void OnEnable() => _button.onClick.AddListener(OnPlayButtonPressed);

        /// <summary>
        /// Removes listeners.
        /// </summary>
        private void OnDisable() => _button.onClick.RemoveListener(OnPlayButtonPressed);

        /// <summary>
        /// Action for when the button is pressed;
        /// </summary>
        public event Action PlayButtonPressed;

        /// <summary>
        /// Plays the level of the game.
        /// </summary>
        private void OnPlayButtonPressed() => PlayButtonPressed?.Invoke();

    }
}

