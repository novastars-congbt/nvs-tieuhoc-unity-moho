using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace DTT.Trivia
{
    /// <summary>
    /// Scriptable object for the question manager.
    /// </summary>
    [CreateAssetMenu(menuName = "DTT/MiniGame/Trivia/QuestionManager", fileName = "New Question Manager")]
    public class QuestionManager : ScriptableObject
    {
        /// <summary>
        /// Whether to randomize the questions.
        /// </summary>
        [LabelText("Ngẫu nhiên câu hỏi")]
        [SerializeField]
        [Tooltip("Whether to randomize the questions.")]
        private bool _randomizeQuestions;

        [LabelText("Không đánh số thứ tự câu hỏi")]
        [SerializeField]
        private bool _isNotNumbered;

        /// <summary>
        /// The amount of available time per question.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        [Tooltip("The amount of available time per question.")]
        private float _timePerQuestion;

        /// <summary>
        /// The time limit for each question.
        /// </summary>
        public float TimePerQuestion => _timePerQuestion;

        /// <summary>
        /// Whether to randomize the question order.
        /// </summary>
        public bool RandomizeQuestions => _randomizeQuestions;

        /// <summary>
        /// Collection with all questions that are part of this quiz.
        /// </summary>
        public List<Question> AllQuestions => _allQuestions;

        /// <summary>
        /// Tracks all questions that have not been answered yet.
        /// </summary>
        public List<Question> AvailableQuestions => _availableQuestions;

        /// <summary>
        /// The current round's question.
        /// </summary>
        public Question CurrentQuestion { get; private set; }

        public bool IsNotNumbered => _isNotNumbered;

        /// <summary>
        /// Collection with all questions.
        /// </summary>
        //[SerializeField]
        [LabelText("Danh sách câu hỏi")]
        public List<Question> _allQuestions = new List<Question>();

        /// <summary>
        /// Tracks all questions that have not been answered.
        /// </summary>
        private readonly List<Question> _availableQuestions = new List<Question>();

        [Serializable]
        public class Question
        {
            [LabelText("Nội dung câu hỏi")]
            [SerializeField]
            private string _title;

            [LabelText("Hình ảnh câu hỏi")]
            [SerializeField]
            private Sprite _questionSpr;

            [LabelText("Âm thanh câu hỏi")]
            [SerializeField]
            private AudioClip _questionAudio;

            [LabelText("Âm thanh trả lời đúng")]
            [SerializeField]
            private AudioClip _audioClipFinish;

            [LabelText("Nhiều đáp án đúng")]
            [SerializeField]
            private bool _matchCorrectAnswers;

            [LabelText("Hiển thị nội dung câu hỏi")]
            [SerializeField]
            private bool _useTitle = true;

            [LabelText("Danh sách câu trả lời")]
            public List<Answer> _answers = new List<Answer>();

            public string Title => _title;

            public AudioClip AudioClipFinish => _audioClipFinish;

            public Sprite Spr => _questionSpr;

            public AudioClip Audio => _questionAudio;

            public bool MatchCorrectAnswers => _matchCorrectAnswers;

            public bool UseTitle => _useTitle;

            public List<Answer> Answers => _answers;
        }

        [Serializable]
        public class Answer
        {
            [LabelText("Nội dung câu trả lời")]
            [SerializeField]
            private string _body;

            [LabelText("Đúng")]
            [SerializeField]
            private bool _isCorrect;

            [LabelText("Ảnh câu trả lời")]
            [SerializeField]
            private Sprite _spr;

            [LabelText("Âm thanh câu trả lời")]
            [SerializeField]
            private AudioClip _audio;
            
            [LabelText("Âm thanh trả lời sai")]
            [SerializeField]
            private AudioClip _audioWrong;

            public bool IsCorrect => _isCorrect;

            public string Body => _body;

            [SerializeField]
            public Sprite Spr => _spr;

            [SerializeField]
            public AudioClip Audio => _audio;
            [SerializeField]
            public AudioClip AudioWrong => _audioWrong;
        }

        /// <summary>
        /// Refreshes the available questions.
        /// Initializes the first question.
        /// </summary>
        internal void Init()
        {
            CurrentQuestion = null;
            _availableQuestions.Clear();
            if (_allQuestions.Count <= 0)
            {
                Debug.LogError("No questions selected.");
                return;
            }


            _availableQuestions.AddRange(_allQuestions);

            if (_randomizeQuestions)
                ChooseRandomQuestion();
            else
                GetNextQuestion();
        }

        /// <summary>
        /// Sets the current question to the one after it if there is one.
        /// </summary>
        internal void GetNextQuestion()
        {
            if (AvailableQuestions.Count <= 0)
                return;

            GameManager.instance.wrongPerAnswer = 0;
            CurrentQuestion = _availableQuestions[0];
            _availableQuestions.Remove(CurrentQuestion);
        }

        /// <summary>
        /// Chooses a random question that hasn't been answered.
        /// </summary>
        internal void ChooseRandomQuestion()
        {
            if (AvailableQuestions.Count <= 0)
                return;

            GameManager.instance.wrongPerAnswer = 0;
            Random random = new Random();

            CurrentQuestion = _availableQuestions[random.Next(_availableQuestions.Count)];
            _availableQuestions.Remove(CurrentQuestion);
        }
    }
}