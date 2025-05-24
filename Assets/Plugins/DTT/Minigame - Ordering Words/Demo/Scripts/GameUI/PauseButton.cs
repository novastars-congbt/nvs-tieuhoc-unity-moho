using UnityEngine;
using UnityEngine.UI;
using System;

namespace DTT.OrderingWords.Demo
{
    /// <summary>
    /// Handles the pause button for the game UI.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class PauseButton : MonoBehaviour
    {
        /// <summary>
        /// Icon of the button.
        /// </summary>
        [SerializeField]
        [Tooltip("Icon of the button")]
        private Image _icon;

        /// <summary>
        /// Pause image.
        /// </summary>
        [SerializeField]
        [Tooltip("Pause image")]
        private Sprite _pauseImage;

        /// <summary>
        /// Play image.
        /// </summary>
        [SerializeField]
        [Tooltip("Play image")]
        private Sprite _resume;

        /// <summary>
        /// Action for when the button is pressed;
        /// </summary>
        public event Action<bool> PauseButtonPressed;

        /// <summary>
        /// Button component.
        /// </summary>
        private Button _button;

        /// <summary>
        /// The current state of the pause button.
        /// </summary>
        private bool _paused = false;

        /// <summary>
        /// Gets necessary components.
        /// </summary>
        private void Awake() => _button = GetComponent<Button>();

        /// <summary>
        /// Adds listeners.
        /// </summary>
        private void OnEnable() => _button.onClick.AddListener(OnPauseButtonPressed);

        /// <summary>
        /// Removes listeners.
        /// </summary>
        private void OnDisable() => _button.onClick.RemoveListener(OnPauseButtonPressed);

        /// <summary>
        /// Toggles the pause state of the game.
        /// </summary>
        private void OnPauseButtonPressed()
        {
            _paused = !_paused;
            PauseButtonPressed?.Invoke(_paused);

            _icon.sprite = _paused ? _resume : _pauseImage;
        }
    }
}