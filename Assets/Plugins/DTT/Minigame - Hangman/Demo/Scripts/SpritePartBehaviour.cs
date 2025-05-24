using UnityEngine;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// Represents a scenario part displaying a sprite which can be scaled out to unlock itself.
    /// </summary>
    public class SpritePartBehaviour : ScenarioPartBehaviour
    {
        /// <summary>
        /// The scaler of the sprite.
        /// </summary>
        [SerializeField]
        private ScaleOutDirectional _scaler;

        /// <summary>
        /// The animate time.
        /// </summary>
        [SerializeField]
        private float _animateTime = 1.0f;
        
        /// <summary>
        /// Whether the sprite is fully displayed.
        /// </summary>
        public override bool Unlocked => _scaler.Completed;

        /// <summary>
        /// Whether the sprite is in the process of being displayed.
        /// </summary>
        public override bool Unlocking => !_scaler.Completed && _scaler.Animating;

        /// <summary>
        /// Unlocks this part starting the process of the sprite being displayed.
        /// </summary>
        /// <param name="service">The game service.</param>
        public override void Unlock(HangmanService service) => _scaler.StartScaling(_animateTime, service);

        /// <summary>
        /// Locks the part by resetting the scaler.
        /// </summary>
        public override void Lock() => _scaler.Reset();
    }
}


