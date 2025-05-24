using UnityEngine;
using UnityEngine.Events;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// A simple behaviour that invokes an event on start.
    /// </summary>
    public class StartEventInvoker : MonoBehaviour
    {
        /// <summary>
        /// The event to invoke.
        /// </summary>
        [SerializeField]
        private UnityEvent _event;
        
        /// <summary>
        /// Invokes the serialized unity event.
        /// </summary>
        private void Start() => _event?.Invoke();
    }
}
