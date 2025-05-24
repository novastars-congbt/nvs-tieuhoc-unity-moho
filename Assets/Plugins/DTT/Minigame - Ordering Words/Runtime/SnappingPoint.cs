using UnityEngine;
using UnityEngine.UI;

namespace DTT.OrderingWords
{
    /// <summary>
    /// Class that handles a snapping point for the draggable words.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class SnappingPoint : MonoBehaviour
    {
        /// <summary>
        /// Reference to the image so the raycast can be disabled when you have to get something back out.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the image so the raycast can be disabled when you have to get something back out.")]
        protected Image _snapArea;

        /// <summary>
        /// Can you actually snap to this snapping point.
        /// </summary>
        public bool SnappAble => _snapArea.raycastTarget;

        /// <summary>
        /// Reference to the image so the raycast can be disabled when you have to get something back out.
        /// </summary>
        public Image SnapArea
        {
            get => _snapArea;
            set => _snapArea = value;
        }

        /// <summary>
        /// Gets the Image component of the object.
        /// </summary>
        protected virtual void Awake() => _snapArea = GetComponent<Image>();
    }
}
