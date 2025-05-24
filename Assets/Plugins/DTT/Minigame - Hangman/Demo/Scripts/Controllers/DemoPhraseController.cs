using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// A behaviour controlling the phrase displayed in the demo scene.
    /// </summary>
    public class DemoPhraseController : PhraseSectionController<PhraseDisplayer>
    {
        /// <summary>
        /// The prefab used for a word in a phrase.
        /// </summary>
        [SerializeField]
        private GameObject _wordPrefab;

        /// <summary>
        /// The prefab used for a word in a phrase.
        /// </summary>
        public GameObject WordPrefab => _wordPrefab;

        /// <summary>
        /// The text renderer displaying the header of the theme.
        /// </summary>
        [SerializeField]
        private Text _themeHeader;
        
        /// <summary>
        /// The letter controller.
        /// </summary>
        [SerializeField]
        private DemoLetterController _letterController;

        /// <summary>
        /// Starts listening for letters clicked by the user.
        /// </summary>
        private void Awake() => _letterController.LetterClicked += OnLetterClicked;

        /// <summary>
        /// Called when a letter has been clicked to try and display the letter.
        /// </summary>
        /// <param name="letter">The letter clicked.</param>
        private void OnLetterClicked(char letter) => p_displayer.SetLetter(letter);

        /// <summary>
        /// Creates the initial display used for the phrase.
        /// </summary>
        /// <param name="settings">The game settings.</param>
        /// <param name="phrase">The phrase used for the game.</param>
        /// <returns>The phrase displayer.</returns>
        protected override PhraseDisplayer CreatePhraseDisplay(HangmanSettings settings, Phrase phrase)
        {
            int wordCount = phrase.WordCount;  
            if (wordCount == 1)
                return Instantiate(WordPrefab, transform).GetComponent<PhraseDisplayer>();
            
            CompositePhraseDisplayer composite = gameObject.AddComponent<CompositePhraseDisplayer>();
            composite.WordPrefab = WordPrefab;

            // Start a coroutine to wait for one frame before rebuilding
            // the layout after the new displayer has been initialized.
            StartCoroutine(WaitForRebuild());

            return composite;
        }

        /// <summary>
        /// Sets the theme display.
        /// </summary>
        /// <param name="theme">The theme display.</param>
        protected override void CreateThemeDisplay(string theme) => _themeHeader.text = theme;

        /// <summary>
        /// Destroys the phrase displayer.
        /// </summary>
        public override void Clear()
        {
            if (p_displayer is CompositePhraseDisplayer)
            {
                // If the displayer is a composite, destroy the component and all child objects.
                Destroy(p_displayer);
                
                for(int i = 0; i < transform.childCount; i++)
                    Destroy(transform.GetChild(i).gameObject);
            }
            else
            {
                // If the displayer is not a composite, destroy the displayer game object.
                Destroy(p_displayer.gameObject);
            }
        }

        /// <summary>
        /// Waits for one frame before forcing a rebuild on this game objects rectangle transform.
        /// </summary>
        /// <returns>Waits for a frame and rebuilds the transform.</returns>
        private IEnumerator WaitForRebuild()
        {
            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }
    }
}