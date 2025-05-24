using System;
using System.Linq;

namespace DTT.MiniGame.Anagram.Extensions
{
    /// <summary>
    /// Contains extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Randomizer for the string shuffle.
        /// </summary>
        private static Random _random = new Random();

        /// <summary>
        /// Randomizes the order of characters in the string.
        /// </summary>
        /// <param name="str">Pre-shuffled string.</param>
        /// <returns>Shuffled string.</returns>
        public static string Shuffle(this string str) =>
            new string(str.ToCharArray().OrderBy(s => (_random.Next(2) % 2) == 0).ToArray());
    }
}