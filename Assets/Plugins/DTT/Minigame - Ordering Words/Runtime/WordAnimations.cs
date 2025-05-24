using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DTT.OrderingWords
{
    /// <summary>
    /// Class that handles the animations of the word.
    /// </summary>
    public class WordAnimations : MonoBehaviour
    {
        /// <summary>
        /// The time for the color fading animation.
        /// </summary>
        [SerializeField]
        [Tooltip("The time for the color fading animation")]
        private float _colorFlashingAnimationTime = 0.6f;

        /// <summary>
        /// The amount of flashes for the color fading animation.
        /// </summary>
        [SerializeField]
        [Tooltip("The amount of flashes for the color fading animation")]
        private int _colorFlashCount = 4;

        /// <summary>
        /// The time for the movement animation.
        /// </summary>
        [SerializeField]
        [Tooltip("The time for the movement animation")]
        private float _movementAnimationTime = 0.6f;

        /// <summary>
        /// The time for the scaling animation.
        /// </summary>
        [SerializeField]
        [Tooltip("The time for the scaling animation")]
        private float _scaleAnimationTime = 0.4f;

        /// <summary>
        /// The time for the rotating animation.
        /// </summary>
        [SerializeField]
        [Tooltip("The time for the rotating animation")]
        private float _rotateAnimationTime = 0.4f;

        /// <summary>
        /// The range for the word rotation/tilting.
        /// </summary>
        [SerializeField]
        [Tooltip("The range for the word rotation/tilting")]
        private float _rotationRange = 10f;

        /// <summary>
        /// The time for the color fadin animation.
        /// </summary>
        public float ColorFadingAnimationTime => _colorFlashingAnimationTime;

        /// <summary>
        /// Wether or not the color animation is playing.
        /// </summary>
        private bool _colorAnimationPlaying = false;

        /// <summary>
        /// Moves a specified word.
        /// </summary>
        /// <param name="target">The target position</param>

        private void OnDisable()
        {
            StopAllCoroutines();
            Debug.LogError("stop");
        }
        public void MoveAnimation(Vector2 target)
        {
            StartCoroutine(Animations.Value(this.transform.position.x, target.x, _movementAnimationTime, (value) => this.transform.position = new Vector3(value, this.transform.position.y, transform.parent.position.z)));
            StartCoroutine(Animations.Value(this.transform.position.y, target.y, _movementAnimationTime, (value) => this.transform.position = new Vector3(this.transform.position.x, value, transform.parent.position.z)));
        }

        /// <summary>
        /// Scaling of the word animation.
        /// </summary>
        /// <param name="to">End scale value.</param>
        public void ScaleAnimation(float to) => StartCoroutine(Animations.Value(transform.localScale.x, to, _scaleAnimationTime, (value) => transform.localScale = new Vector3(value, value, value)));

        /// <summary>
        /// Animation for the color fading of the word.
        /// </summary>
        /// <param name="target">The target color</param>
        /// <param name="OnComplete">The callback action</param>
        public IEnumerator ColorFlashingAnimation(Color target, Action OnComplete = null)
        {
            float time = _colorFlashingAnimationTime;
            int count = _colorFlashCount;
            Color originalColor = this.GetComponent<Word>().BackgroundColor;

            if (!_colorAnimationPlaying)
            {
                _colorAnimationPlaying = true;
                time = time / count;

                for (int i = 0; i < count; ++i)
                {
                    StartCoroutine(Animations.Value(originalColor.r, target.r, time, (value) => this.GetComponent<Word>().BackgroundColor = new Color(value, this.GetComponent<Word>().BackgroundColor.g, this.GetComponent<Word>().BackgroundColor.b)));
                    StartCoroutine(Animations.Value(originalColor.g, target.g, time, (value) => this.GetComponent<Word>().BackgroundColor = new Color(this.GetComponent<Word>().BackgroundColor.r, value, this.GetComponent<Word>().BackgroundColor.b)));
                    StartCoroutine(Animations.Value(originalColor.b, target.b, time, (value) => this.GetComponent<Word>().BackgroundColor = new Color(this.GetComponent<Word>().BackgroundColor.r, this.GetComponent<Word>().BackgroundColor.g, value)));
                    yield return new WaitForSeconds(time);
                    StartCoroutine(Animations.Value(target.r, originalColor.r, time, (value) => this.GetComponent<Word>().BackgroundColor = new Color(value, this.GetComponent<Word>().BackgroundColor.g, this.GetComponent<Word>().BackgroundColor.b)));
                    StartCoroutine(Animations.Value(target.g, originalColor.g, time, (value) => this.GetComponent<Word>().BackgroundColor = new Color(this.GetComponent<Word>().BackgroundColor.r, value, this.GetComponent<Word>().BackgroundColor.b)));
                    StartCoroutine(Animations.Value(target.b, originalColor.b, time, (value) => this.GetComponent<Word>().BackgroundColor = new Color(this.GetComponent<Word>().BackgroundColor.r, this.GetComponent<Word>().BackgroundColor.g, value)));
                    yield return new WaitForSeconds(time);

                }
                OnComplete();
                _colorAnimationPlaying = false;
            }
        }

        /// <summary>
        /// Animation for the transparency fading of the word.
        /// </summary>
        /// <param name="from">Start transparency</param>
        /// <param name="to">Target transparency</param>
        /// <param name="OnComplete">The callback action</param>
        public void TransparencyAnimation(float from, float to, Action OnComplete = null)
        {
            Color newAlphaColor = this.GetComponent<Image>().color;
            StartCoroutine(Animations.Value(from, to, _colorFlashingAnimationTime, (value) => this.GetComponent<Word>().SetTransparency(value), OnComplete));
        }

        /// <summary>
        /// Scaling of the word animation.
        /// </summary>
        public void ScaleAnimation(float to, Action OnStart = null, Action OnComplete = null)
        {
            StartCoroutine(Animations.Value(transform.localScale.x, to, _scaleAnimationTime, (value) => transform.localScale = new Vector3(value, value, value), OnComplete));
            OnStart();
        }

        /// <summary>
        /// Animation for the rotation of the word.
        /// </summary>
        /// <param name="from">The initial rotation</param>
        public void RotateAnimation(float from) => StartCoroutine(Animations.Value(from, Random.Range(-_rotationRange, _rotationRange), _rotateAnimationTime, (value) => gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, value)));

        /// <summary>
        /// Animation that reset the rotation of the word.
        /// </summary>
        /// 
        public void ResetRotationAnimation()
        {
            float from = gameObject.transform.eulerAngles.z;

            if (from > _rotationRange)
                from = -(360 - from);

            StartCoroutine(Animations.Value(from, 0, _rotateAnimationTime, (value) => gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, value)));
        }
    }
}
