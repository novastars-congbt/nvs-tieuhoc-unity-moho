using UnityEngine;

namespace DTT.Hangman
{
    /// <summary>
    /// An abstract representation of a behaviour that displays a letter.
    /// </summary>
    public abstract class LetterDisplayer : MonoBehaviour
    {
        /// <summary>
        /// The letter to display.
        /// </summary>
        public char Letter { get; private set; }
        
        /// <summary>
        /// Whether the letter has been set for display.
        /// </summary>
        public abstract bool IsSet { get; }

        /// <summary>
        /// Sets up the letter reference.
        /// </summary>
        /// <param name="letter">The letter to display.</param>
        public virtual void Setup(char letter) => Letter = letter;

        /// <summary>
        /// Clears the display.
        /// </summary>
        public virtual void Clear() { }

        /// <summary>
        /// Sets the letter value for display.
        /// </summary>
        public abstract void SetLetterValue();
    }
}