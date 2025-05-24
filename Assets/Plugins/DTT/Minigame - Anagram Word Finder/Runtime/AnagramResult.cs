namespace DTT.MiniGame.Anagram
{
    /// <summary>
    /// Holds the result data from an anagram game.
    /// </summary>
    public struct AnagramResult
    {
        /// <summary>
        /// The completed word.
        /// </summary>
        public readonly string word;

        /// <summary>
        /// Letters moved until completion.
        /// </summary>
        public readonly int lettersMoved;

        /// <summary>
        /// Seconds it took to complete the game.
        /// </summary>
        public readonly int seconds;

        /// <summary>
        /// Constructs the anagram result.
        /// </summary>
        /// <param name="correctWord">Whether the word was correct.</param>
        /// <param name="word">The completed word.</param>
        public AnagramResult(string word, int lettersMoved, int seconds)
        {
            this.word = word;
            this.lettersMoved = lettersMoved;
            this.seconds = seconds;
        }
    }
}