using System;
using UnityEngine;

namespace DTT.MiniGame.Anagram
{
    /// <summary>
    /// Struct holding the basic difficulty settings of the anagram game.
    /// </summary>
    [Serializable]
    public struct AnagramDifficulty
    {
        /// <summary>
        /// Max length the words can be.
        /// </summary>
        [SerializeField]
        [Tooltip("Max length the words can be.")]
        public uint maxWordLength;

        /// <summary>
        /// Minimum length the words must be.
        /// </summary>
        [SerializeField]
        [Tooltip("Minimum length the words must be.")]
        public uint minWordLength;
    }
}