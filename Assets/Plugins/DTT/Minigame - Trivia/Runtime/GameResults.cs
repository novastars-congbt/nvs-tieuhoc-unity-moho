using System;
using System.Text;

namespace DTT.Trivia
{
    /// <summary>
    /// Handles the results of the game.
    /// </summary>
    public class GameResults
    {
        /// <summary>
        /// Time taken to finish a level.
        /// </summary>
        public float TimeTaken { get; private set; }

        /// <summary>
        /// Amount of wrong guesses.
        /// </summary>
        public float WrongGuesses { get; private set; }

        /// <summary>
        /// Returns result info in string format.
        /// </summary>
        /// <returns>Result in string format</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('\n');
            sb.Append("Time: ");

            string format = @"mm\:ss";
            sb.Append(TimeSpan.FromSeconds(TimeTaken).ToString(format));

            sb.Append('\n');
            sb.Append('\n');
            sb.Append("Wrong Guesses: ");
            sb.Append(WrongGuesses);

            return sb.ToString();
        }

        /// <summary>
        /// Sets the wrong guesses.
        /// </summary>
        /// <param name="amount">Amount to set.</param>
        internal void SetWrongGuesses(int amount) => WrongGuesses = amount;

        /// <summary>
        /// Increments the worng guesses by 1.
        /// </summary>
        internal void IncrementWrongGuess() => WrongGuesses++;

        /// <summary>
        /// Sets the time taken to finish a game.
        /// </summary>
        /// <param name="timeTaken">The time to set.</param>
        internal void SetTimeTaken(float timeTaken) => TimeTaken = timeTaken;
    }
}