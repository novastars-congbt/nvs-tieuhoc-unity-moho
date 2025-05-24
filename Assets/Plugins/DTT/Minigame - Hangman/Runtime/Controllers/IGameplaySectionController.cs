namespace DTT.Hangman
{
    /// <summary>
    /// Provides interaction with an object that is part of the gameplay of a hangman game.
    /// </summary>
    public interface IGameplaySectionController
    {
        /// <summary>
        /// Generates the section contents.
        /// </summary>
        /// <param name="service">The game service.</param>
        /// <param name="list">The list the phrase is chosen from.</param>
        /// <param name="phrase">The phrase for the game.</param>
        void Generate(HangmanService service, HangmanSettings.PhraseList list, Phrase phrase);

        /// <summary>
        /// Clears the section contents.
        /// </summary>
        void Clear();
    }
}