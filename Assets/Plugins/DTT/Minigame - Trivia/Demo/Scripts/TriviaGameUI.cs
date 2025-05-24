using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DTT.MinigameBase.UI;

namespace DTT.Trivia.Demo
{
    /// <summary>
    /// UI manager class for the demo scene.
    /// </summary>
    public class TriviaGameUI : MonoBehaviour
    {
        /// <summary>
        /// Reference to the game manager class.
        /// </summary>
        [SerializeField]
        private GameManager _gameManager;

        /// <summary>
        /// UI class for the question.
        /// </summary>
        [SerializeField]
        private QuestionUI _questionPrefab;

        /// <summary>
        /// UI class for the answers.
        /// </summary>
        [SerializeField]
        private AnswerUI _answerPrefab;

        /// <summary>
        /// Layout group for the question.
        /// </summary>
        [SerializeField]
        private HorizontalLayoutGroup _layoutQuestion;

        /// <summary>
        /// Layout group for the answers.
        /// </summary>
        [SerializeField]
        private HorizontalLayoutGroup _layoutAnswers;

        /// <summary>
        /// Reference to the start game button.
        /// </summary>
        [SerializeField]
        private GameObject _startButton;

        /// <summary>
        /// Reference to the results screen.
        /// </summary>
        [SerializeField]
        private ResultsMenu _resultsMenu;

        /// <summary>
        /// Button to submit multiple choice answers.
        /// </summary>
        [SerializeField]
        private Button _guessButton;

        /// <summary>
        /// How long should the answer flash when it's correct.
        /// </summary>
        [SerializeField]
        private float _flashDuration = 1f;

        /// <summary>
        /// Time before the next flash.
        /// </summary>
        [SerializeField]
        private float _timeBetweenFlashes = 0.12f;

        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private AudioClip _audioClipTrueAnswer;

        [SerializeField]
        private AudioClip _audioClipFalseAnswer;

        [SerializeField]
        private GameObject _textScore;

        public int currentQuestion;
        
        private bool IsNotNumbered;

        /// <summary>
        /// The current question object.
        /// </summary>
        private QuestionUI _questionObject;

        /// <summary>
        /// The current answers on screen.
        /// </summary>
        private readonly List<AnswerUI> _answerObjects = new List<AnswerUI>();

        /// <summary>
        /// Collection to store current guesses.
        /// </summary>
        private readonly List<QuestionManager.Answer> _guessList = new List<QuestionManager.Answer>();

        /// <summary>
        /// Handles starting the game.
        /// </summary>
        private void OnStarted()
        {
            currentQuestion = 0;
            GenerateBoard();
        }

        private void OnRestart()
        {
            // if (MiniGameEndController.instance == null)
            // {
            //     _startButton.SetActive(true);
            //     _guessButton.gameObject.SetActive(false);
            //     _resultsMenu.gameObject.SetActive(false);
            //     _textScore.gameObject.SetActive(false);
            //     _gameManager._timer.gameObject.SetActive(false);
            // }
            // else
            // {
                _startButton.SetActive(false);
                _guessButton.gameObject.SetActive(false);
                _resultsMenu.gameObject.SetActive(false);
                _textScore.gameObject.SetActive(true);
                _gameManager._timer.gameObject.SetActive(true);
            //}
            _guessList.Clear();
            ClearBoard();
        }

        /// <summary>
        /// Handles an answer being selected.
        /// If not multiple choice, evaluates the answer directly.
        /// </summary>
        /// <param name="guess">Selected answer.</param>
        public void OnAnswerSelected(AnswerUI guess)
        {
            if (_gameManager.isCorrectAnswer) return;
            if (_guessList.Contains(guess.AnswerData))
            {
                _guessList.Remove(guess.AnswerData);
                return;
            }

            _guessList.Add(guess.AnswerData);

            if (!_gameManager.QuestionManager.CurrentQuestion.MatchCorrectAnswers)
            {
                _gameManager.EvaluateGuess(_guessList);
                _guessList.Clear();
            }
        }

        /// <summary>
        /// Used by the submit button on multiple choice questions to evaluate the answers.
        /// </summary>
        public void MakeGuess()
        {
            if (_gameManager.isCorrectAnswer) return;
            foreach (AnswerUI answer in _answerObjects)
                answer.Deselect();

            _gameManager.EvaluateGuess(_guessList);
        }

        /// <summary>
        /// Generates the question and answers.
        /// </summary>
        private void GenerateBoard()
        {
            _startButton.SetActive(false);
            _resultsMenu.gameObject.SetActive(false);
            _textScore.gameObject.SetActive(true);
            _gameManager._timer.gameObject.SetActive(true);
            ClearBoard();

            //_questionObject = Instantiate(_questionPrefab, _layoutQuestion.transform, false);
            _questionObject = GetQuestionUI();
            _questionObject.index = currentQuestion;
            _questionObject.isNotNumbered = _gameManager.QuestionManager.IsNotNumbered;
            _questionObject.Initialize(_gameManager.QuestionManager.CurrentQuestion);
            currentQuestion++;

            int index = 0;
            foreach (QuestionManager.Answer answerData in _gameManager.QuestionManager.CurrentQuestion.Answers)
            {
                //AnswerUI answerUI = Instantiate(_answerPrefab, _layoutAnswers.transform, false);
                AnswerUI answerUI = GetAnswerUI(index);
                answerUI.index = index;
                answerUI.Deselect();
                answerUI.Initialize(answerData);

                _answerObjects.Add(answerUI);
                index++;
            }

            foreach (AnswerUI answer in _answerObjects) {
                answer.Selected -= OnAnswerSelected;
                answer.Selected += OnAnswerSelected;
            }

            if (_gameManager.QuestionManager.CurrentQuestion.MatchCorrectAnswers)
                _guessButton.gameObject.SetActive(true);

            else
                _guessButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Clears the question and answers.
        /// </summary>
        private void ClearBoard()
        {
            foreach (AnswerUI answerObj in _answerObjects)
                //Destroy(answerObj.gameObject);
                answerObj.gameObject.SetActive(false);

            if (_questionObject != null)
                //Destroy(_questionObject.gameObject);
                _questionObject.gameObject.SetActive(false);

            _answerObjects.Clear();
        }

        /// <summary>
        /// Called when the game is finished.
        /// Shows the results menu.
        /// </summary>
        /// <param name="results">The game results.</param>
        private void OnFinish(GameResults results)
        {
            
            if (MiniGameEndController.instance == null)
            {
                _resultsMenu.ShowText(results.TimeTaken, results.WrongGuesses);
                _resultsMenu.gameObject.SetActive(true);
            } 
            else
            {
                MiniGameEndController.instance.score = (int)Mathf.Ceil(GameManager.instance.totalScore);
                MiniGameEndController.instance.ShowGameEnd();
            }
            Debug.Log("Game Ended.");
            Debug.Log(results.ToString());
        }

        /// <summary>
        /// Handles a correct guess.
        /// </summary>
        /// <param name="correctAnswers">The correct answers.</param>
        private void OnCorrectGuess(List<QuestionManager.Answer> correctAnswers)
        {
            Debug.Log("Correct guess.");
            _gameManager.isCorrectAnswer = true;
            _audioSource.PlayOneShot(_audioClipTrueAnswer);
            StartCoroutine(PlayCorrectAnimations());
        }

        /// <summary>
        /// Blinks the correct answers before going to the next question.
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayCorrectAnimations()
        {
            foreach (QuestionManager.Answer answer in _guessList)
                foreach (AnswerUI answerObject in _answerObjects)
                    if (answer == answerObject.AnswerData)
                        StartCoroutine(answerObject.CorrectGuess(_flashDuration, _timeBetweenFlashes));
            _guessList.Clear();
            yield return new WaitForSeconds(_audioClipTrueAnswer.length);
            _questionObject.PlayQuestionAudioFinish();
            if (_questionObject.audioClipFinish == null) yield return new WaitForSeconds(_flashDuration);
            else yield return new WaitForSeconds(_questionObject.audioClipFinish.length);
            _gameManager.GetNextQuestion();

            if (!_gameManager.IsGameActive)
                yield break;

            GenerateBoard();
        }

        /// <summary>
        /// Handles an incorrect guess.
        /// </summary>
        /// <param name="correctAnswers">The correct answers.</param>
        private void OnIncorrectGuess(List<QuestionManager.Answer> correctAnswers)
        {
            Debug.Log("Incorrect guess.");
            _audioSource.PlayOneShot(_audioClipFalseAnswer);
            StartCoroutine(PlayIncorrectAnimations());
        }

        IEnumerator PlayIncorrectAnimations() {
            AnswerUI answerUI = null;
            foreach (QuestionManager.Answer answer in _guessList)
                foreach (AnswerUI answerObject in _answerObjects)
                    if (answer == answerObject.AnswerData) {
                        answerObject.WrongGuess();
                        answerUI = answerObject;
                    }
            yield return new WaitForSeconds(_audioClipFalseAnswer.length);
            if (answerUI != null) answerUI.PlayAudioWrong();
            _guessList.Clear();
            //yield return new WaitForSeconds(_audioClipFalseAnswer.length);
        }

        /// <summary>
        /// Handles the end of the questions.
        /// Ends the game.
        /// </summary>
        private void OnNoQuestionsRemaining()
        {
            _gameManager.ForceFinish();
            Debug.Log("No questions remaining.");
        }

        /// <summary>
        /// Handles event subscribing.
        /// </summary>
        private void OnEnable()
        {
            _gameManager.OnEnableGameManager();
            OnRestart();
            _gameManager.Started += OnStarted;
            _gameManager.CorrectGuess += OnCorrectGuess;
            _gameManager.IncorrectGuess += OnIncorrectGuess;
            _gameManager.Finish += OnFinish;
            _gameManager.NoQuestionsRemaining += OnNoQuestionsRemaining;
            /*if (MiniGameEndController.instance != null)*/ _gameManager.StartGame();
        }

        /// <summary>
        /// Handles event unsubscribing.
        /// </summary>
        private void OnDisable()
        {
            _gameManager.OnDisableGameManager();
            _gameManager.Started -= OnStarted;
            _gameManager.CorrectGuess -= OnCorrectGuess;
            _gameManager.IncorrectGuess -= OnIncorrectGuess;
            _gameManager.Finish -= OnFinish;
            _gameManager.NoQuestionsRemaining -= OnNoQuestionsRemaining;
        }

        QuestionUI GetQuestionUI() {
            if (_layoutQuestion.transform.childCount > 0) { 
                _layoutQuestion.transform.GetChild(0).gameObject.SetActive(true);
                return _layoutQuestion.transform.GetChild(0).GetComponent<QuestionUI>();
            }
            else return Instantiate(_questionPrefab, _layoutQuestion.transform, false);
        }
        
        AnswerUI GetAnswerUI(int index) {
            if (_layoutAnswers.transform.childCount > index) {
                _layoutAnswers.transform.GetChild(index).gameObject.SetActive(true);
                return _layoutAnswers.transform.GetChild(index).GetComponent<AnswerUI>();
            }
            else return Instantiate(_answerPrefab, _layoutAnswers.transform, false);
        }
    }
}