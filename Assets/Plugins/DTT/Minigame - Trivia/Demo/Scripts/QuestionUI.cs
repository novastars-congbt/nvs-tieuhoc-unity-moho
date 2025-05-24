using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.Trivia.Demo
{
    /// <summary>
    /// UI class to display a question.
    /// </summary>
    public class QuestionUI : MonoBehaviour
    {
        /// <summary>
        /// Component to display an image.
        /// </summary>
        [SerializeField]
        private Image _image;

        /// <summary>
        /// Layout group element for the image.
        /// </summary>
        [SerializeField]
        private GameObject _imageLayoutPosition;

        /// <summary>
        /// Layout group element for the audio.
        /// </summary>
        [SerializeField]
        private GameObject _audioLayoutPosition;

        /// <summary>
        /// Layout group element for the text.
        /// </summary>
        [SerializeField]
        private GameObject _textLayoutPosition;

        /// <summary>
        /// Reference to the audiosource component.
        /// </summary>
        [SerializeField]
        private AudioSource _audioSource;

        /// <summary>
        /// Component to display text.
        /// </summary>
        [SerializeField]
        private Text _text;

        /// <summary>
        /// The data class for this question.
        /// </summary>
        public /*Question*/ QuestionManager.Question QuestionData { get; private set; }

        public AudioClip audioClipFinish;

        public int index;

        public bool isNotNumbered;

        /// <summary>
        /// Sets the necessary component data based on the question type.
        /// </summary>
        /// <param name="question">The given question type.</param>
        public void Initialize(QuestionManager.Question question)
        {
            QuestionData = question;
            string numbered = "";
            string ques = "";
            if (!isNotNumbered) numbered = "Câu " + (index + 1) + ": ";
            ques = question.Title.Replace("\\n", "\n");
            if (question.MatchCorrectAnswers) _text.text = numbered + ques + "\n" + "<size=25>(Có thể chọn nhiều đáp án)</size>";
            else _text.text = numbered + ques;
            _image.sprite = question.Spr;
            _imageLayoutPosition.SetActive(question.Spr);
            _image.enabled = true;
            _image.preserveAspect = true;
            _audioSource.clip = question.Audio;
            _audioLayoutPosition.SetActive(question.Audio);
            _textLayoutPosition.SetActive(question.UseTitle);
            audioClipFinish = question.AudioClipFinish;


            //_imageLayoutPosition.SetActive(false);
            //_audioLayoutPosition.SetActive(false);
            //switch (question)
            //{
            //    case QuestionImage questionImage:
            //        _imageLayoutPosition.SetActive(true);
            //        _image.enabled = true;
            //        _image.preserveAspect = true;
            //        _image.sprite = questionImage.Spr;
            //        _textLayoutPosition.SetActive(questionImage.UseTitle);
            //        audioClipFinish = questionImage.AudioClipFinish;
            //        break;
            //    case QuestionAudio questionAudio:
            //        _audioSource.clip = questionAudio.Audio;
            //        _audioLayoutPosition.SetActive(true);
            //        _textLayoutPosition.SetActive(questionAudio.UseTitle);
            //        audioClipFinish = questionAudio.AudioClipFinish;
            //        break;
            //    case QuestionFull questionFull:
            //        _imageLayoutPosition.SetActive(true);
            //        _image.enabled = true;
            //        _image.preserveAspect = true;
            //        _image.sprite = questionFull.Spr;
            //        _audioSource.clip = questionFull.Audio;
            //        _audioLayoutPosition.SetActive(true);
            //        _textLayoutPosition.SetActive(questionFull.UseTitle);
            //        audioClipFinish = questionFull.AudioClipFinish;
            //        break;
            //    default:
            //        _image.enabled = false;
            //        break;
            //}
        }

        /// <summary>
        /// Plays the audio of the question.
        /// </summary>
        public void PlayQuestionAudio() => _audioSource.Play();

        public void PlayQuestionAudioFinish() => _audioSource.PlayOneShot(audioClipFinish);
    }
}