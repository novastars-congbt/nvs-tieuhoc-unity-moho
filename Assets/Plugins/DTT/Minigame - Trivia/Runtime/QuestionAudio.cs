using UnityEngine;

namespace DTT.Trivia
{
    /// <summary>
    /// Scriptable object for questions with an image.
    /// </summary>
    [CreateAssetMenu(menuName = "DTT/MiniGame/Trivia/AudioQuestion", fileName = "New Audio Question")]
    public class QuestionAudio : Question
    {
        /// <summary>
        /// The audio of the question.
        /// </summary>
        [SerializeField]
        private AudioClip _questionAudio;

        /// <summary>
        /// Will display the title when true.
        /// </summary>
        [SerializeField]
        private bool _useTitle;

        /// <summary>
        /// The audio of the question.
        /// </summary>
        public AudioClip Audio => _questionAudio;

        /// <summary>
        /// Will display the title when true.
        /// </summary>
        public bool UseTitle => _useTitle;
    }
}