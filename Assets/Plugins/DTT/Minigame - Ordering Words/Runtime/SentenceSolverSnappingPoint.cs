using UnityEngine;
using UnityEngine.UI;

namespace DTT.OrderingWords
{
	/// <summary>
	/// Blank space in the sentence solver game.
	/// </summary>
	public class SentenceSolverSnappingPoint : SnappingPoint
    {
        /// <summary>
        /// The text component used for showing hints.
        /// </summary>
        [SerializeField]
        [Tooltip("The text component used for showing hints.")]
        private Text _textComponent;
        [SerializeField]
        LayoutElement _layoutElement;

        /// <summary>
        /// Delegate for passing a word for when the word is removed.
        /// </summary>
        /// <param name="wordToRemove">The item to remove from this snapping point</param>
        public delegate void RemoveWord(Word wordToRemove);

        /// <summary>
        /// Event fired when the occupant should be removed for a new occupant.
        /// </summary>
        public event RemoveWord OnRemoveWord;

        /// <summary>
        /// The correct value for the blank space.
        /// </summary>
        public string CorrectValue => _textComponent.text;

        /// <summary>
        /// The <see cref="Word"/> currently occupying the snapping point.
        /// </summary>
        public Word SnappedWord => _snappedWord;

        /// <summary>
        /// Whether the current occupant is correctly placed according to the exercise.
        /// </summary>
        public bool IsCorrect => SnappedWord != null && SnappedWord.TextComponent == CorrectValue;

        /// <summary>
        /// The <see cref="Word"/> currently occupying the snapping point.
        /// </summary>
        private Word _snappedWord;

        /// <summary>
        /// Set the snap area.
        /// </summary>
        protected override void Awake() => SnapArea = _snapArea;

        /// <summary>
        /// Initialize the blank space.
        /// </summary>
        /// <param name="value">The correct value to be filled in the blank space</param>
        /// <param name="onRemoveWord">The action to invoke when a snapped word should be removed</param>
        public void Init(string value, RemoveWord onRemoveWord = null)
        {
            SetTextTransparency(0);
            _textComponent.text = value;
            OnRemoveWord += onRemoveWord;
        }

        public void SetText(int fontSize)
        {
            _textComponent.fontSize = fontSize;
            _layoutElement.preferredWidth = _textComponent.preferredWidth;
        }

        /// <summary>
        /// Set a new occupant to the snapping point.
        /// </summary>
        /// <param name="newWord">The new occupant</param>
        public void SetSnappedWord(Word newWord)
        {
            Word previousOccupant = _snappedWord;
            _snappedWord = newWord;
            if (newWord != null && previousOccupant != null)
                OnRemoveWord?.Invoke(previousOccupant);
        }

        /// <summary>
        /// Set the transparency of the text component.
        /// </summary>
        /// <param name="alpha">The alpha value for the transparency</param>
        public void SetTextTransparency(float alpha)
        {
            Color textColor = _textComponent.color;
            textColor.a = alpha;
            _textComponent.color = textColor;
        }
    }
}
