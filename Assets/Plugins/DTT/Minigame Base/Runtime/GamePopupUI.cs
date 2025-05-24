﻿using System;
using DTT.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.MinigameBase.UI
{
    /// <summary>
    /// Handles the standardized popup in the game.
    /// </summary>
    public class GamePopupUI : MonoBehaviour
    {
        /// <summary>
        /// Called when the resume button is pressed.
        /// </summary>
        public event Action ResumeButtonPressed;
        
        /// <summary>
        /// Called when the restart button is pressed.
        /// </summary>
        public event Action RestartButtonPressed;
        
        /// <summary>
        /// Called when the next button is pressed.
        /// </summary>
        public event Action NextButtonPressed;

        /// <summary>
        /// Called when the home button is pressed.
        /// </summary>
        public event Action HomeButtonPressed;
        
        /// <summary>
        /// The text object for the title.
        /// </summary>
        [SerializeField]
        private Text _titleText;
        
        /// <summary>
        /// The text object for the backdrop of the title.
        /// </summary>
        [SerializeField]
        private Text _titleBackdropText;

        public Text _answerText;

        /// <summary>
        /// The button for resuming.
        /// </summary>
        [SerializeField]
        private Button _resumeButton;
        
        /// <summary>
        /// The button for restarting.
        /// </summary>
        [SerializeField]
        private Button _restartButton;

        /// <summary>
        /// The button for restarting.
        /// </summary>
        [SerializeField]
        private Button _nextButton;
        
        /// <summary>
        /// The button for returning to home.
        /// </summary>
        [SerializeField]
        private Button _homeButton;

        /// <summary>
        /// Canvas group of the entire popup.
        /// </summary>
        [SerializeField]
        private CanvasGroup _canvasGroup;

        /// <summary>
        /// The animation of showing the popup.
        /// </summary>
        private Coroutine _showAnimation;

        /// <summary>
        /// Adds listeners.
        /// </summary>
        private void OnEnable()
        {
            _resumeButton.onClick.AddListener(OnResumeButtonClicked);
            _restartButton.onClick.AddListener(OnRestartButtonClicked);
            _nextButton.onClick.AddListener(OnNextButtonClicked);
            _homeButton.onClick.AddListener(OnHomeButtonClicked);
        }

        /// <summary>
        /// Removes listeners.
        /// </summary>
        private void OnDisable()
        {
            _resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            _nextButton.onClick.RemoveListener(OnNextButtonClicked);
            _homeButton.onClick.RemoveListener(OnHomeButtonClicked);
        }

        /// <summary>
        /// Called when the resume button is pressed.
        /// </summary>
        private void OnResumeButtonClicked() => ResumeButtonPressed?.Invoke();

        /// <summary>
        /// Called when the restart button is pressed.
        /// </summary>
        private void OnRestartButtonClicked() => RestartButtonPressed?.Invoke();

        /// <summary>
        /// Called when the restart button is pressed.
        /// </summary>
        private void OnNextButtonClicked() => NextButtonPressed?.Invoke();

        /// <summary>
        /// Called when the home button is pressed.
        /// </summary>
        private void OnHomeButtonClicked() => HomeButtonPressed?.Invoke();

        /// <summary>
        /// Sets the title for the paused state.
        /// </summary>
        public void SetTitleToPaused()
        {
            _titleText.text = "TẠM DỪNG";
            _titleBackdropText.text = "TẠM DỪNG";
        }

        /// <summary>
        /// Sets the title for the finished state.
        /// </summary>
        public void SetTitleToFinished()
        {
            _titleText.text = "HOÀN THÀNH MÀN CHƠI";
            _titleBackdropText.text = "HOÀN THÀNH MÀN CHƠI";
        }

        public void SetTitleToLose()
        {
            _titleText.text = "CHÚC BẠN MAY MẮN LẦN SAU";
            _titleBackdropText.text = "CHÚC BẠN MAY MẮN LẦN SAU";
        }

        /// <summary>
        /// Enables the resume button, so it's shown to the user.
        /// </summary>
        /// <param name="isEnabled">Whether to enable or disable</param>
        public void EnableResumeButton(bool isEnabled) => _resumeButton.gameObject.SetActive(isEnabled);
        
        /// <summary>
        /// Enables the restart button, so it's shown to the user.
        /// </summary>
        /// <param name="isEnabled">Whether to enable or disable</param>
        public void EnableRestartButton(bool isEnabled) => _restartButton.gameObject.SetActive(isEnabled);

        /// <summary>
        /// Enables the next button, so it's shown to the user.
        /// </summary>
        /// <param name="isEnabled">Whether to enable or disable</param>
        public void EnableNextButton(bool isEnabled) => _nextButton.gameObject.SetActive(isEnabled);
        
        /// <summary>
        /// Enables the home button, so it's shown to the user.
        /// </summary>
        /// <param name="isEnabled">Whether to enable or disable</param>
        public void EnableHomeButton(bool isEnabled) => _homeButton.gameObject.SetActive(isEnabled);

        /// <summary>
        /// Shows the popup based on the state.
        /// </summary>
        /// <param name="state">Whether to show the popup.</param>
        public void Show(bool state)
        {
            if(_showAnimation != null)
                StopCoroutine(_showAnimation);

            _canvasGroup.interactable = state;
            _canvasGroup.blocksRaycasts = state;
            if (_answerText != null) _answerText.gameObject.SetActive(state);
            DTTween.Value(_canvasGroup.alpha, state ? 1f : 0f, 0.6f, Easing.EASE_IN_OUT_SINE,
                alpha => _canvasGroup.alpha = alpha);
        }
    }
}