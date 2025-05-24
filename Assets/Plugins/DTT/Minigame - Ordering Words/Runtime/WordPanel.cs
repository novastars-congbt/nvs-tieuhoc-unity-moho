using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace DTT.OrderingWords
{
    /// <summary>
    /// Class that manages the panel of words.
    /// </summary>
    public class WordPanel : MonoBehaviour
    {
        /// <summary>
        /// The panel holding all words that have not snapped.
        /// </summary>
        [SerializeField]
        [Tooltip("The panel holding all draggable items that have not snapped")]
        private RectTransform _draggablePanel;

        /// <summary>
        /// The prefab for draggable items used in the game.
        /// </summary>
        [SerializeField]
        [Tooltip("The prefab for draggable items used in the game")]
        private Word _draggablePrefab;

        /// <summary>
        /// The children of the panel.
        /// </summary>
        public List<GameObject> Words => _words;

        /// <summary>
        /// Action for when the word is snapped.
        /// </summary>
        public event Action WordSnap;

        /// <summary>
        /// Action for when the word is dragged.
        /// </summary>
        public event Action WordTap;

        /// <summary>
        /// The children of the panel.
        /// </summary>
        private List<GameObject> _words = new List<GameObject>();

        /// <summary>
        /// Creates the words for the exercise.
        /// </summary>
        public void CreateWords(List<string> words, List<Color> colors)
        {
            Random random = new Random();
            Color currentColor = colors[0];

            for (int i = 0; i < words.Count; ++i)
            {
                if (colors.Count <= i)
                    currentColor = colors[random.Next(0, colors.Count)];
                else
                    currentColor = colors[i];

                CreateDraggableWord(words[i], i, currentColor);
            }
            DivideItemsEvenly();
        }

        /// <summary>
        /// Create a draggable word item.
        /// </summary>
        /// <param name="word">The word to create</param>
        public void CreateDraggableWord(string word, int index, Color color)
        {
            Word draggable = Instantiate(_draggablePrefab, _draggablePanel);
            draggable.GetComponent<Word>().Init(word, color, SnapAction, TapAction, DropAction);
            AddItem(draggable.gameObject);

            void TapAction() => WordTap?.Invoke();

            void DropAction()
            {
                if (draggable.SnappingPoint == null)
                    AddItem(draggable.gameObject);
            }

            void SnapAction()
            {
                if (draggable.SnappingPoint != null)
                    draggable.transform.SetParent(draggable.SnappingPoint.transform);

                WordSnap?.Invoke();
            }
        }

        /// <summary>
        /// Add an item to the panel.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="updatePanel">Whether to update the panel upon adding the item</param>
        public void AddItem(GameObject item, bool updatePanel = true)
        {
            if (!_words.Contains(item))
                _words.Add(item);

            if (updatePanel)
                DivideItemsEvenly();
        }

        /// <summary>
        /// Remove an item from the panel.
        /// </summary>
        /// <param name="item">The item to remove</param>
        public void RemoveItem(GameObject item)
        {
            if (_words.Contains(item))
                _words.Remove(item);
        }

        /// <summary>
        /// Places all children on a grid within the panel.
        /// </summary>
        public void DivideItemsEvenly()
        {
            List<Vector2> gridPoints = GetGridPositions(4).ToList();
            List<GameObject> unassignedChildren = new List<GameObject>(_words);

            GameObject mostRecentChild = _words[_words.Count - 1];
            Vector2 closestPoint = GetClosestGridPoint(mostRecentChild, gridPoints);
            gridPoints.Remove(closestPoint);
            unassignedChildren.Remove(mostRecentChild);

            if (mostRecentChild.activeSelf) mostRecentChild.GetComponent<WordAnimations>().MoveAnimation(closestPoint);

            //Order the points by prioritizing points that have an item closest to them.
            gridPoints = gridPoints.OrderBy(point => GetClosestChildDistance(point, _words)).ToList();

            for (int i = 0; i < gridPoints.Count; ++i)
            {
                Vector2 gridPoint = gridPoints[i];
                GameObject closestChild = GetClosestChild(gridPoint, unassignedChildren);
                unassignedChildren.Remove(closestChild);

                if (closestChild.activeSelf) closestChild.GetComponent<WordAnimations>().MoveAnimation(gridPoint);
            } 
        }

        /// <summary>
        /// Clears all the words.
        /// </summary>
        public void ClearWords()
        {
            for (int i = 0; i < _words.Count; ++i)
                Destroy(_words[i].gameObject);
            
            _words.Clear();
        }

        /// <summary>
        /// Get an array of world space positions evenly divided over the panel.
        /// </summary>
        /// <param name="columns">The number of columns for each row</param>
        /// <returns>An array of world space positions</returns>
        private Vector2[] GetGridPositions(int columns)
        {
            RectTransform current = this.GetComponent<RectTransform>();
            Vector3[] objectCorners = new Vector3[4];
            current.GetWorldCorners(objectCorners);
            Rect worldRect = new Rect(objectCorners[0].x, objectCorners[0].y,
                    objectCorners[2].x - objectCorners[0].x, objectCorners[2].y - objectCorners[0].y);

            Vector2[] points = new Vector2[_words.Count];
            int rowCount = Mathf.CeilToInt(_words.Count / (float)columns);
            float spacingY = worldRect.height / rowCount;
            int index = 0;
            for (int y = 0; y < rowCount; ++y)
            {
                int columnCount = y < rowCount - 1 ? columns : columns - ((rowCount * columns) - _words.Count);
                float spacingX = worldRect.width / columnCount;
                for (int x = 0; x < columnCount; ++x)
                {
                    points[index] = new Vector2
                    {
                        x = worldRect.xMin + (spacingX * x) + (spacingX / 2),
                        y = worldRect.yMin + (spacingY * y) + (spacingY / 2)
                    };
                    index++;
                }
            }

            return points;
        }

        /// <summary>
        /// Get the closest item in the given array of children relative to the given point.
        /// </summary>
        /// <param name="from">The given point to calculate the distance from</param>
        /// <param name="children">The given children to calculate the distance from</param>
        /// <returns>The closest child to the given position</returns>
        private GameObject GetClosestChild(Vector2 from, IList<GameObject> children)
        {
            float closestDistance = -1;
            GameObject closestChild = null;
            for (int i = 0; i < children.Count; ++i)
            {
                GameObject child = children[i];
                float distance = Vector2.Distance(child.transform.position, from);
                if (closestDistance < 0 || distance < closestDistance)
                {
                    closestDistance = distance;
                    closestChild = child;
                }
            }
            return closestChild;
        }

        /// <summary>
        /// Get the distance to the closest item in the given array of children relative to the given point.
        /// </summary>
        /// <param name="from">The given point to calculate the distance from</param>
        /// <param name="children">The given children to calculate the distance from</param>
        /// <returns>The distance to the closest item</returns>
        private float GetClosestChildDistance(Vector2 from, IList<GameObject> children)
        {
            float closestDistance = -1;
            for (int i = 0; i < children.Count; ++i)
            {
                GameObject child = children[i];
                float distance = Vector2.Distance(child.transform.position, from);

                if (closestDistance < 0 || distance < closestDistance)
                    closestDistance = distance;
            }

            return closestDistance;
        }

        /// <summary>
        /// Get the closest point relative the the given item.
        /// </summary>
        /// <param name="from">The item to calculate the distance from</param>
        /// <param name="points">The points to calculate the distance to</param>
        /// <returns>The closest point to the item</returns>
        private Vector2 GetClosestGridPoint(GameObject from, IList<Vector2> points)
        {
            float closestDistance = -1;
            Vector2 closestPoint = Vector2.zero;
            for (int i = 0; i < points.Count; ++i)
            {
                Vector2 point = points[i];
                float distance = Vector2.Distance(from.transform.position, point);
                if (closestDistance < 0 || distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = point;
                }
            }
            return closestPoint;
        }
    }
}
