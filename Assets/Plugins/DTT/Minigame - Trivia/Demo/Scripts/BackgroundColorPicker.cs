using UnityEngine;
using UnityEngine.UI;

namespace DTT.Trivia.Demo
{
    /// <summary>
    /// Chooses a different background color for each question.
    /// </summary>
    public class BackgroundColorPicker : MonoBehaviour
    {
        /// <summary>
        /// Array for the colors.
        /// </summary>
        [SerializeField]
        private Color[] _colorArray;

        /// <summary>
        /// Image component of the background.
        /// </summary>
        [SerializeField]
        private Image _backgroundImage;

        /// <summary>
        ///Background pattern;
        /// </summary>
        [SerializeField]
        private RawImage _pattern;

        /// <summary>
        /// Game manager.
        /// </summary>
        [SerializeField]
        private GameManager _gameManager;

        /// <summary>
        /// The index of the current color element in the array.
        /// </summary>
        private int _currentColorIndex;

        /// <summary>
        /// Set the initial color when the script loads.
        /// </summary>
        private void Awake() => ChangeColor();

        /// <summary>
        /// Handles event subscribing.
        /// </summary>
        private void OnEnable() => _gameManager.NextQuestion += ChangeColor;

        /// <summary>
        /// Handles event unsubscribing.
        /// </summary>
        private void OnDisable() => _gameManager.NextQuestion -= ChangeColor;

        /// <summary>
        /// Change the backround color to the next color in the array.
        /// </summary>
        private void ChangeColor()
        {
            _backgroundImage.color = _colorArray[_currentColorIndex];

            _currentColorIndex++;
            if (_currentColorIndex >= _colorArray.Length)
                _currentColorIndex = 0;
        }

        /// <summary>
        /// Moves the pattern.
        /// </summary>
        private void FixedUpdate()
        {
            _pattern.uvRect = new Rect(_pattern.uvRect.x + 0.0005f, _pattern.uvRect.y + 0.0005f, 1,1);
        }
    }
}