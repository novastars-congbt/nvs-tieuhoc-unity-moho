using System.Collections;
//using UnityEditor.XR;
using UnityEngine;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// Represents the behaviour of a petal part of a sunflower.
    /// </summary>
    public class PetalBehaviour : ScenarioPartBehaviour
    {
        /// <summary>
        /// The time until the petal is dropped.
        /// </summary>
        [SerializeField]
        private float _dropTime = 1.0f;

        /// <summary>
        /// The renderer of the petal.
        /// </summary>
        [SerializeField]
        private SpriteRenderer _renderer;

        /// <summary>
        /// Whether the petal is completely dropped.
        /// </summary>
        public override bool Unlocked => !_dropping && _dropped /*Vector3.Distance(transform.position, _droppedPosition) < 0.1f*/;

        /// <summary>
        /// Whether the petal is currently dropping.
        /// </summary>
        public override bool Unlocking => _dropping && !_dropped;

        /// <summary>
        /// Whether the petal is currently dropping.
        /// </summary>
        public bool _dropping;
        public bool _dropped;

        /// <summary>
        /// The dropped color of the petal.
        /// </summary>
        private readonly Color _droppedColor = new Color32(135, 135, 135, 255);

        /// <summary>
        /// The dropped position.
        /// </summary>
        private Vector3 _droppedPosition;

        /// <summary>
        /// The start position.
        /// </summary>
        private Vector3 _startPosition;

        /// <summary>
        /// The starting color.
        /// </summary>
        private Color _startColor;

        /// <summary>
        /// The local euler angles on start.
        /// </summary>
        private Vector3 _startLocalEulerAngles;

        /// <summary>
        /// The target local euler angles.
        /// </summary>
        private Vector3 _targetLocalEulerAngles;

        /// <summary>
        /// Sets up the start and target state of the petal.
        /// </summary>
        private IEnumerator Start()
        {
            // Waits a frame until the rotation of the pedals is set.
            //yield return null;
            yield return new WaitForSeconds(0.1f);
            _startPosition = transform.position;
            _startLocalEulerAngles = transform.localEulerAngles;
            _targetLocalEulerAngles = transform.localEulerAngles.z >= 90 ? new Vector3(0f, 0f, 170f) : new Vector3(0f, 0f, 10f);
            _startColor = _renderer.color;

            SpriteRenderer leavesRenderer = GetComponentInParent<SunflowerScenarioController>().LeavesRenderer;
            _droppedPosition = leavesRenderer.transform.position;
            _droppedPosition.x = _startPosition.x;
            _droppedPosition.y -= (leavesRenderer.bounds.size.y * 0.5f);
            _droppedPosition = _droppedPosition.RandomOffset(Vector3.up, 0.75f);
        }

        /// <summary>
        /// Unlocks the petal by making it drop.
        /// </summary>
        /// <param name="service">The game service.</param>
        public override void Unlock(HangmanService service)
        {
            if (!this)
                return;

            if (_renderer) _renderer.sortingOrder = RandomExtensions.FiftyFifty() ? 3 : 5;
            StartCoroutine(DropEnumerator(service));
        }

        /// <summary>
        /// Locks the petal by resetting it to its starting state.
        /// </summary>
        public override void Lock()
        {
            transform.position = _startPosition;
            transform.localEulerAngles = _startLocalEulerAngles;
            _renderer.color = _startColor;
        }

        /// <summary>
        /// Returns an enumerator that drops the petal down linearly interpolating its state to the stored target state.
        /// </summary>
        /// <param name="service">The game service.</param>
        private IEnumerator DropEnumerator(HangmanService service)
        {
            _dropping = true;

            float time = 0.0f;
            while (time <= _dropTime)
            {
                // Wait for a pause yield if the service is paused.
                if (service.IsPaused)
                    yield return new WaitForService(service);

                float percentage = EaseInQuart(time / _dropTime);
                transform.position = Vector3.Lerp(_startPosition, _droppedPosition, percentage);
                transform.localEulerAngles = Vector3.Lerp(_startLocalEulerAngles, _targetLocalEulerAngles, percentage);
                _renderer.color = Color.Lerp(_startColor, _droppedColor, percentage);

                yield return null;

                time += Time.deltaTime;
            }

            _dropping = false;
            _dropped = true;
            SunflowerScenarioController.instance.EndGame();
        }

        /// <summary>
        /// Returns an eased value by multiplying given by itself.
        /// </summary>
        /// <param name="value">The value to ease.</param>
        /// <returns>The eased value.</returns>
        private float EaseInQuart(float value) => value * value;
    }
}