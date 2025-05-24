using System.Collections.Generic;
using UnityEngine;

namespace DTT.Hangman
{
    /// <summary>
    /// Represents a list of phrases usable for a hangman game.
    /// </summary>
    [CreateAssetMenu(menuName = "DTT/Hangman/Wordlist")]
    public class PhraseList : ScriptableObject
    {
        /// <summary>
        /// The theme of the list.
        /// </summary>
        [SerializeField]
        private string _theme;

        /// <summary>
        /// The phrases in the list.
        /// </summary>
        //[SerializeField]
        public List<Phrase> _phrases;

        /// <summary>
        /// The theme of the list.
        /// </summary>
        public string Theme => _theme;

        /// <summary>
        /// The amount of phrases in the list.
        /// </summary>
        public int PhraseCount => _phrases.Count;

        /// <summary>
        /// Returns a phrase from the list by index.
        /// </summary>
        /// <param name="index">The index of the phrase element.</param>
        public Phrase this[int index] => _phrases[index];

        public void PassPhrase(int index, bool pass)
        {
            if (index >= 0 && index < _phrases.Count)
            {
                Phrase phrase = _phrases[index];
                phrase.pass = pass;
                _phrases[index] = phrase;
                Debug.Log($"PhraseList: Phrase at index {index} pass status set to: {pass}");
            }
            else
            {
                Debug.LogError($"PhraseList: Invalid index {index} in PassPhrase method");
            }
        }
    }
}
