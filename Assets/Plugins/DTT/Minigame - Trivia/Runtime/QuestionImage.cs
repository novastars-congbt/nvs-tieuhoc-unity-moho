using UnityEngine;

namespace DTT.Trivia
{
    /// <summary>
    /// Scriptable object for questions with an image.
    /// </summary>
    [CreateAssetMenu(menuName = "DTT/MiniGame/Trivia/ImageQuestion", fileName = "New Image Question")]
    public class QuestionImage : Question
    {
        /// <summary>
        /// Image of the question.
        /// </summary>
        [SerializeField]
        private Sprite _questionSpr;

        [SerializeField]
        private bool _useTitle;

        /// <summary>
        /// Image of the question.
        /// </summary>
        public Sprite Spr => _questionSpr;

        public bool UseTitle => _useTitle;
    }
}