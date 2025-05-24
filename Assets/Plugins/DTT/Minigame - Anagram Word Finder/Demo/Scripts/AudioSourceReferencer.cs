using UnityEngine;

namespace DTT.MiniGame.Anagram.Demo
{
    /// <summary>
    /// Base class for getting an audio source from an object.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public abstract class AudioSourceReferencer : MonoBehaviour
    {
        /// <summary>
        /// Audio source of the component.
        /// </summary>
        protected AudioSource AudioSource { get; private set; }

        /// <summary>
        /// Gets the audio component.
        /// </summary>
        protected virtual void Awake() => AudioSource = GetComponent<AudioSource>();
    }
}