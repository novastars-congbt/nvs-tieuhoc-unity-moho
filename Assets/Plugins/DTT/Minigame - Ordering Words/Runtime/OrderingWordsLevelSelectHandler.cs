using System.Collections.Generic;
using DTT.MinigameBase.LevelSelect;
using UnityEngine;

namespace DTT.OrderingWords
{
    /// <summary>
    /// The level select handler implementation for supporting the Level Select from Minigame base.
    /// </summary>
    public class OrderingWordsLevelSelectHandler : LevelSelectHandler<GameSettings, OrderingWordsResult, GameManager>
    {
        /// <summary>
        /// The configuration to use.
        /// </summary>
        [SerializeField]
        private List<GameSettings> _levels;
        
        /// <summary>
        /// The configuration that should be used for the level.
        /// </summary>
        /// <param name="levelNumber">The number of the level.</param>
        /// <returns>The configuration that is relevant for the given level.</returns>
        protected override GameSettings GetConfig(int levelNumber) => _levels[ (levelNumber-1) % _levels.Count];

        /// <summary>
        /// Calculates the game score based on the amount of tries the player used.
        /// </summary>
        /// <param name="result">The result of the players' performance.</param>
        /// <returns>A score value between 0 and 1.</returns>
        protected override float CalculateScore(OrderingWordsResult result) => Mathf.Ceil(Mathf.InverseLerp(30, 0, result.timeTaken) * 3) / 3.0f;
    }
}