using UnityEngine;
using System;

namespace DTT.Trivia
{
    /// <summary>
    /// Data class for answers with audio.
    /// </summary>
    [Serializable]
    public class AnswerAudio : Answer
    {
        /// <summary>
        /// The audio of the answer.
        /// </summary>
        [SerializeField]
        private AudioClip _audio;

        /// <summary>
        /// The audio of the answer.
        /// </summary>
        [SerializeField]
        public AudioClip Audio => _audio;
    }
}