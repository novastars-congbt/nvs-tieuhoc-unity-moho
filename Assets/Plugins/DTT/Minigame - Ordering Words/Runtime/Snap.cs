using System;
using UnityEngine;
using UnityEngine.Events;

namespace DTT.OrderingWords
{
    ///<summary>
    /// Makes an object snappable.
    ///</summary>
    public class Snap : MonoBehaviour
    {
        /// <summary>
        /// The time it takes to complete the snapping.
        /// </summary>
        [SerializeField]
        [Tooltip("The time it takes to complete the snapping")]
        private float _snapTime = 0.5f;

        /// <summary>
        /// Invokes when snapping is complete.
        /// </summary>
        public event Action OnSnapped;

        /// <summary>
        /// The word component for the draggable word.
        /// </summary>
        private Word _draggableWord;

        /// <summary>
        /// Get the current draggable word and subscribe to its events.
        /// </summary>
        private void OnEnable()
        {
            _draggableWord = GetComponent<Word>();
            _draggableWord.OnDrop += DoSnap;
        }

        /// <summary>
        /// Unsubscribe from events.
        /// </summary>
        private void OnDisable() => _draggableWord.OnDrop -= DoSnap;

        /// <summary>
        /// Snap to a snappingpoint if applicable and snap back if not.
        /// </summary>
        private void DoSnap()
        {
            SnappingPoint snappingPoint = _draggableWord.GetScriptFromRaycast<SnappingPoint>(this.gameObject);
            if (snappingPoint != null)
            {
                _draggableWord.SetSnappingPoint(snappingPoint);
                snappingPoint.SnapArea.raycastTarget = false;
                SnapToPosition(snappingPoint.transform.position);
            }
        }

        /// <summary>
        /// Snap to a position.
        /// </summary>
        /// <param name="to">the position it should snap to</param>
        /// <param name="onFinish">what should happen when the leantween is done</param>
        private void SnapToPosition(Vector3 to, UnityAction onFinish = null)
        {
            _draggableWord.SetRayCast(true);
            Vector2 from = transform.position;
            StartCoroutine(Animations.Value(0, 1, _snapTime, (value) => OnSnapAnimation(value, from, to), () => OnCompleteSnap(to, onFinish)));
        }

        /// <summary>
        /// Sets the values for the when the snap animation is playing.
        /// </summary>
        /// <param name="value">The speed of the animation</param>
        /// <param name="from">the initial position</param>
        /// <param name="to">the position it should snap to</param>
        private void OnSnapAnimation(float value, Vector3 from, Vector3 to)
        {
            bool isSnapping = _draggableWord.SnappingPoint != null;

            if (isSnapping && _draggableWord.SnappingPoint == null) 
                return;
            
            Vector3 pos = new Vector3(
                Mathf.Lerp(@from.x, to.x, value),
                Mathf.Lerp(@from.y, to.y, value),
                transform.position.z);

            transform.position = pos;

            // Get invoked once, when it reaches (almost) it destination. Gives a headstart to invoke before onComplete().
            if (Mathf.Abs(Vector2.Distance(pos, to)) < 0.01f)
            {
                OnSnapped?.Invoke();
            }
        }

        /// <summary>
        /// Sets the values for when the snap animations has played.
        /// </summary>
        /// <param name="to">the position it should snap to</param>
        /// <param name="onFinish">what should happen when the leantween is done</param>
        private void OnCompleteSnap(Vector3 to, UnityAction onFinish = null)
        {
            if (_draggableWord.SnappingPoint != null)
                _draggableWord.transform.position = to;

            onFinish?.Invoke();
        }
    }
}
