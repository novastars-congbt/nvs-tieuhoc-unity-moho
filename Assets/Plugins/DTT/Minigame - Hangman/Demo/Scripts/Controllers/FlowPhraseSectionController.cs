using DTT.MinigameBase.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// A behaviour controlling the phrase displayed in the demo scene. It displays the words in a flow layout like manner.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class FlowPhraseSectionController : PhraseSectionController
    {
        /// <summary>
        /// The prefab that will display a letter. The prefab should have a LetterDisplayer component.
        /// </summary>
        [SerializeField]
        private GameObject _letterPrefab;

        /// <summary>
        /// The text renderer displaying the header of the theme.
        /// </summary>
        [SerializeField]
        private Text _themeHeader;

        /// <summary>
        /// The padding between letters on the X axis.
        /// </summary>
        [SerializeField]
        private float _paddingX;

        /// <summary>
        /// The padding between letters on the Y axis.
        /// </summary>
        [SerializeField]
        private float _paddingY;

        /// <summary>
        /// The space between separate words.
        /// </summary>
        [SerializeField]
        private float _spaceBetweenWords = 10;

        /// <summary>
        /// Whether the letters should be resized to better fit the bounds.
        /// Turns this off when you want to allow the letters to overflow outside the bounds.
        /// </summary>
        [SerializeField]
        private bool _resizeOverflow = true;

        /// <summary>
        /// The <see cref="RectTransform"/> of this object.
        /// </summary>
        [SerializeField]
        private RectTransform _rectTransform;

        /// <summary>
        /// List of all current displayed letters.
        /// </summary>
        private List<LetterDisplayer> _letterDisplayers = new List<LetterDisplayer>();

        /// <summary>
        /// The letter controller.
        /// </summary>
        [SerializeField]
        private DemoLetterController _letterController;

        private string strTheme;



        /// <summary>
        /// Casts the transform of this object to a <see cref="RectTransform"/>.
        /// </summary>
        //private void Awake() => _rectTransform = (RectTransform)transform;

        //private void Awake()
        //{
        //    Debug.LogError("============== co awake");
        //    MiniGameEndController.actionNext += ActionNext;
        //    MiniGameEndController.actionRestart += ActionRestart;
        //}

        /// <summary>
        /// Adds listener to the letter clicked event.
        /// </summary>
        private void OnEnable()
        {
            _letterController.LetterClicked += OnLetterClicked;
            if (MiniGameEndController.instance != null)
            {
                //MiniGameEndController.instance.actionNextGame += ActionNext;
                //MiniGameEndController.instance.actionRestartGame += ActionRestart;
            }
            gameUI.actionNext += ActionNext;
            gameUI.actionRestart += ActionRestart;
            Debug.LogError("============== enable flow phrase");
        }

        /// <summary>
        /// Removes listener from the letter clicked event.
        /// </summary>
        private void OnDisable()
        {
            _letterController.LetterClicked -= OnLetterClicked;
            if (MiniGameEndController.instance != null)
            {
                //MiniGameEndController.instance.actionNextGame -= ActionNext;
                //MiniGameEndController.instance.actionRestartGame -= ActionRestart;
            }
            gameUI.actionNext -= ActionNext;
            gameUI.actionRestart -= ActionRestart;
            Debug.LogError("============== disable flow phrase");
        }

        /// <summary>
        /// Creates the necessary letter prefabs for the given phrase.
        /// </summary>
        /// <param name="phrase">Phrase to be displayed.</param>
        public void CreateLayout(Phrase phrase)
        {
            Phrase[] words = phrase.Split();

            // Creates the letter objects.
            for (int i = 0; i < words.Length; i++)
            {
                for (int j = 0; j < words[i].Length; j++)
                {
                    RectTransform newLetter = (RectTransform)Instantiate(_letterPrefab, _rectTransform).transform;
                    LetterDisplayer displayer = newLetter.GetComponent<LetterDisplayer>();
                    Phrase currentPhrase = words[i];
                    displayer.Setup(currentPhrase[j]);

                    // Set the letter value if we are showing vowels and the letter is a vowel.
                    if (phrase.Exposes(currentPhrase[j]))
                        displayer.SetLetterValue();

                    _letterDisplayers.Add(displayer);
                }
            }

            RectTransform wordPrefabRect = (RectTransform)_letterPrefab.transform;

            OrganizeLayout(words, wordPrefabRect.rect.width, wordPrefabRect.rect.height);
        }

        /// <summary>
        /// Organizes the layout based on a flow layout. When the content doesn't fit the bounds,
        /// it resizes all the letters to fit and reorganizes the letters.
        /// </summary>
        /// <param name="words">All words in the phrase.</param>
        /// <param name="letterWidth">Width of a letter.</param>
        /// <param name="letterHeight">Height of a letter.</param>
        private void OrganizeLayout(Phrase[] words, float letterWidth, float letterHeight)
        {
            float boundsWidth = _rectTransform.rect.width;
            float boundsHeight = _rectTransform.rect.height;

            float paddedWidth = letterWidth + _paddingX;
            float paddedHeight = letterHeight + _paddingY;

            float currentWidth = 0;
            float currentHeight = boundsHeight;

            float widthOverflow = 0;

            int lettersChecked = 0;

            for (int i = 0; i < words.Length; i++)
            {
                // Checks the length of the current word.
                float wordLength = paddedWidth * words[i].Length + _spaceBetweenWords;
                if (currentWidth + wordLength > boundsWidth)
                {
                    if (wordLength > boundsWidth && currentWidth == 0)
                    {
                        // When a word is wider than the width of the bounding box, it saves the amount it overflowed.
                        if (_resizeOverflow)
                        {
                            float overflow = wordLength - boundsWidth;
                            if (widthOverflow < overflow) widthOverflow = overflow;
                        }
                        else
                        {
                            Debug.LogError("Word length exceeds the bounds of the given box. Make the letter prefab smaller, or increase the bounding box size.");
                        }
                    }
                    else
                    {
                        // Adds height to the offset.
                        currentHeight -= paddedHeight;
                        currentWidth = 0;
                    }
                }

                // Places the letters on the correct positions.
                for (int j = 0; j < words[i].Length; j++)
                {
                    RectTransform newLetter = (RectTransform)_letterDisplayers[lettersChecked].transform;
                    lettersChecked++;

                    newLetter.anchoredPosition = new Vector2(currentWidth + paddedWidth / 2, currentHeight - paddedHeight / 2);

                    currentWidth += paddedWidth;
                }

                currentWidth += _spaceBetweenWords;
            }

            // Checks how much the letters overflow from the bottom of the bounding box.
            float heightOverflow = -(currentHeight - paddedHeight);
            if (heightOverflow < 0)
                heightOverflow = 0;

            if (_resizeOverflow)
            {
                // Gets by how much the letter size must be multiplied to fit in the bounds.
                float overflowRatio;
                if (heightOverflow / boundsHeight > widthOverflow / boundsWidth)
                {
                    if (heightOverflow == 0)
                        return;

                    overflowRatio = boundsHeight / (heightOverflow + boundsHeight);
                }
                else
                {
                    if (widthOverflow == 0)
                        return;

                    overflowRatio = boundsWidth / (widthOverflow + boundsWidth);
                }

                float overflowWidth = letterWidth * overflowRatio;
                float overflowHeight = letterHeight * overflowRatio;

                // Scales the letters.
                foreach (LetterDisplayer letter in _letterDisplayers)
                {
                    RectTransform letterRect = (RectTransform)letter.transform;
                    letterRect.sizeDelta = new Vector2(overflowWidth, overflowHeight);
                }

                // After resizing, reorganizes the letters so they are properly placed withing the bounds.
                OrganizeLayout(words, overflowWidth, overflowHeight);
            }
        }

        /// <summary>
        /// Clears the layout of all letter objects.
        /// </summary>
        public override void Clear()
        {
            for (int i = 0; i < _rectTransform.childCount; i++)
                Destroy(_rectTransform.GetChild(i).gameObject);

            _letterDisplayers.Clear();
        }

        /// <summary>
        /// Checks whether the phrase had been completed.
        /// </summary>
        /// <returns>Whether the phrase is completed.</returns>
        protected override bool GetIsPhraseCompleted()
        {
            foreach (LetterDisplayer letter in _letterDisplayers)
                if (!letter.IsSet)
                    return false;

            return true;
        }

        /// <summary>
        /// Sets the header title and starts creating the layout.
        /// </summary>
        /// <param name="service">The hangman game service.</param>
        /// <param name="list">The list the phrase was chosen from.</param>
        /// <param name="phrase">The phrase to be displayed.</param>
        protected override void OnGeneratePhrase(HangmanService service, HangmanSettings.PhraseList list, Phrase phrase)
        {
            string str = GetTheme(service, list);
            if (str.Length > 0) strTheme = str;
            _themeHeader.text = strTheme;
            //_themeHeader.text = list.Theme;
            gameUI._popup._answerText.text = phrase.value;
            CreateLayout(phrase);
        }

        /// <summary>
        /// Handles displaying the clicked letter.
        /// </summary>
        /// <param name="letter">The clicked letter.</param>
        private void OnLetterClicked(char letter)
        {
            foreach (LetterDisplayer letterDisplayer in _letterDisplayers)
                if (letterDisplayer.Letter == letter)
                    letterDisplayer.SetLetterValue();
        }

        string GetTheme(HangmanService service, HangmanSettings.PhraseList list) {
            for (int i = service.index; i >= 0; i--) {
                if (list[i].theme.Length > 0) return list[i].theme;
            }
            return list.Theme;
        }
    }
}