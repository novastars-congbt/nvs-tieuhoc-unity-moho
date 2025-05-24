using System.Collections.Generic;
using UnityEngine;

namespace DTT.OrderingWords.Demo
{
    ///<summary>
    /// Holds and plays audio assets.
    ///</summary>
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// The different audio objects that can be played.
        /// </summary>
        public enum GameSfx
        {
            /// <summary>
            /// Audio asset for when a UI button is pressed.
            /// </summary>
            [InspectorName("Button Click")]
            UI_BUTTON_CLICK = 0,

            /// <summary>
            /// Audio asset for when an incorrect answer is selected.
            /// </summary>
            [InspectorName("Negative Feedback")]
            NEGATIVE_FEEDBACK = 1,

            /// <summary>
            /// Audio asset for when a correct answer is selected.
            /// </summary>
            [InspectorName("Positive Feedback")]
            POSITIVE_FEEDBACK = 2,

            /// <summary>
            /// Audio asset for when an item is clicked.
            /// </summary>
            [InspectorName("Item click")]
            ITEM_CLICK = 3,
        }

        /// <summary>
        /// The audio source used to play the clips.
        /// </summary>
        [SerializeField]
        [Tooltip("The audio source used to play")]
        private AudioSource _audioSource;

        /// <summary>
        /// This list contains all the clips we will play.
        /// </summary>
        [SerializeField]
        [Tooltip("The audio clips we wish to play in app")]
        private List<AudioAsset> _clips;

        /// <summary>
        /// A set of dictionaries allowing quick look up of the correct clip by its enum.
        /// </summary>
        private readonly Dictionary<GameSfx, AudioAsset> _sfxClipPairs = new Dictionary<GameSfx, AudioAsset>();

        /// <summary>
        /// Creates the dictionary to play audio through.
        /// </summary>
        private void Awake() => PopulateGameClips();

        /// <summary>
        /// Plays an audio clip.
        /// </summary>
        /// <param name="clip">The clip to play.</param>
        public void PlayAudioClip(GameSfx clip) => _audioSource.PlayOneShot(_sfxClipPairs[clip].AudioClip, _sfxClipPairs[clip].Volume);

        /// <summary>
        /// Creates dictionary entries for quick lookup of sound effects by enum.
        /// </summary>
        private void PopulateGameClips()
        {
            _sfxClipPairs.Clear();
            for (int i = 0; i < _clips.Count; i++)
                _sfxClipPairs.Add((GameSfx)i, _clips[i]);
        }
    }
}
