using UnityEngine;
using System;
using DTT.Utils.Extensions;

namespace DTT.MiniGame.Anagram.UI
{
    /// <summary>
    /// Handles snapping an <see cref="AnagramLetter"/> to a snapping point.
    /// </summary>
    public class AnagramSnapPoint : MonoBehaviour
    {
        /// <summary>
        /// Invoked when a new letter is dropped on this snap point.
        /// </summary>
        public event Action LetterSnapped;

        /// <summary>
        /// Invoked when another letter was replaced with a new letter.
        /// </summary>
        public event Action<AnagramLetter> RemovedLetter;

        /// <summary>
        /// Current letter this snap point is holding.
        /// </summary>
        public AnagramLetter CurrentLetter { get; private set; }

        /// <summary>
        /// RectTransform of this object.
        /// </summary>
        public RectTransform RectTransform => (RectTransform)transform;

        /// <summary>
        /// Sets a letter to this snap point.
        /// Replaces it with another if it is already occupied.
        /// </summary>
        /// <param name="letter">Letter to be snapped.</param>
        internal void SetLetter(AnagramLetter letter)
        {
            AnagramLetter oldLetter = CurrentLetter;
            CurrentLetter = letter;

            // Removes old letter.
            if (oldLetter != null)
            {
                oldLetter.PickUp -= UnsnapLetter;
                RemovedLetter?.Invoke(oldLetter);
            }

            // Snaps the new letter.
            CurrentLetter.SetCurrentSnapPoint(this);
            CurrentLetter.SnapToPosition(RectTransform.GetWorldRect().center);

            CurrentLetter.PickUp += UnsnapLetter;
        }

        /// <summary>
        /// Snaps a letter when it has been dragged into this field.
        /// </summary>
        /// <param name="letter">Letter to be snapped.</param>
        public void SnapLetter(AnagramLetter letter)
        {
            SetLetter(letter);

            LetterSnapped?.Invoke();
        }

        /// <summary>
        /// Removes the listeners from a letter.
        /// </summary>
        /// <param name="letter">Letter to be removed.</param>
        private void UnsnapLetter(AnagramLetter letter)
        {
            letter.PickUp -= UnsnapLetter;
            CurrentLetter = null;
        }
    }
}