using UnityEngine;
using UnityEngine.UI;
using System;

namespace DTT.OrderingWords.Demo
{
    /// <summary>
    /// Handles the button for restarting the game.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class RestartButton : MonoBehaviour
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
        private void OnEnable() => _button.onClick.AddListener(OnRestartButtonPressed);

        /// <summary>
        /// Removes listeners.
        /// </summary>
        private void OnDisable() => _button.onClick.RemoveListener(OnRestartButtonPressed);

        /// <summary>
        /// Action for when the button is pressed;
        /// </summary>
        public event Action RestartButtonPressed;

        /// <summary>
        /// Plays the level of the game.
        /// </summary>
        private void OnRestartButtonPressed() => RestartButtonPressed?.Invoke();
    }
}