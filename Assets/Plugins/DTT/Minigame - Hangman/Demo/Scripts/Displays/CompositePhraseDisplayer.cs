using UnityEngine;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// A composite phrase displayer used for the demo scene to display the phrase parts
    /// separately to form a sentence.
    /// </summary>
    public class CompositePhraseDisplayer : PhraseDisplayer
    {
        /// <summary>
        /// The word prefab.
        /// </summary>
        public GameObject WordPrefab { get; set; }
        
        /// <summary>
        /// The phrase displayers used to form the sentence.
        /// </summary>
        private PhraseDisplayer[] _displayers;

        /// <summary>
        /// Regenerates the phrase section.
        /// </summary>
        /// <param name="service">The game service.</param>
        /// <param name="list">The list the phrase is chosen from.</param>
        /// <param name="phrase">The phrase for the game.</param>
        public override void Generate(HangmanService service, HangmanSettings.PhraseList list, Phrase phrase)
        {
            Phrase[] words = phrase.Split();
            _displayers = new PhraseDisplayer[words.Length];

            for (int i = 0; i < _displayers.Length; i++)
            {
                PhraseDisplayer displayer = Instantiate(WordPrefab, transform).GetComponent<PhraseDisplayer>();
                displayer.Generate(service, list, words[i]);
                
                _displayers[i] = displayer;
            }
        }

        /// <summary>
        /// Returns whether the full phrase is displayed.
        /// </summary>
        /// <returns>Whether the full phrase is displayed.</returns>
        public override bool IsFullPhraseDisplayed()
        {
            for(int i = 0; i < _displayers.Length; i++)
                if (!_displayers[i].IsFullPhraseDisplayed())
                    return false;

            return true;
        }

        /// <summary>
        /// Sets the letter value on all phrase displayers.
        /// </summary>
        /// <param name="letter">The letter to set.</param>
        public override void SetLetter(char letter)
        {
            for (int i = 0; i < _displayers.Length; i++)
                _displayers[i].SetLetter(letter);
        }
    }
}