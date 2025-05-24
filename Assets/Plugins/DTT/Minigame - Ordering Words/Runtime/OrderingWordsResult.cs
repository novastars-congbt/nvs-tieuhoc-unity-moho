using System.Text;

namespace DTT.OrderingWords
{
    /// <summary>
    /// Class to contain the results of an ordering words minigame.
    /// </summary>
    public class OrderingWordsResult
    {
        /// <summary>
        /// Time it took to finish the game in seconds.
        /// </summary>
        public readonly float timeTaken;

        /// <summary>
        /// Amount of levels that were completed in the game.
        /// </summary>
        public readonly int amountOfLevelsCompleted;

        /// <summary>
        /// Sets the result information.
        /// </summary>
        /// <param name="timeTaken">Time the player took to finish the game in seconds.</param>
        /// <param name="amountOfRestarts">Amount of times the player has restarted a level.</param>
        /// <param name="amountOfLevelsCompleted">Amount levels completed by the player before the game finished.</param>
        public OrderingWordsResult(float timeTaken, int amountOfLevelsCompleted)
        {
            this.timeTaken = timeTaken;
            this.amountOfLevelsCompleted = amountOfLevelsCompleted;
        }

        /// <summary>
        /// Returns result info in string format for debugging.
        /// </summary>
        /// <returns>Result in string format</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Amount of wrong attempts: ");
            sb.Append(amountOfLevelsCompleted);
            sb.Append('\t');
            sb.Append("Time taken (s): ");
            sb.Append(timeTaken);
            return sb.ToString();
        }
    }
}