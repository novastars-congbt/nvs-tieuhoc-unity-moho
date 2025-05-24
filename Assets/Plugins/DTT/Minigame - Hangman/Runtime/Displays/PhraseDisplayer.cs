using UnityEngine;

namespace DTT.Hangman
{
    /// <summary>
    /// A representation of a behaviour that displays a phrase.
    /// </summary>
    public class PhraseDisplayer : MonoBehaviour, IGameplaySectionController
    {
        /// <summary>
        /// The prefab of the letter game object.
        /// </summary>
        [SerializeField]
        private GameObject _letterPrefab;

        /// <summary>
        /// The prefab of the letter game object.
        /// </summary>
        public GameObject LetterPrefab => _letterPrefab;

        /// <summary>
        /// The letter display instances in the scene.
        /// </summary>
        public LetterDisplayer[] LetterDisplayers { get; private set; }

        /// <summary>
        /// Returns the transform of the parent transform for the letters in the scene.
        /// </summary>
        /// <returns>The parent transform.</returns>
        public virtual Transform GetLetterParentInScene() => transform;

        /// <summary>
        /// Generates the phrase display.
        /// </summary>
        /// <param name="service">The game service.</param>
        /// <param name="list">The list the phrase is chosen from.</param>
        /// <param name="phrase">The phrase for the game.</param>
        public virtual void Generate(HangmanService service, HangmanSettings.PhraseList list, Phrase phrase)
        {
            LetterDisplayers = new LetterDisplayer[phrase.Length];

            Transform parent = GetLetterParentInScene();
            for (int i = 0; i < LetterDisplayers.Length; i++)
            {
                GameObject instance = Instantiate(_letterPrefab, parent);
                LetterDisplayer displayer = instance.GetComponent<LetterDisplayer>();
                
                displayer.Setup(phrase[i]);
                
                // Set the letter value if we are showing vowels and the letter is a vowel.
                if(phrase.Exposes(phrase[i]))
                    displayer.SetLetterValue();

                LetterDisplayers[i] = displayer;
            }
        }

        /// <summary>
        /// Clears the display.
        /// </summary>
        public virtual void Clear()
        {
            for(int i = 0; i < LetterDisplayers.Length; i++)
                LetterDisplayers[i].Clear();
        }

        /// <summary>
        /// Returns whether the full phrase is being displayed.
        /// </summary>
        /// <returns>Whether the full phrase is being displayed.</returns>
        public virtual bool IsFullPhraseDisplayed()
        {
            for(int i = 0; i < LetterDisplayers.Length; i++)
                if (!LetterDisplayers[i].IsSet)
                    return false;

            return true;
        }

        /// <summary>
        /// Sets a given letter to be displayed in the phrase.
        /// </summary>
        /// <param name="letter">The letter to be displayed in the phrase.</param>
        public virtual void SetLetter(char letter)
        {
            for(int i = 0; i < LetterDisplayers.Length; i++)
                if(LetterDisplayers[i].Letter == letter)
                    LetterDisplayers[i].SetLetterValue();
        }
    }
}
