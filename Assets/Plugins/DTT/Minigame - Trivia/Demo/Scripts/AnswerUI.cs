using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DTT.MinigameBase.Handles;
using System.Collections;
//using Codice.CM.Common;

namespace DTT.Trivia.Demo
{
    /// <summary>
    /// UI class to display an answer.
    /// </summary>
    [RequireComponent(typeof(Handle))]
    public class AnswerUI : MonoBehaviour
    {
        /// <summary>
        /// Background component;
        /// </summary>
        [SerializeField]
        private Image _background;

        /// <summary>
        /// Background shadow component;
        /// </summary>
        [SerializeField]
        private Image _backgroundShadow;

        /// <summary>
        /// Component to display an image.
        /// </summary>
        [SerializeField]
        private Image _image;

        /// <summary>
        /// Component to display text.
        /// </summary>
        [SerializeField]
        private Text _text;

        /// <summary>
        /// Component to play audio.
        /// </summary>
        private AudioClip _audioclip;

        /// <summary>
        /// sound button;
        /// </summary>
        [SerializeField]
        private GameObject _soundButtonParent;

        /// <summary>
        /// Reference to the audiosource component.
        /// </summary>
        [SerializeField]
        private AudioSource _audioSource;

        /// <summary>
        /// The data class for this answer.
        /// </summary>
        public QuestionManager.Answer AnswerData { get; private set; }

        /// <summary>
        /// Invoked when selected by the user.
        /// </summary>
        public event Action<AnswerUI> Selected;

        /// <summary>
        /// Enables clicking on the images.
        /// </summary>
        public Handle Handle { get; private set; }

        /// <summary>
        /// Default color of the background.
        /// </summary>
        private Color _defaultBackgroundColor;

        /// <summary>
        /// Default color of the background shadow.
        /// </summary>
        private Color _defaultBackgroundShadowColor;

        /// <summary>
        /// True when the button is selected.
        /// </summary>
        private bool _isSelected;

        public int index = -1;

        private AudioClip _audioClipWrong;

        /// <summary>
        /// Gets necessary components.
        /// </summary>
        /// 
        protected virtual void Awake()
        {
            Handle = GetComponent<Handle>();
            _defaultBackgroundColor = _background.color;
            _defaultBackgroundShadowColor = _backgroundShadow.color;
        }

        /// <summary>
        /// Sets the necessary component data based on the answer type.
        /// </summary>
        /// <param name="answer">The given answer type.</param>
        public void Initialize(QuestionManager.Answer answer)
        {
            AnswerData = answer;

            if (answer.Body == "")
                _text.gameObject.SetActive(false);
            else
                _text.text = SetOrder(index) + answer.Body;

            _image.sprite = answer.Spr;
            _image.gameObject.SetActive(answer.Spr);
            _image.preserveAspect = true;
            _audioclip = answer.Audio;
            _soundButtonParent.SetActive(answer.Audio);
            _audioClipWrong = answer.AudioWrong;

            //_image.gameObject.SetActive(false);
            //_soundButtonParent.SetActive(false);
            //switch (answer)
            //{
            //    case AnswerAudio answerAudio:
            //        _audioclip = answerAudio.Audio;
            //        _soundButtonParent.SetActive(true);
            //        break;
            //    case AnswerImage answerImage:
            //        _image.gameObject.SetActive(true);
            //        _image.preserveAspect = true;
            //        _image.sprite = answerImage.Spr;
            //        break;
            //    case AnswerFull answerFull:
            //        _image.gameObject.SetActive(true);
            //        _image.preserveAspect = true;
            //        _image.sprite = answerFull.Spr;
            //        _audioclip = answerFull.Audio;
            //        _soundButtonParent.SetActive(true);
            //        break;
            //    default:
            //        break;
            //}
        }

        string SetOrder(int index)
        {
            string order = "";
            if (index >= 0) order = Convert.ToChar(index + 65) + ". ";
            return order;
        }

        /// <summary>
        /// Plays the audioclip that is assigned to the answer.
        /// </summary>
        public void PlayAudio() => _audioSource.PlayOneShot(_audioclip);

        public void PlayAudioWrong() => _audioSource.PlayOneShot(_audioClipWrong);

        /// <summary>
        /// Adds necessary listeners.
        /// </summary>
        protected virtual void OnEnable() => Handle.PointerDown += OnSelected;

        /// <summary>
        /// Removes listeners.
        /// </summary>
        protected virtual void OnDisable() => Handle.PointerDown -= OnSelected;

        /// <summary>
        /// Called when the letter is picked up.
        /// </summary>
        private void OnSelected(PointerEventData eventData)
        {
            if (GameManager.instance.isCorrectAnswer) return;
            if (_isSelected)
            {
                Deselect();
            }
            else
            {
                _text.color = Color.white;
                _background.color = new Color32(235, 183, 43, 255);
                _backgroundShadow.color = new Color32(155, 120, 28, 255);
                _isSelected = true;
            }
            Selected?.Invoke(this);
            //_isSelected = !_isSelected;
        }

        /// <summary>
        /// Called when the answer is deselected.
        /// </summary>
        public void Deselect()
        {
            _text.color = Color.black;
            _background.color = _defaultBackgroundColor;
            _backgroundShadow.color = _defaultBackgroundShadowColor;
            _isSelected = false;
        }

        /// <summary>
        /// Called when the answer is part of an incorrect guess.
        /// </summary>
        public void WrongGuess()
        {
            _text.color = Color.white;
            _background.color = new Color32(191, 74, 56, 255);
            _backgroundShadow.color = new Color32(160, 66, 51, 255);
            _isSelected = false;
        }

        /// <summary>
        /// Called when the answer is part of an correct guess.
        /// </summary>
        public IEnumerator CorrectGuess(float flashDuration, float timeBetweenFlashes)
        {
            _text.color = Color.white;
            float timer = Time.time + flashDuration;

            while (Time.time < timer && _background != null)
            {
                _backgroundShadow.color = new Color32(26, 144, 57, 255);
                _background.color = new Color32(40, 207, 83, 255);

                yield return new WaitForSeconds(timeBetweenFlashes);

                if (_background == null)
                    break;
                _background.color = new Color32(45, 224, 91, 255);

                yield return new WaitForSeconds(timeBetweenFlashes);
            }
        }
    }
}