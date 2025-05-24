
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.Trivia
{
    /// <summary>
    /// Base class for the question scriptable objects.
    /// </summary>
    [CreateAssetMenu(menuName = "DTT/MiniGame/Trivia/Question", fileName = "New Question")]
    public class Question : ScriptableObject
    {
        /// <summary>
        /// Text content of the question.
        /// </summary>
        [SerializeField]
        private string _title;

        [SerializeField]
        private AudioClip _audioClipFinish;

        /// <summary>
        /// Whether the question has more than one correct answer.
        /// </summary>
        [SerializeField]
        private bool _matchCorrectAnswers;

        /// <summary>
        /// Collection for all answers.
        /// </summary>
        [SerializeReference]
        public List<Answer> _answers;

        /// <summary>
        /// Text content of the question.
        /// </summary>
        public string Title => _title;

        public AudioClip AudioClipFinish => _audioClipFinish;

        /// <summary>
        /// Marks the question as multiple choice.
        /// </summary>
        public bool MatchCorrectAnswers => _matchCorrectAnswers;

        /// <summary>
        /// Collection for all answers.
        /// </summary>
        public List<Answer> Answers => _answers;
    }
}