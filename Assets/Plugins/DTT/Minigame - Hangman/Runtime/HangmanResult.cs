namespace DTT.Hangman
{
    /// <summary>
    /// Represents the result of a hangman game.
    /// </summary>
    public struct HangmanResult
    {
        /// <summary>
        /// Whether the phrase is finished.
        /// </summary>
        public bool finishedPhrase;

        /// <summary>
        /// The amount of wrong guesses by the user.
        /// </summary>
        public int wrongGuesses;
        
        /// <summary>
        /// The time taken for the game.
        /// </summary>
        public double timeTaken;
    }
}
