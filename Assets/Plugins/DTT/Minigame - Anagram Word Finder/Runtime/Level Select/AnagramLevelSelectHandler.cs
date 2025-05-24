using UnityEngine;
using DTT.MinigameBase.LevelSelect;
using System.Collections.Generic;
using System.Linq;

namespace DTT.MiniGame.Anagram
{
    /// <summary>
    /// Controls the level select flow, see <see cref="LevelSelectHandler{TConfig, TResult, TMinigame}"/>.
    /// </summary>
    public class AnagramLevelSelectHandler : LevelSelectHandler<AnagramConfig, AnagramResult, AnagramManager>
    {
        /// <summary>
        /// A reference to the config list that will be used to set up a level.
        /// </summary>
        [SerializeField]
        [Tooltip("A reference to the config list that will be used to set up a level.")]
        private List<AnagramConfig> _levelSettings;

        /// <summary>
        /// Calculates the score based on the results of the game.
        /// </summary>
        /// <param name="result">Game results.</param>
        /// <returns>Float from 0 to 1.</returns>
        protected override float CalculateScore(AnagramResult result)
        {
            int failedTries = result.lettersMoved - result.word.Length;
            float score = 0;

            // Adds attempt score.
            score += Mathf.Pow(.5f, 1 + failedTries / 3);
            
            // Adds time score.
            score += Mathf.Pow(.5f, 1 + result.seconds / 10);

            return score;
        }

        /// <summary>
        /// Retrieves the level config object for that specific level.
        /// </summary>
        /// <param name="levelNumber">The number of the level.</param>
        /// <returns>The game config for the level.</returns>
        protected override AnagramConfig GetConfig(int levelNumber)=>
            levelNumber > _levelSettings.Count? _levelSettings.Last() : _levelSettings[levelNumber-1];
    }
}