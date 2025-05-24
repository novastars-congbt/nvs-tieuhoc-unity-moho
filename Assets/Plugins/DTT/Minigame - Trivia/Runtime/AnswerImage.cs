using System;
using UnityEngine;

namespace DTT.Trivia
{
    /// <summary>
    /// Data class for answers with an image.
    /// </summary>
    [Serializable]
    public class AnswerImage : Answer
    {
        /// <summary>
        /// The image of the answer.
        /// </summary>
        [SerializeField]
        private Sprite _spr;

        /// <summary>
        /// The image of the answer.
        /// </summary>
        [SerializeField]
        public Sprite Spr => _spr;
    }
}