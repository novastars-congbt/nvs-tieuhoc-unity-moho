using System;
using DTT.Utils.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.MiniGame.Anagram.UI
{
    /// <summary>
    /// Handles ordering SnapObjects in an ordered manner.
    /// It snaps the components to the closest possible spot.
    /// </summary>
    public class SnapBoard : MonoBehaviour
    {
        /// <summary>
        /// RectTransform of this object.
        /// </summary>
        public RectTransform RectTransform => (RectTransform)transform;

        /// <summary>
        /// Snapped objects to the board.
        /// </summary>
        private readonly List<SnapObject> _snappedObjects = new List<SnapObject>();

        /// <summary>
        /// The canvas size of the previous frame.
        /// </summary>
        private Vector2 _previousCanvasSize;

        /// <summary>
        /// Reference to the canvas of this object.
        /// </summary>
        private Canvas _canvas;

        /// <summary>
        /// Adds a snap object to the board.
        /// </summary>
        /// <param name="snapObject">Snapped object.</param>
        /// <param name="instant">
        /// Whether the object should instantly set on the snap position,
        /// or use the snap object snap animation.
        /// </param>
        public void AddSnapObject(SnapObject snapObject, bool instant = false)
        {
            int closestGridPos = 0;
            float closestDist = float.MaxValue;
            float closestObjectX = float.MaxValue;

            // Finds closest snap point.
            for (int i = 0; i < _snappedObjects.Count; i++)
            {
                float dist = Vector2.Distance(snapObject.RectTransform.position, _snappedObjects[i].RectTransform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestGridPos = i;
                    closestObjectX = _snappedObjects[i].RectTransform.position.x;
                }
            }

            if (closestObjectX < snapObject.RectTransform.position.x)
                closestGridPos++;

            // Checks if the object needs to be inserted.
            if (closestGridPos > _snappedObjects.Count)
                _snappedObjects.Add(snapObject);
            else
                _snappedObjects.Insert(closestGridPos, snapObject);

            SnapPositions(instant);
        }

        /// <summary>
        /// Removes a snap object from the snap board.
        /// </summary>        
        /// <param name="snapObject">Snapped object.</param>
        /// <param name="instant">
        /// Whether the object should instantly set on the snap position,
        /// or use the snap object snap animation.
        /// </param>
        public void RemoveSnapObject(SnapObject snapObject, bool instant = false)
        {
            if (!_snappedObjects.Remove(snapObject))
                return;

            SnapPositions(instant);
        }

        /// <summary>
        /// Removes all snapped objects from the board.
        /// </summary>
        public void ClearBoard() => _snappedObjects.Clear();

        /// <summary>
        /// Retrieves initial references and values.
        /// </summary>
        private void Start()
        {
            _canvas = GetComponentInParent<Canvas>();
            _previousCanvasSize = _canvas.pixelRect.size;
        }

        /// <summary>
        /// Checks if resolution has changed so snap points can be updated.
        /// </summary>
        private void Update()
        {
            if (_previousCanvasSize != _canvas.pixelRect.size)
            {
                SnapPositions(false);
            }
            
            _previousCanvasSize = _canvas.pixelRect.size;
        }

        /// <summary>
        /// Snaps a snap point.
        /// </summary>
        /// <param name="instant">Whether the animation should be instant.</param>
        private void SnapPositions(bool instant)
        {
            for (int i = 0; i < _snappedObjects.Count; i++)
            {
                if (instant)
                    _snappedObjects[i].RectTransform.position = GridPosToWorldPos(i);
                else
                    _snappedObjects[i].SnapToPosition(GridPosToWorldPos(i));
            }
        }

        /// <summary>
        /// Calculates what the world position of a layout point should be.
        /// </summary>
        /// <param name="point">Point of the layout to be calculated.</param>
        /// <returns>World position of the given point.</returns>
        private Vector2 GridPosToWorldPos(int point)
        {
            Rect worldRect = RectTransform.GetWorldRect();
            float stepSize = worldRect.width / (_snappedObjects.Count + 1);

            return new Vector2((worldRect.position.x) + (stepSize * (1 + point)), worldRect.center.y);
        }
    }
}