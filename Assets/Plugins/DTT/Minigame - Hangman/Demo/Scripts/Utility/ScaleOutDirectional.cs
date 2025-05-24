using System;
using System.Collections;
using UnityEngine;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// A behaviour that scales out a transform in a direction.
    /// </summary>
    public class ScaleOutDirectional : MonoBehaviour
    {
        /// <summary>
        /// The directions to start the scaling from.
        /// </summary>
        private enum Direction
        {
            /// <summary>
            /// The left side.
            /// </summary>
            [InspectorName("Left")]
            LEFT,
            
            /// <summary>
            /// The right side.
            /// </summary>
            [InspectorName("Right")]
            RIGHT,
            
            /// <summary>
            /// The top side.
            /// </summary>
            [InspectorName("Top")]
            TOP,
            
            /// <summary>
            /// The bottom side.
            /// </summary>
            [InspectorName("Bottom")]
            BOTTOM
        }

        /// <summary>
        /// The starting side.
        /// </summary>
        [SerializeField]
        private Direction _startSide;

        /// <summary>
        /// Whether the scaling has been completed.
        /// </summary>
        public bool Completed => Vector3.Distance(transform.localScale, _targetScale) < 0.1f;

        /// <summary>
        /// Whether the scaling animation is still busy.
        /// </summary>
        public bool Animating { get; private set; }

        /// <summary>
        /// The local scale on start.
        /// </summary>
        [SerializeField, HideInInspector]
        private Vector3 _localScaleOnStart;

        /// <summary>
        /// The local position on start.
        /// </summary>
        [SerializeField, HideInInspector]
        private Vector3 _localPositionOnStart;

        /// <summary>
        /// The target position to move towards.
        /// </summary>
        [SerializeField, HideInInspector]
        private Vector3 _targetPosition;

        /// <summary>
        /// The target scale to scale towards.
        /// </summary>
        [SerializeField, HideInInspector]
        private Vector3 _targetScale;

        /// <summary>
        /// Sets starting state values.
        /// </summary>
        private void Start() => SetStateValues();

        /// <summary>
        /// Resets the scaling.
        /// </summary>
        public void Reset()
        {
            StopAllCoroutines();
            transform.localScale = _localScaleOnStart;
            transform.localPosition = _localPositionOnStart;
        }

        /// <summary>
        /// Sets state values.
        /// </summary>
        private void OnValidate() => SetStateValues();

        /// <summary>
        /// Starts the scaling of the transform.
        /// </summary>
        /// <param name="time">The time to take for the scaling.</param>
        /// <param name="service">The game service.</param>
        public void StartScaling(float time, HangmanService service) => StartCoroutine(ScaleInRoutine(time, service));

        /// <summary>
        /// Returns and enumerator that does the scaling of the transform.
        /// </summary>
        /// <param name="animateTime">The time to take for the animation.</param>
        /// <param name="service">The game service.</param>
        /// <returns>Waits for the given animation time.</returns>
        private IEnumerator ScaleInRoutine(float animateTime, HangmanService service)
        {
            Animating = true;
            
            float time = 0.0f;
            while (time < animateTime)
            {
                // Wait for a pause yield if the service is paused.
                if (service.IsPaused)
                    yield return new WaitForService(service);
                
                float percentage = time / animateTime;
                
                transform.localScale = Vector3.Lerp(_localScaleOnStart, _targetScale, percentage);
                transform.localPosition = Vector3.Lerp(_localPositionOnStart, _targetPosition, percentage);
                yield return null;
                
                time += Time.deltaTime;
            }

            transform.localScale = _targetScale;
            transform.localPosition = _targetPosition;

            Animating = false;
        }

        /// <summary>
        /// Returns the target scale based on the selected starting side.
        /// </summary>
        /// <returns>The target scale.</returns>
        private Vector3 GetTargetScale()
        {
            float z = transform.localScale.z;
            if (_startSide == Direction.LEFT || _startSide == Direction.RIGHT)
                return new Vector3(0f, 1f, z);

            return new Vector3(1f, 0f, z);
        }

        /// <summary>
        /// Returns the target position based on the selected starting side.
        /// </summary>
        /// <returns>The target position.</returns>
        private Vector3 GetTargetPosition()
        {
            Vector3 position = transform.localPosition;
            switch (_startSide)
            {
                case Direction.LEFT:
                    position.x += _localScaleOnStart.x * 0.5f;
                    break;
                
                case Direction.RIGHT:
                    position.x -= _localScaleOnStart.x * 0.5f;
                    break;
                
                case Direction.TOP:
                    position.y -= _localScaleOnStart.y * 0.5f;
                    break;
                
                case Direction.BOTTOM:
                    position.y += _localScaleOnStart.y * 0.5f;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return position;
        }

        /// <summary>
        /// Sets the start and target positions.
        /// </summary>
        private void SetStateValues()
        {
            _localPositionOnStart = transform.localPosition;
            _localScaleOnStart = transform.localScale;
            
            _targetPosition = GetTargetPosition();
            _targetScale = GetTargetScale();
        }
    }
}