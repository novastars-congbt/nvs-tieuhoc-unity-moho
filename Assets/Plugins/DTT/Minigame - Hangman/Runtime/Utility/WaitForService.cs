using UnityEngine;

namespace DTT.Hangman
{
    /// <summary>
    /// Represents a custom yield instruction that waits for the hangman
    /// game service to be active.
    /// </summary>
    public class WaitForService : CustomYieldInstruction
    {
        /// <summary>
        /// The hangman service to wait for.
        /// </summary>
        private readonly HangmanService _service;

        /// <summary>
        /// Creates a new instance of the yield instruction setting the service reference.
        /// </summary>
        /// <param name="service">The hangman service reference.</param>
        public WaitForService(HangmanService service) => _service = service;

        /// <summary>
        /// Whether the service is still paused.
        /// </summary>
        public override bool keepWaiting => !_service.IsGameActive;
    }
}
