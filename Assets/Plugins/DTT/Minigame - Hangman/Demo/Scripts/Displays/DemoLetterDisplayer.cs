using UnityEngine;
using UnityEngine.UI;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// A behaviour displaying a letter in the demo scene.
    /// </summary>
    public class DemoLetterDisplayer : LetterDisplayer
    {
        /// <summary>
        /// The display image.
        /// </summary>
        [SerializeField]
        private Image _image;

        /// <summary>
        /// The letter text renderer.
        /// </summary>
        [SerializeField]
        private Text _text;

        /// <summary>
        /// Whether the letter text is set.
        /// </summary>
        public override bool IsSet => _text.text == Letter.ToString();

        /// <summary>
        /// Sets up the display with the letter.
        /// </summary>
        /// <param name="letter">The letter to set the display up with.</param>
        public override void Setup(char letter)
        {
            base.Setup(letter);
            
            // Nothing is to be displayed if the character is a white space part of a sentence.
            _image.enabled = !char.IsWhiteSpace(letter);
        }

        /// <summary>
        /// Sets the letter value this display is setup with.
        /// </summary>
        public override void SetLetterValue() => _text.text = Letter.ToString();
    }
}
