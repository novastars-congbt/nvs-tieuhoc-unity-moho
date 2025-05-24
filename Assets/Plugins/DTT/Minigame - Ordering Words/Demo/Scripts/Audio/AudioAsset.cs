using UnityEngine;

namespace DTT.OrderingWords.Demo
{
    ///<summary>
    /// Holds audio files.
    ///</summary>
    [CreateAssetMenu(fileName = "AudioAsset", menuName = "DTT/MiniGame/OrderingWords/AudioAsset")]
    public class AudioAsset : ScriptableObject
    {
        /// <summary>
        /// Used for scaling audio down.
        /// </summary>
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("This can be used to turn down non-music audio if it's too loud")]
        private float _volume = 1f;

        /// <summary>
        /// The audio clip.
        /// </summary>
        [SerializeField]
        [Tooltip("The audio clip")]
        private AudioClip _audioClip;

        /// <summary>
        /// Get and Set for volume.
        /// </summary>
        public float Volume
        {
            get => _volume;
            set => _volume = Mathf.Clamp01(value); 
        }

        /// <summary>
        /// Getter for the audio clip.
        /// </summary>
        public AudioClip AudioClip => _audioClip;
    }
}
