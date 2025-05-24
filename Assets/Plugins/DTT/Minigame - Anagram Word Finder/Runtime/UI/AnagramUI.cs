using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DTT.Utils.Extensions;
using DTT.MiniGame.Anagram.Extensions;

namespace DTT.MiniGame.Anagram.UI
{
    /// <summary>
    /// Handles generating the UI elements and snapping letters to the snap points.
    /// </summary>
    public class AnagramUI : MonoBehaviour
    {
        /// <summary>
        /// Prefab for the letters.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab for the letters.")]
        private AnagramLetter _letterPrefab;

        /// <summary>
        /// Prefab for the snap points.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab for the snap points.")]
        private AnagramSnapPoint _snapPointPrefab;

        /// <summary>
        /// <see cref="SnapBoard"/> where the letters snap to when not used.
        /// </summary>
        [SerializeField]
        [Tooltip("Snap board where the letters snap to when not used.")]
        private SnapBoard _letterSnapBoard;

        /// <summary>
        /// Layout group for the snap points.
        /// </summary>
        [SerializeField]
        [Tooltip("Layout group for the snap points.")]
        private HorizontalLayoutGroup _snapLayoutGroup;

        /// <summary>
        /// Holder for when letters are being dragged.
        /// </summary>
        [SerializeField]
        [Tooltip("Holder for when letters are being dragged.")]
        private RectTransform _dragHolder;

        /// <summary>
        /// Holder for when the letters are dropped on a snap point.
        /// </summary>
        [SerializeField]
        [Tooltip("Holder for when the letters are dropped on a snap point.")]
        private RectTransform _dropHolder;

        /// <summary>
        /// Amount of times a letter was moved.
        /// </summary>
        public int LettersMoved { get; private set; }

        /// <summary>
        /// Invoked when all snap points are filled with a letter.
        /// </summary>
        public event Action<string> WordComplete;

        /// <summary>
        /// All current letter UI elements.
        /// </summary>
        private List<AnagramLetter> _letters = new List<AnagramLetter>();

        /// <summary>
        /// All current snap point UI elements.
        /// </summary>
        private List<AnagramSnapPoint> _snapPoints = new List<AnagramSnapPoint>();

        /// <summary>
        /// List holding the white spaces in the snap layout group.
        /// The object represent a space character in the word.
        /// </summary>
        private List<GameObject> _witeSpaces = new List<GameObject>();

        /// <summary>
        /// Current string used to generate the UI.
        /// </summary>
        private string _currentLetters;

        /// <summary>
        /// Adds listeners to the elements.
        /// </summary>
        private void OnEnable() => AddListeners();

        /// <summary>
        /// Removes listeners from the UI elements.
        /// </summary>
        private void OnDisable() => ClearListeners();

        /// <summary>
        /// Generates the UI elements of the anagram game.
        /// </summary>
        /// <param name="word">Word to be found.</param>
        public void GenerateUI(string word)
        {
            ClearBoard();
            _currentLetters = word;
            string randomizedWord = word.Shuffle();

            // Gets the estimated size of the snap points
            RectTransform layoutRect = (RectTransform)_snapLayoutGroup.transform;
            float gridSizeWidth =
                layoutRect.rect.width - _snapLayoutGroup.padding.left - _snapLayoutGroup.padding.right;
            float gridSizeHeight =
                layoutRect.rect.height - _snapLayoutGroup.padding.top - _snapLayoutGroup.padding.bottom;
            float letterSizeX = gridSizeWidth / (float)randomizedWord.Length;

            _snapLayoutGroup.childControlHeight = true;
            _snapLayoutGroup.childControlWidth = true;

            // Creates a letter and snap point for each letter in the word.
            for (int i = 0; i < randomizedWord.Length; i++)
            {
                // If the letter is a space, it fill the layout group with an empty object.
                if (_currentLetters[i] == ' ')
                {
                    GameObject whiteSpace = new GameObject();
                    whiteSpace.AddComponent<RectTransform>();
                    whiteSpace.transform.SetParent(_snapLayoutGroup.transform);
                    _witeSpaces.Add(whiteSpace);
                }
                else
                {
                    AnagramSnapPoint snapPoint = Instantiate(_snapPointPrefab, _snapLayoutGroup.transform);
                    _snapPoints.Add(snapPoint);

                    snapPoint.LetterSnapped += CheckWordComplete;
                    snapPoint.RemovedLetter += OnLetterReplaced;
                }

                if (randomizedWord[i] == ' ')
                    continue;

                // Creates a letter.
                AnagramLetter letterUI = Instantiate(_letterPrefab, _letterSnapBoard.transform);
                _letters.Add(letterUI);
                letterUI.Drop += OnLetterDropped;
                letterUI.PickUp += OnLetterPickedUp;
                letterUI.Init(randomizedWord[i]);
                _letterSnapBoard.AddSnapObject(letterUI, true);

                // Sets the size of the letters equal to the snap points.
                letterUI.RectTransform.sizeDelta = new Vector2(letterSizeX - _snapLayoutGroup.spacing, gridSizeHeight);
            }
        }

        /// <summary>
        /// Updates the size of the letters so it remains consistent.
        /// </summary>
        private void Update()
        {
            for (int i = 0; i < _letters.Count; i++)
            {
                RectTransform layoutRect = (RectTransform)_snapLayoutGroup.transform;
                float gridSizeWidth =
                    layoutRect.rect.width - _snapLayoutGroup.padding.left - _snapLayoutGroup.padding.right;
                float gridSizeHeight =
                    layoutRect.rect.height - _snapLayoutGroup.padding.top - _snapLayoutGroup.padding.bottom;
                float letterSizeX = gridSizeWidth / _currentLetters.Length;
                _letters[i].RectTransform.sizeDelta = new Vector2(letterSizeX - _snapLayoutGroup.spacing, gridSizeHeight);
            }
        }

        /// <summary>
        /// Removes the listeners and destroys all UI elements related to the anagram game.
        /// </summary>
        public void ClearBoard()
        {
            LettersMoved = 0;
            ClearListeners();

            foreach (AnagramLetter letter in _letters)
                Destroy(letter.gameObject);

            foreach (AnagramSnapPoint snapPoint in _snapPoints)
                Destroy(snapPoint.gameObject);

            foreach (GameObject whiteSpace in _witeSpaces)
                Destroy(whiteSpace);

            _letters.Clear();
            _snapPoints.Clear();
            _witeSpaces.Clear();
            _letterSnapBoard.ClearBoard();
        }

        /// <summary>
        /// Removes the listeners from the anagram UI elements.
        /// </summary>
        private void ClearListeners()
        {
            foreach (AnagramLetter letter in _letters)
            {
                letter.Drop -= OnLetterDropped;
                letter.PickUp -= OnLetterPickedUp;
            }

            foreach (AnagramSnapPoint snapPoint in _snapPoints)
            {
                snapPoint.LetterSnapped -= CheckWordComplete;
                snapPoint.RemovedLetter -= OnLetterReplaced;
            }
        }

        /// <summary>
        /// Adds the listeners to the anagram UI elements.
        /// </summary>
        private void AddListeners()
        {
            foreach (AnagramLetter letter in _letters)
            {
                letter.Drop += OnLetterDropped;
                letter.PickUp += OnLetterPickedUp;
            }

            foreach (AnagramSnapPoint snapPoint in _snapPoints)
            {
                snapPoint.LetterSnapped += CheckWordComplete;
                snapPoint.RemovedLetter += OnLetterReplaced;
            }
        }

        /// <summary>
        /// Sets the letters interactable.
        /// </summary>
        /// <param name="interactable">Whether the letters should be interactable.</param>
        public void SetInteractable(bool interactable)
        {
            foreach (AnagramLetter letter in _letters)
                letter.MoveHandle.enabled = interactable;
        }

        /// <summary>
        /// Handles picking up a letter.
        /// </summary>
        /// <param name="letter">Picked up letter.</param>
        private void OnLetterPickedUp(AnagramLetter letter)
        {
            _letterSnapBoard.RemoveSnapObject(letter);
            letter.transform.SetParent(_dragHolder);
        }

        /// <summary>
        /// Handles when a letter is replaced by another letter on one of the snap points.
        /// </summary>
        /// <param name="letter">Letter to be replaced.</param>
        public void OnLetterReplaced(AnagramLetter letter)
        {
            AnagramLetter newOccupant = letter.LastSnapPoint.CurrentLetter;

            if (letter != newOccupant && newOccupant.LastSnapPoint != null)
            {
                // Sets the letter on the last snap point of the other letter.
                letter.transform.SetParent(_dropHolder);
                newOccupant.LastSnapPoint.SetLetter(letter);
            }
            else
            {
                // Sets the letter back on the letter board.
                letter.transform.SetParent(_letterSnapBoard.transform);
                _letterSnapBoard.AddSnapObject(letter);
                letter.SetCurrentSnapPoint(null);
            }
        }

        /// <summary>
        /// Handles when a letter is dropped after being held.
        /// </summary>
        /// <param name="letter">Dropped letter.</param>
        public void OnLetterDropped(AnagramLetter letter)
        {
            List<AnagramSnapPoint> overlapping = new List<AnagramSnapPoint>();

            // Gets all overlapping snap points.
            foreach (AnagramSnapPoint snapPoint in _snapPoints)
            {
                if (letter.RectTransform.GetWorldRect().Overlaps(snapPoint.RectTransform.GetWorldRect()))
                    overlapping.Add(snapPoint);
            }

            // If none are found, snap back to the letter board.
            if (overlapping.Count == 0)
            {
                letter.SetCurrentSnapPoint(null);
                _letterSnapBoard.AddSnapObject(letter);
                letter.transform.SetParent(_letterSnapBoard.transform);
                return;
            }

            // Check which overlapping snap point is the closest.
            float currentDist = float.MaxValue;
            AnagramSnapPoint bestOverlap = null;
            foreach (AnagramSnapPoint overlap in overlapping)
            {
                float dist = Vector2.Distance(letter.RectTransform.GetWorldRect().center, overlap.RectTransform.GetWorldRect().center);
                if (dist < currentDist)
                {
                    currentDist = dist;
                    bestOverlap = overlap;
                }
            }

            letter.transform.SetParent(_dropHolder);
            bestOverlap.SnapLetter(letter);
        }

        /// <summary>
        /// Checks if all snap points have been filled and invokes
        /// <see cref="WordComplete"/> when all snap points are filled in.
        /// </summary>
        private void CheckWordComplete()
        {
            LettersMoved++;
            char[] wordResult = new char[_currentLetters.Length];
            int spaces = 0;

            for (int i = 0; i < wordResult.Length; i++)
            {
                if (_currentLetters[i] == ' ')
                {
                    spaces++;
                    wordResult[i] = ' ';
                    continue;
                }
                else if (_snapPoints[i - spaces].CurrentLetter == null)
                {
                    return;
                }

                wordResult[i] = _snapPoints[i - spaces].CurrentLetter.Letter;
            }

            WordComplete?.Invoke(new string(wordResult));
        }
    }
}