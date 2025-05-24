using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DTT.MinigameBase.Handles;

namespace DTT.OrderingWords
{
    /// <summary>
    /// Class that handles a Word object and it's parts.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class Word : Handle
    {
        /// <summary>
        /// The text component present in the item.
        /// </summary>
        [SerializeField]
        [Tooltip("The text component present in the item")]
        private Text _textComponent;

        /// <summary>
        /// The background image of the word.
        /// </summary>
        [SerializeField]
        [Tooltip("The background component of the item")]
        private Image _backgroundImage;

        /// <summary>
        /// The minimum width of the text component.
        /// </summary>
        [SerializeField]
        [Tooltip("The minimum width of the text component")]
        private float _textMinWidth;

        /// <summary>
        /// The maximum width of the text component.
        /// </summary>
        [SerializeField]
        [Tooltip("The maximum width of the text component")]
        private float _textMaxWidth;

        /// <summary>
        /// Delegate for the dropping event.
        /// </summary>
        public delegate void UserAction();

        /// <summary>
        /// Event fired when the item completed snapping to a snapping point.
        /// </summary>
        public event UserAction OnSnap;

        /// <summary>
        /// Event fired when the item is dropped without snapping.
        /// </summary>
        public event UserAction OnDrop;

        /// <summary>
        /// Event fired when the item is dropped without snapping.
        /// </summary>
        public event UserAction OnTap;

        /// <summary>
        /// Wether or not the word is draggable.
        /// </summary>
        public bool CanDrag => _canDrag;

        /// <summary>
        /// The snappingPoint reference if it is currently on one.
        /// </summary>
        public SnappingPoint SnappingPoint
        {
            get => _snappingPoint;
            set => _snappingPoint = value;
        }

        /// <summary>
        /// The last snapping point the item snapped to before being dragged.
        /// </summary>
        public SnappingPoint LastSnappingPoint
        {
            get => _lastSnappingPoint;
            set => _lastSnappingPoint = value;
        }

        /// <summary>
        /// The text currently displayed on the item.
        /// </summary>
        public string TextComponent => _textComponent.text;

        /// <summary>
        /// Get the background color of the item.
        /// </summary>
        /// <returns>The item's background color</returns>
        public Color BackgroundColor
        {
            get => _backgroundImage.color;
            set => _backgroundImage.color = value;
        }

        /// <summary>
        /// The default color of the word.
        /// </summary>
        public Color DefaultColor
        {
            get => _defaultColor;
            set => _defaultColor = value;
        }

        /// <summary>
        /// The transform of the object to move.
        /// </summary>
        private RectTransform _rectTransform;

        /// <summary>
        /// Set when correct answer, so doesn't get turned back on again.
        /// </summary>
        private bool _turnedOffForever;

        /// <summary>
        /// Reference to the Canvas Group so we can disable raycast.
        /// </summary>
        private CanvasGroup _thisCanvas;

        /// <summary>
        /// The default color of the word.
        /// </summary>
        private Color _defaultColor;

        /// <summary>
        /// The last snapping point the item snapped to before being dragged.
        /// </summary>
        private SnappingPoint _lastSnappingPoint;

        /// <summary>
        /// The snappingPoint reference if it is currently on one.
        /// </summary>
        private SnappingPoint _snappingPoint;

        /// <summary>
        /// Wether or not the word is draggable.
        /// </summary>
        private bool _canDrag;

        /// <summary>
        /// Gets the button, saves its initial position and allow it to drag.
        /// </summary>
        private void Awake()
        {
            _canDrag = true;
            _rectTransform = (RectTransform)transform;
            _thisCanvas = this.gameObject.GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Subscribe to events.
        /// </summary>
        private void OnEnable()
        {
            Drag += OnDrag;
            PointerUp += AnimateDrop;
            PointerDown += AnimateDrag;
        }

        /// <summary>
        /// Unsubscribe to events.
        /// </summary>
        private void OnDisable()
        {
            Drag -= OnDrag;
            PointerUp -= AnimateDrop;
            PointerDown -= AnimateDrag;
        }

        /// <summary>
        /// Set the display value of the item.
        /// </summary>
        /// <param name="value">The text to display on the item</param>
        /// <param name="backgroundColor">The color of the item</param>
        /// <param name="onSnap">Action invoked on snapping</param>
        /// <param name="onDrag">Action invoked on clicking/touching the item</param>
        /// <param name="onDrop">Action invoked when the item in released</param>
        public void Init(string value, Color backgroundColor, UserAction onSnap = null, UserAction onTap = null, UserAction onDrop = null)
        {
            _defaultColor = backgroundColor;
            _backgroundImage.color = _defaultColor;
            SetText(value);

            OnTap += onTap;
            OnSnap += onSnap;
            OnDrop += onDrop;
        }

        /// <summary>
        ///  Setting the snapping point of the word.
        /// </summary>
        /// <param name="snappingPoint">The new snapping point</param>
        public void SetSnappingPoint(SnappingPoint snappingPoint)
        {
            if (snappingPoint != null)
            {
                ((SentenceSolverSnappingPoint)snappingPoint).SetSnappedWord(this);

                _snappingPoint = snappingPoint;
                _snappingPoint.SnapArea.raycastTarget = true;
            }
            else
            {
                _snappingPoint = snappingPoint;
            }
            OnSnap?.Invoke();
        }

        /// <summary>
        /// Execute the animation before any other subscribed events.
        /// </summary>
        /// <param name="eventData">The pointer data payload</param>
        public void OnEndDrag(PointerEventData eventData) => AnimateDrop();
     
        /// <summary>
        /// Set the transparency of the item.
        /// </summary>
        /// <param name="alpha">The alpha value (0-1)</param>
        public void SetTransparency(float alpha)
        {
            if (_thisCanvas)
                _thisCanvas.alpha = alpha;
        }

        /// <summary>
        /// Turn the raycast on or off so that the item can't be dragged anymore.
        /// </summary>
        /// <param name="active">True enables raycast, false disables</param>
        /// <param name="finalTurnOff">If it now turned off forever</param>
        public void SetRayCast(bool active, bool finalTurnOff = false)
        {
            if (_turnedOffForever)
                return;

            _thisCanvas.blocksRaycasts = active;
            _turnedOffForever = finalTurnOff;
        }

        /// <summary>
        /// Get a ray cast hit list at the current input position.
        /// </summary>
        /// <returns>List of all the items that are hit on the current input position (Only items that have raycast enabled)</returns>
        /// <param name="excludeObject">The object to be excluded from the item list</param>
        public T GetScriptFromRaycast<T>(GameObject excludeObject) where T : class
        {
            List<RaycastResult> hitResults = new List<RaycastResult>();
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            EventSystem.current.RaycastAll(pointer, hitResults);

            foreach (RaycastResult raycastResult in hitResults)

                if (!raycastResult.gameObject.Equals(excludeObject) && raycastResult.gameObject.GetComponent<T>() != null)
                    return raycastResult.gameObject.GetComponent<T>();
                else if (!raycastResult.gameObject.transform.parent.gameObject.Equals(excludeObject) && raycastResult.gameObject.transform.parent.GetComponentInParent<T>() != null)
                    return raycastResult.gameObject.GetComponentInParent<T>();

            return null;
        }

        /// <summary>
        /// Animate the dragging of the item.
        /// </summary>
        private void AnimateDrag(PointerEventData eventData = null)
        {
            OnTap?.Invoke();

            if (_snappingPoint != null)
                ((SentenceSolverSnappingPoint)_snappingPoint).SetSnappedWord(null);

            _snappingPoint = null;

            gameObject.GetComponent<WordAnimations>().ScaleAnimation(1.2f);
            gameObject.GetComponent<WordAnimations>().ResetRotationAnimation();
        }

        /// <summary>
        /// Animate the dropping of the item.
        /// </summary>
        /// <param name="eventData">The pointer event</param>
        private void AnimateDrop(PointerEventData eventData = null)
        {
            gameObject.GetComponent<WordAnimations>().RotateAnimation(0);
            gameObject.GetComponent<WordAnimations>().ScaleAnimation(1, SnapOrDrop, () => SetRayCast(true));
        }

        /// <summary>
        /// Checks wether to drop or snap to a snapping point.
        /// </summary>
        private void SnapOrDrop()
        {
            if (_snappingPoint == null)
            {
                SetRayCast(true);
                OnDrop?.Invoke();
            }
            else
            {
                OnSnap?.Invoke();
            }
        }

        /// <summary>
        /// Set the text of the item and size the item accordingly.
        /// </summary>
        /// <param name="text">The text to set</param>
        private void SetText(string text)
        {
            _textComponent.text = text;

            // Limit the width of the text box within certain margins.
            RectTransform textRect = (RectTransform)_textComponent.transform;
            float width = textRect.sizeDelta.x > _textMaxWidth ? _textMaxWidth : textRect.sizeDelta.x < _textMinWidth ? _textMinWidth : textRect.sizeDelta.x;
            if (textRect.sizeDelta.x > 340)
                textRect.sizeDelta = new Vector2(width, textRect.sizeDelta.y);
        }

        /// <summary>
        /// Adds the position change to the object.
        /// </summary>
        /// <param name="eventData">The data about the pointer event.</param>
        private void OnDrag(PointerEventData eventData)
        {
            //rectTransform.anchoredPosition += eventData.delta;
            var position = Camera.main.ScreenToWorldPoint(eventData.position);
            position.z = transform.position.z;
            transform.position = position;
        }
    }
}
