using System;

namespace DTT.Hangman
{
    /// <summary>
    /// Provides extension methods for the letter casing enumeration.
    /// </summary>
    public static class CasingExtensions
    {
        /// <summary>
        /// Applies the letter casing to a given word.
        /// </summary>
        /// <param name="casing">The casing to apply.</param>
        /// <param name="word">The word to update.</param>
        /// <returns>The updated word.</returns>
        public static string ApplyTo(this LetterCasing casing, string word)
        {
            switch (casing)
            {
                case LetterCasing.UPPER:
                    return word.ToUpper();
                
                case LetterCasing.LOWER:
                    return word.ToLower();

                default:
                    throw new ArgumentOutOfRangeException(nameof(casing), casing, null);
            }
        }
        
        /// <summary>
        /// Applies the letter casing to a given letter.
        /// </summary>
        /// <param name="casing">The casing to apply.</param>
        /// <param name="letter">The letter to update.</param>
        /// <returns>The updated letter.</returns>
        public static char ApplyTo(this LetterCasing casing, char letter)
        {
            switch (casing)
            {
                case LetterCasing.UPPER:
                    return char.ToUpper(letter);
                
                case LetterCasing.LOWER:
                    return char.ToLower(letter);

                default:
                    throw new ArgumentOutOfRangeException(nameof(casing), casing, null);
            }
        }
    }
}
