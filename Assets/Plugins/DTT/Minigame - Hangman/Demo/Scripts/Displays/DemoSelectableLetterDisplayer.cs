using System;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.Hangman
{
    /// <summary>
    /// A behaviour displaying a selectable letter in the demo scene.
    /// </summary>
    public class DemoSelectableLetterDisplayer : SelectableLetterDisplayer
    {
        /// <summary>
        /// Button handling the click event.
        /// </summary>
        [SerializeField]
        private Button _button;

        /// <summary>
        /// Component displaying the letter.
        /// </summary>
        [SerializeField]
        private Text _text;

        /// <summary>
        /// Background image of the letter.
        /// </summary>
        [SerializeField]
        private Image _image;

        /// <summary>
        /// The tint of the letter after being pressed.
        /// </summary>
        [SerializeField]
        private Color _selectedTint = Color.grey;

        /// <summary>
        /// The time it takes to fade to the selected state.
        /// </summary>
        [SerializeField]
        private float _fadeTime = .5f;

        /// <summary>
        /// The initial tint of the letter background.
        /// </summary>
        private Color _startTint;

        /// <summary>
        /// The current fade time.
        /// </summary>
        private float _currentFadeTime = 0;

        /// <summary>
        /// Gets the initial tint of the background image.
        /// </summary>
        private void Awake() => _startTint = _image.color;

        /// <summary>
        /// Fades the letter over time.
        /// </summary>
        private void Update()
        {
            _currentFadeTime = Mathf.Clamp(_currentFadeTime, 0, _fadeTime);

            if (IsSelected && _currentFadeTime <= _fadeTime)
            {
                _image.color = Color.Lerp(_startTint, _selectedTint, _currentFadeTime / _fadeTime);
                _currentFadeTime += Time.deltaTime;
            }
        }

        /// <summary>
        /// Sets up the onClick listener on the button and displays the letter.
        /// </summary>
        /// <param name="letter">The to be displayed letter.</param>
        /// <param name="onClickCallback">Callback for when the letter is selected.</param>
        /// <param name="removeOnClick">Whether the letter should remove itself after being pressed.</param>
        public override void Setup(char letter, Action<Vector3> onClickCallback, bool removeOnClick)
        {
            base.Setup(letter, onClickCallback, removeOnClick);

            _text.text = letter.ToString();
            _button.onClick.AddListener(OnClicked);
        }

        /// <summary>
        /// Handles setting the letter inactive when it's removed on click.
        /// </summary>
        protected override void OnSelected()
        {
            if (_removeOnClick)
            {
                gameObject.SetActive(false);
                return;
            }
        }
    }
}