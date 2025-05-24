using DTT.MinigameBase.Handles;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DTT.MiniGame.Anagram.UI
{
    /// <summary>
    /// Behaviour that handles the snapping and events of the anagram letters.
    /// </summary>
    [RequireComponent(typeof(MoveHandle))]
    public class AnagramLetter : SnapObject
    {
        /// <summary>
        /// Text the letter should be drawn to.
        /// </summary>
        [SerializeField]
        [Tooltip("Text the letter should be drawn to.")]
        private Text _text;

        /// <summary>
        /// Invoked when the letters is picked up.
        /// </summary>
        public event Action<AnagramLetter> PickUp;

        /// <summary>
        /// Invoked when the letter is dropped.
        /// </summary>
        public event Action<AnagramLetter> Drop;

        /// <summary>
        /// Reference to the <see cref="UI.MoveHandle"/> of this object.
        /// </summary>
        public MoveHandle MoveHandle { get; private set; }

        /// <summary>
        /// Character this object represents.
        /// </summary>
        public char Letter { get; private set; }

        /// <summary>
        /// Last snap point the letter snapped to.
        /// </summary>
        public AnagramSnapPoint LastSnapPoint { get; private set; }

        /// <summary>
        /// Gets necessary components.
        /// </summary>
        protected virtual void Awake() => MoveHandle = GetComponent<MoveHandle>();

        /// <summary>
        /// Adds necessary listeners.
        /// </summary>
        protected virtual void OnEnable()
        {
            MoveHandle.PointerUp += OnDrop;
            MoveHandle.PointerDown += OnPickUp;
        }

        /// <summary>
        /// Removed listeners.
        /// </summary>
        protected virtual void OnDisable()
        {
            MoveHandle.PointerUp -= OnDrop;
            MoveHandle.PointerDown -= OnPickUp;
        }

        /// <summary>
        /// Initializes the letter.
        /// </summary>
        /// <param name="letter">Character of this component.</param>
        internal void Init(char letter)
        {
            Letter = letter;
            _text.text = letter.ToString();
        }

        /// <summary>
        /// Sets the current snapping point.
        /// </summary>
        /// <param name="snapPoint">New snap point.</param>
        internal void SetCurrentSnapPoint(AnagramSnapPoint snapPoint) => LastSnapPoint = snapPoint;

        /// <summary>
        /// Snaps this object to a certain position.
        /// Override to implement your own behaviour for when the object snaps.
        /// </summary>
        /// <param name="position">Position to snap to.</param>
        public override void SnapToPosition(Vector2 position) => RectTransform.position = position;

        /// <summary>
        /// Called when the letter is dropped.
        /// </summary>
        private void OnDrop(PointerEventData eventData) => Drop?.Invoke(this);

        /// <summary>
        /// Called when the letter is picked up.
        /// </summary>
        private void OnPickUp(PointerEventData eventData) => PickUp?.Invoke(this);
    }
}