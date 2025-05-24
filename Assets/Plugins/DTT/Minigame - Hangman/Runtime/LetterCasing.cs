using UnityEngine;

namespace DTT.Hangman
{
    /// <summary>
    /// Represents the casing to use for letters in the game.
    /// </summary>
    public enum LetterCasing
    {
        /// <summary>
        /// Only use upper casing.
        /// </summary>
        [InspectorName("Upper")]
        UPPER,

        /// <summary>
        /// Only use lower casing.
        /// </summary>
        [InspectorName("Lower")]
        LOWER,
    }
}
