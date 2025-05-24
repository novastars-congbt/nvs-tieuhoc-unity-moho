using UnityEngine;

namespace DTT.Trivia
{
    /// <summary>
    /// Scriptable object for questions with an image.
    /// </summary>
    [CreateAssetMenu(menuName = "DTT/MiniGame/Trivia/FullQuestion", fileName = "New Full Question")]
    public class QuestionFull : Question
    {
        [SerializeField]
        private Sprite _questionSpr;

        public Sprite Spr => _questionSpr;

        [SerializeField]
        private AudioClip _questionAudio;

        [SerializeField]
        private bool _useTitle;

        public AudioClip Audio => _questionAudio;

        public bool UseTitle => _useTitle;
    }
}