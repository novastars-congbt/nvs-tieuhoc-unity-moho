using DTT.MinigameBase;
using DTT.MinigameBase.Timer;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.Trivia
{
    /// <summary>
    /// Controls the game flow.
    /// </summary>
    public class GameManager : MonoBehaviour, IMinigame<QuestionManager, GameResults>
    {
        public static GameManager instance;

        /// <summary>
        /// The Question settings.
        /// </summary>
        [SerializeField]
        [LabelText("Data")]
        private QuestionManager _questionManager;

        /// <summary>
        /// Timer for the game.
        /// </summary>
        //[SerializeField]
        public Timer _timer;

        /// <summary>
        /// The results controller.
        /// </summary>
        public GameResults GameResults { get; private set; }

        /// <summary>
        /// Reference to the question manager.
        /// </summary>
        public QuestionManager QuestionManager => _questionManager;

        /// <summary>
        /// Is true when the game is paused.
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Is true when the game has started and hasn't finished.
        /// </summary>
        public bool IsGameActive { get; private set; }

        public bool isCorrectAnswer = false;

        [SerializeField]
        float maxScore = 100;

        [SerializeField]
        int maxWrongPerAnswer;

        public Text _textScore;

        public float totalScore;

        public float scorePerAnswer;

        float maxScorePerAnswer;

        public int wrongPerAnswer;

        /// <summary>
        /// Event invoked when a correct guess has been made.
        /// </summary>
        public event Action<List<QuestionManager.Answer>> CorrectGuess;

        /// <summary>
        /// Event invoked when an incorrect guess has been made.
        /// </summary>
        public event Action<List<QuestionManager.Answer>> IncorrectGuess;

        /// <summary>
        /// Event invoked when there are no questions remaining to be answered.
        /// </summary>
        public event Action NoQuestionsRemaining;

        /// <summary>
        /// Event invoked the game goes to the next question;
        /// </summary>
        public event Action NextQuestion;

        /// <summary>
        /// Event invoked when the timer reaches the set limit.
        /// </summary>
        public event Action TimeLimitReached;

        /// <summary>
        /// Event invoked when the game has finished.
        /// </summary>
        public event Action<GameResults> Finish;

        /// <summary>
        /// Event invoked when the game has started.
        /// </summary>
        public event Action Started;

        /// <summary>
        /// The timers' total time displayed at the end.
        /// </summary>
        private float _totalTime;

        /// <summary>
        /// Is true when the game has started and isn't finished.
        /// </summary>
        private bool _timeLimitReached;

        /// <summary>
        /// Starts the game using the default settings.
        /// </summary>
        public void StartGame() => StartGame(_questionManager);

        /// <summary>
        /// Is called when the user wants to start the quiz with custom settings.
        /// </summary>
        public void StartGame(QuestionManager questionManager)
        {
            Debug.LogError("==================== start game");
            if (_timer != null)
                _timer.Begin();

            _questionManager = questionManager;
            IsPaused = false;
            IsGameActive = true;

            InitializeGame();
            Started?.Invoke();
        }

        public void OnEnableGameManager()
        {
            if (instance == null) instance = this;
            //if (MiniGameEndController.instance != null) MiniGameEndController.instance.actionRestartGame += StartGame;
        }

        public void OnDisableGameManager()
        {
            instance = null;
            //if (MiniGameEndController.instance != null) MiniGameEndController.instance.actionRestartGame -= StartGame;
        }

        /// <summary>
        /// Checks if the time limit has been reached and invokes the event.
        /// </summary>
        private void Update()
        {
            if (!_timeLimitReached && _timer != null && _timer.TimePassed.TotalSeconds >= _questionManager.TimePerQuestion)
            {
                TimeLimitReached?.Invoke();
                _timeLimitReached = true;
            }
        }

        /// <summary>
        /// Used to pause the game.
        /// </summary>
        public void Pause()
        {
            IsPaused = true;

            if (_timer != null)
                _timer.Pause();
        }

        /// <summary>
        /// Continues the game.
        /// </summary>
        public void Continue()
        {
            IsPaused = false;

            if (_timer != null)
                _timer.Resume();
        }

        /// <summary>
        /// Is called when the user wants to force finish the quiz.
        /// </summary>
        public void ForceFinish()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _totalTime = (float)_timer.TimePassed.TotalSeconds;
                GameResults.SetTimeTaken(_totalTime);
            }

            IsGameActive = false;
            Finish?.Invoke(GameResults);
        }

        /// <summary>
        /// Used to invoke the evaluation events based on the guess.
        /// </summary>
        /// <param name="guesses">List with guesses.</param>
        public void EvaluateGuess(List<QuestionManager.Answer> guesses)
        {
            List<QuestionManager.Answer> correctAnswers = _questionManager.CurrentQuestion.Answers.Where(x => x.IsCorrect == true).ToList();
            guesses = guesses.Distinct().ToList();

            // Checks for a wrong guess in the list.
            foreach (QuestionManager.Answer guess in guesses)
            {
                if (!guess.IsCorrect)
                {
                    SetScoreIncorrect();
                    IncorrectGuess?.Invoke(correctAnswers);
                    GameResults.IncrementWrongGuess();
                    return;
                }
            }

            if (_timer != null)
            {
                _timer.Stop();
                //_totalTime += (float)_timer.TimePassed.TotalSeconds;
            }


            if (!_questionManager.CurrentQuestion.MatchCorrectAnswers)
            {
                SetScoreCorrect();
                CorrectGuess?.Invoke(correctAnswers);             
            }
            else if (guesses.Count == correctAnswers.Count)
            {
                SetScoreCorrect();
                CorrectGuess?.Invoke(correctAnswers);
            }
            else
            {
                SetScoreIncorrect();
                IncorrectGuess?.Invoke(correctAnswers);
                GameResults.IncrementWrongGuess();
            }
        }

        /// <summary>
        /// Goes to the next question.
        /// </summary>
        public void GetNextQuestion()
        {
            isCorrectAnswer = false;
            if (_questionManager.AvailableQuestions.Count <= 0)
            {
                NoQuestionsRemaining?.Invoke();
                _totalTime = 0;
                return;
            }

            if (_timer != null)
                //_timer.Begin();
                _timer.Resume();

            NextQuestion?.Invoke();
            _timeLimitReached = false;
            if (_questionManager.RandomizeQuestions) _questionManager.ChooseRandomQuestion();
            else _questionManager.GetNextQuestion();
        }

        /// <summary>
        /// Initializes the results and the question manager.
        /// </summary>
        private void InitializeGame()
        {
            isCorrectAnswer = false;
            totalScore = 0;
            _textScore.text = "Điểm: " + (int)Mathf.Ceil(totalScore);
            scorePerAnswer = maxScore / _questionManager.AllQuestions.Count;
            GameResults = new GameResults();
            _questionManager.Init();
            
        }

        void SetScoreCorrect()
        {
            totalScore += scorePerAnswer * (maxWrongPerAnswer + 1 - wrongPerAnswer) / (maxWrongPerAnswer + 1);
            _textScore.text = "Điểm: " + (int)Mathf.Ceil(totalScore);
        }

        void SetScoreIncorrect()
        {
            wrongPerAnswer++;
            if (wrongPerAnswer >= maxWrongPerAnswer) wrongPerAnswer = maxWrongPerAnswer;
        }

    }
}