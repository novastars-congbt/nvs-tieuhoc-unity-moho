using System;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DTT.Hangman
{
    /// <summary>
    /// Represents a phrase usable for a hangman game.
    /// </summary>
    [Serializable]
    public struct Phrase
    {
        /// <summary>
        /// The phrase value.
        /// </summary>
        [LabelText("Nội dung")]
        public string value;
        public bool pass;
        [Tooltip("Sử dụng khi trong 1 level chơi có nhiều tiêu đề khác nhau")]
        [LabelText("Tiêu đề")]
        public string theme;

        /// <summary>
        /// The descriptions and its audible of the phrase usable for hints.
        /// </summary>
        [LabelText("Gợi ý")]
        public string[] descriptions;
        [LabelText("Âm thanh kết thúc")]
        public AudioClip[] audible;
      
        /// <summary>
        /// The exposed letters of the phrase to be shown
        /// at the start of the game.
        /// </summary>
        [LabelText("Những kí tự hiển thị sẵn khi bắt đầu")]
        public char[] exposedLetters;

        /// <summary>
        /// Returns a character of the phrase based on the character element index.
        /// </summary>
        /// <param name="index">The index.</param>
        public char this[int index] => value[index];
      
        /// <summary>
        /// The length of the phrase.
        /// </summary>
        public int Length => value.Length;

        /// <summary>
        /// The amount of words in the phrase.
        /// </summary>
        public int WordCount => value.Count(letter => letter == ' ') + 1;

        /// <summary>
        /// Creates a new phrase instance.
        /// </summary>
        /// <param name="value">The phrase value.</param>
        /// <param name="descriptions">The exposed letters of the phrase to be shown at the start of the game.</param>
        /// <param name="exposedLetters">The exposed letters of the phrase to be shown at the start of the game.</param>
        public Phrase(string value, bool pass, string theme, AudioClip[] audible, string[] descriptions = null, char[] exposedLetters = null)
        {
            this.audible = audible;
            this.value = value;
            this.descriptions = descriptions;
            this.exposedLetters = exposedLetters;
            this.pass = pass;
            this.theme = theme;
        }

        /// <summary>
        /// Splits the phrase up in sub phrases. Use this to split up a phrase that is a sentence into words.
        /// </summary>
        /// <returns>The sub phrases.</returns>
        public Phrase[] Split()
        {
            Phrase[] phrases = value.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(phrase => new Phrase(phrase, false, "", null))
                                    .ToArray();
         
            for (int i = 0; i < phrases.Length; i++)
            {
                phrases[i].descriptions = descriptions;
                phrases[i].exposedLetters = exposedLetters;
            }

            return phrases;
        }

        /// <summary>
        /// Returns a new phrase that exposes the given letters.
        /// </summary>
        /// <param name="exposedLetters">The letters to expose.</param>
        /// <returns>The new phrase exposing the letters.</returns>
        public Phrase Expose(char[] exposedLetters) => new Phrase(value, pass, theme, audible, descriptions, exposedLetters);

        /// <summary>
        /// Returns whether this phrase contains a given letter.
        /// </summary>
        /// <param name="letter">The letter to check for.</param>
        /// <returns>Whether this phrase contains a given letter.</returns>
        public bool Contains(char letter)
        {
            char letterToUpper = char.ToUpperInvariant(letter);
            for(int i = 0; i < value.Length; i++)
                if (char.ToUpperInvariant(value[i]) == letterToUpper)
                    return true;
                return false;
        }

        /// <summary>
        /// Returns whether a given letter is exposed by this phrase.
        /// </summary>
        /// <param name="letter">The letter to check for.</param>
        /// <returns>Whether the given letter is exposed by this phrase.</returns>
        public bool Exposes(char letter)
        {
            char letterToUpper = char.ToUpperInvariant(letter);
            for(int i = 0; i < exposedLetters.Length; i++)
                if (char.ToUpperInvariant(exposedLetters[i]) == letterToUpper)
                    return true;

            return false;
        }
        /// <summary>
        /// Returns the string value of this phrase.
        /// </summary>
        /// <returns>The string value.</returns>
        public override string ToString() => value;
    }
}
