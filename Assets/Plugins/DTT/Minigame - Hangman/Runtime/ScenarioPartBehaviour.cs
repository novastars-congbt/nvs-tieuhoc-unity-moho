using UnityEngine;

namespace DTT.Hangman
{
    /// <summary>
    /// Represents the behaviour, that is part of a larger scenario.
    /// </summary>
    public abstract class ScenarioPartBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Whether this part has been unlocked.
        /// </summary>
        public abstract bool Unlocked { get; }
        
        /// <summary>
        /// Whether this part is currently unlocking.
        /// </summary>
        public abstract bool Unlocking { get; }

        /// <summary>
        /// Unlocks this part.
        /// </summary>
        /// <param name="service">The hangman service usable for the unlocking.</param>
        public abstract void Unlock(HangmanService service);

        /// <summary>
        /// Locks this part.
        /// </summary>
        public abstract void Lock();
    }
}
