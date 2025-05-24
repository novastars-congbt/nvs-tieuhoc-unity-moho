using System;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.Trivia.Demo
{
    /// <summary>
    /// The results screen.
    /// </summary>
    public class ResultsMenu : MonoBehaviour
    {
        /// <summary>
        /// Text component of timer.
        /// </summary>
        [SerializeField]
        private Text _timerText;

        /// <summary>
        /// Text component for the wrong guesses.
        /// </summary>
        [SerializeField]
        private Text _wrongGuessesText;

        /// <summary>
        /// Sets the text to the passed string.
        /// </summary>
        /// <param name="result">Result string.</param>
        public void ShowText(float timeTaken, float wrongGuesses)
        {
            Debug.Log(TimeSpan.FromSeconds(timeTaken).ToString(@"mm\:ss"));
            _timerText.text = TimeSpan.FromSeconds(timeTaken).ToString(@"mm\:ss");
            _wrongGuessesText.text = (int)GameManager.instance.totalScore + "" /*wrongGuesses.ToString()*/;
        }
    }
}