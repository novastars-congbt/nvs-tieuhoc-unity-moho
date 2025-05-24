using DTT.MiniGame.Anagram.Utils;
using System.Collections;
using UnityEngine;

namespace DTT.MiniGame.Anagram.UI
{
    /// <summary>
    /// Implementation of the <see cref="AnagramLetter"/> that smoothly snaps the letter.
    /// </summary>
    public class SmoothAnagramLetter : AnagramLetter
    {
        /// <summary>
        /// Time it takes for the letter to move.
        /// </summary>
        [SerializeField]
        [Tooltip("Time it takes for the letter to move.")]
        private float _moveTime = .15f;

        /// <summary>
        /// Current playing movement animation.
        /// </summary>
        private IEnumerator _currentAnimation;

        /// <summary>
        /// Stops the coroutine when the object is destroyed.
        /// </summary>
        protected virtual void OnDestroy() => StopAnimation(this);

        /// <summary>
        /// Adds listeners.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            PickUp += StopAnimation;
        }

        /// <summary>
        /// Removes listeners.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            PickUp -= StopAnimation;
        }

        /// <summary>
        /// Starts the animation coroutine.
        /// </summary>
        /// <param name="position">Goal position.</param>
        public override void SnapToPosition(Vector2 position)
        {
            StopAnimation(this);
            _currentAnimation = MoveToPosition(position);
            StartCoroutine(_currentAnimation);
        }

        /// <summary>
        /// Stops the current snap animation.
        /// </summary>
        private void StopAnimation(AnagramLetter letter)
        {
            if (_currentAnimation != null)
                StopCoroutine(_currentAnimation);
        }

        /// <summary>
        /// Smoothly moves the letter to the goal position.
        /// </summary>
        /// <param name="position">Goal position.</param>
        private IEnumerator MoveToPosition(Vector2 position)
        {
            Vector2 originalPosition = RectTransform.position;
            Vector2 distanceToGoal = position - originalPosition;
            yield return Animations.Value(0, 1, _moveTime,
                value => RectTransform.position = originalPosition + distanceToGoal * value);
        }
    }
}