using UnityEngine;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// A behaviour that can activate/deactivate
    /// the game object it is attached to.
    /// </summary>
    public class GameObjectActivator : MonoBehaviour
    {
        /// <summary>
        /// The game service.
        /// </summary>
        [SerializeField]
        private HangmanService _service;

        /// <summary>
        /// Starts listening for gameplay events.
        /// </summary>
        private void Awake()
        {
            _service.Started += () => SetActive(false);
            _service.Finish += (_) => SetActive(true);
        }
        
        /// <summary>
        /// Sets the active state of the game object this behaviour
        /// is attached to.
        /// </summary>
        /// <param name="value">The new active state value.</param>
        public void SetActive(bool value) => gameObject.SetActive(value);
    }
}
