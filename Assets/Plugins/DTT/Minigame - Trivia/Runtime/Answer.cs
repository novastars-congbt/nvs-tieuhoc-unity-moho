using System;
using UnityEngine;

namespace DTT.Trivia
{
    /// <summary>
    /// Base data class for answers.
    /// </summary>
    [Serializable]
    public class Answer
    {
        /// <summary>
        /// Marks the answer as correct.
        /// </summary>
        [SerializeField]
        private bool _isCorrect;

        /// <summary>
        /// The text content of the answer.
        /// </summary>
        [SerializeField]
        private string _body;

        /// <summary>
        /// Marks the answer as correct.
        /// </summary>
        public bool IsCorrect => _isCorrect;

        /// <summary>
        /// The text content of the answer.
        /// </summary>
        public string Body => _body;
    }
}