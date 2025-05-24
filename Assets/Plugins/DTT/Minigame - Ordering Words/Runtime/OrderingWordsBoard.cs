using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.OrderingWords
{
    /// <summary>
    /// Class that manages the board of the game (sentence and draggable words). Handles creation/clearing of the level exercise.
    /// </summary>
    public class OrderingWordsBoard : MonoBehaviour
    {
        /// <summary>
        /// The word panel that words move to when dropped but not snapped.
        /// </summary>
        [SerializeField]
        [Tooltip("The panel words move to when dropped but not snapped")]
        private WordPanel _wordPanel;

        /// <summary>
        /// The text component prefab for displaying part of a sentence.
        /// </summary>
        [SerializeField]
        [Tooltip("The text component prefab for displaying part of a sentence")]
        private Text _textPrefab;

        /// <summary>
        /// The prefab for a blank space in a sentence.
        /// </summary>
        [SerializeField]
        [Tooltip("The prefab for a blank space in a sentence")]
        private SentenceSolverSnappingPoint _blankSpacePrefab;

        /// <summary>
        /// The panel the sentence is displayed in.
        /// </summary>
        [SerializeField]
        [Tooltip("The panel the sentence is displayed in")]
        private CanvasGroup _sentencePanel;

        /// <summary>
        /// Event for when the correct word answers are given.
        /// </summary>
        public event Action CorrectAnswerGiven;

        /// <summary>
        /// Event for when an incorrect word answer is given.
        /// </summary>
        public event Action IncorrectAnswerGiven;

        /// <summary>
        /// Color for the incorrect words animation.
        /// </summary>
        public Color IncorrectColor { set => _incorrectColor = value; }

        /// <summary>
        /// The word panel that words move to when dropped but not snapped.
        /// </summary>
        public WordPanel WordPanel => _wordPanel;

        /// <summary>
        /// All blank spaces from left to right.
        /// </summary>
        private List<SentenceSolverSnappingPoint> _blankSpaces = new List<SentenceSolverSnappingPoint>();

        private List<string> _correctWord = new List<string>();

        /// <summary>
        /// Color for the incorrect words animation.
        /// </summary>
        private Color _incorrectColor;

        int fontSize;

        /// <summary>
        /// On enable subscribes to events.
        /// </summary>
        private void OnEnable() => _wordPanel.WordSnap += OnSnap;

        /// <summary>
        /// On disable unsubscribes to events.
        /// </summary>
        private void OnDisable() => _wordPanel.WordSnap -= OnSnap;

        /// <summary>
        /// Set the currently displayed sentence.
        /// </summary>
        /// <param name="sentence">The correct sentence</param>
        /// <param name="draggableWords">All draggable words in the exercise</param>
        public void SetSentence(string sentence, List<string> draggableWords, List<Color> possibleWordColor)
        {
            _wordPanel.CreateWords(draggableWords, possibleWordColor);
            List<string> unassignedDraggables = new List<string>(draggableWords);

            List<string> sentenceParts = new List<string>();
            int startIndex = 0;
            int currentIndex = 0;

            while (currentIndex <= sentence.Length)
            {
                bool foundMatch = false;
                if (currentIndex == sentence.Length)
                {
                    if (startIndex < currentIndex)
                    {
                        sentenceParts.Add(sentence.Substring(startIndex, currentIndex - startIndex).Trim());
                    }
                    break;
                }

                foreach (string draggable in unassignedDraggables)
                {
                    if (currentIndex + draggable.Length <= sentence.Length &&
                        sentence.Substring(currentIndex, draggable.Length) == draggable)
                    {
                        if (startIndex < currentIndex)
                        {
                            sentenceParts.Add(sentence.Substring(startIndex, currentIndex - startIndex).Trim());
                        }
                        sentenceParts.Add(draggable);
                        currentIndex += draggable.Length;
                        startIndex = currentIndex;
                        foundMatch = true;
                        break;
                    }
                }

                if (!foundMatch)
                {
                    currentIndex++;
                }
            }

            StringBuilder currentPart = new StringBuilder();
            for (int i = 0; i < sentenceParts.Count; ++i)
            {
                string currentWord = sentenceParts[i].Trim();

                if (!unassignedDraggables.Contains(currentWord))
                {
                    currentPart.Append(currentWord + " ");
                    if (i == sentenceParts.Count - 1 && currentPart.Length > 0)
                    {
                        InstantiateSentencePart(currentPart.ToString());
                        currentPart.Clear();
                    }
                }
                else
                {
                    if (currentPart.Length > 0)
                        InstantiateSentencePart(currentPart.ToString());

                    currentPart.Clear();
                    InstantiateBlankSpace(currentWord);
                    unassignedDraggables.Remove(currentWord);
                }
            }
        }

        /// <summary>
        /// Disables or enables all draggable items.
        /// </summary>
        /// <param name="active">The enable or disable user input</param>
        /// <param name="finalTurnOff">Wether or not the word's raycast can be turned on again</param>
        public void SetUserInput(bool active, bool finalTurnOff)
        {
            for (int i = 0; i < _wordPanel.Words.Count; ++i)
                _wordPanel.Words[i].GetComponent<Word>().SetRayCast(active, finalTurnOff);
        }

        /// <summary>
        /// Clear the scene of runtime instantiated items.
        /// </summary>
        public void Clear()
        {
            _wordPanel.ClearWords();
            ClearSentence();
            Debug.LogError("Clear");
        }

        /// <summary>
        /// Instantiate a blank space in the <see cref="_sentencePanel"/>
        /// </summary>
        /// <param name="correctValue">The value that should be filled in the blank space to create a correct sentence</param>
        private void InstantiateBlankSpace(string correctValue)
        {
            SentenceSolverSnappingPoint blankSpace = Instantiate(_blankSpacePrefab, _sentencePanel.transform);
            blankSpace.Init(correctValue, OnRemoveSnappedItem);
            _blankSpaces.Add(blankSpace);
            _correctWord.Add(correctValue);
        }

        /// <summary>
        /// Instantiate part of the sentence for the exercise.
        /// </summary>
        /// <param name="sentencePart">The part to instantiate</param>
        private void InstantiateSentencePart(string sentencePart)
        {
            Text text = Instantiate(_textPrefab, _sentencePanel.transform);
            text.text = sentencePart;
            fontSize = text.fontSize;
        }

        /// <summary>
        /// Method called when any of the <see cref="_wordPanel.Words"/> should be removed from the snapping point.
        /// </summary>
        /// <param name="removeItem">The item to remove from their snapping point</param>
        private void OnRemoveSnappedItem(Word removeItem)
        {
            Word newOccupier = ((SentenceSolverSnappingPoint)removeItem.SnappingPoint).SnappedWord;
            if (newOccupier.LastSnappingPoint != null)
            {
                removeItem.SetSnappingPoint(newOccupier.LastSnappingPoint);
                if (removeItem.gameObject.activeSelf) removeItem.GetComponent<WordAnimations>().MoveAnimation(removeItem.SnappingPoint.transform.position);
            }
            else
            {
                removeItem.SetSnappingPoint(null);
                _wordPanel.AddItem(removeItem.gameObject);
            }
            removeItem.LastSnappingPoint = removeItem.SnappingPoint;
        }

        /// <summary>
        /// Check whether all blankspaces are filled in correctly.
        /// </summary>
        private void OnSnap()
        {
            if (_blankSpaces.TrueForAll(x => x.IsCorrect))
                AnimateCorrectAnswer();
            else if (_blankSpaces.TrueForAll(x => x.SnappedWord != null))
                AnimateIncorrectAnswer();
        }

        /// <summary>
        /// Shows the user the answer was incorrect.
        /// </summary>
        private void AnimateIncorrectAnswer()
        {
            for (int i = 0; i < _wordPanel.Words.Count; ++i)
            {
                if(_wordPanel.Words[i].GetComponent<Word>().SnappingPoint == null)
                    continue;

                else
                {
                    Word word = _wordPanel.Words[i].GetComponent<Word>();
                    word.transform.SetParent(_wordPanel.transform);

                    StartCoroutine(
                    word.GetComponent<WordAnimations>().ColorFlashingAnimation(_incorrectColor, () =>
                        {
                            word.BackgroundColor = word.DefaultColor;
                            ((SentenceSolverSnappingPoint)word.SnappingPoint).SetSnappedWord(null);
                            word.SetSnappingPoint(null);
                            _wordPanel.AddItem(word.gameObject);
                        }));
                }
            }
            IncorrectAnswerGiven?.Invoke();
        }

        /// <summary>
        /// Shows the user the answer was correct.
        /// </summary>
        private void AnimateCorrectAnswer()
        {
            for (int i = 0; i < _blankSpaces.Count; ++i)
            {
                // Capture iteration variable.
                int _i = i;
                
                SentenceSolverSnappingPoint blankSpace = _blankSpaces[i];
                if (!blankSpace.IsCorrect)
                    return;
                blankSpace.SnappedWord.SetRayCast(false, true);
                blankSpace.SnappedWord.GetComponent<WordAnimations>().TransparencyAnimation(1,0,() =>
                    {
                        _wordPanel.Words.Remove(blankSpace.SnappedWord.gameObject);
                        Destroy(blankSpace.SnappedWord.gameObject);
                        blankSpace.SetText(fontSize);
                        
                        // This makes sure this event is only called once.
                        if(_i == 0)
                            CorrectAnswerGiven?.Invoke();
                    });
                StartCoroutine(Animations.Value(0,1, blankSpace.SnappedWord.GetComponent<WordAnimations>().ColorFadingAnimationTime, (value) => blankSpace.SetTextTransparency(value)));
            }
        }

        /// <summary>
        /// Clear the GameObjects in the sentence panel.
        /// </summary>
        private void ClearSentence()
        {
            for (int i = 0; i < _blankSpaces.Count; ++i)
                Destroy(_blankSpaces[i].gameObject);

            _blankSpaces.Clear();
            _correctWord.Clear();

            for (int i = 0; i < _sentencePanel.transform.childCount; ++i)
                Destroy(_sentencePanel.transform.GetChild(i).gameObject);
        }
    }
}