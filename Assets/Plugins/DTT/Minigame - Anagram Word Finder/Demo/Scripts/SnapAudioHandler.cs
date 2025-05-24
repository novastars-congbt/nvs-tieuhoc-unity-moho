using DTT.MiniGame.Anagram.UI;
using UnityEngine;

namespace DTT.MiniGame.Anagram.Demo
{
    /// <summary>
    /// Handles audio for the <see cref="AnagramSnapPoint"/> component.
    /// </summary>
    public class SnapAudioHandler : AudioSourceReferencer
    {
        /// <summary>
        /// Anagram snap point of this handler.
        /// </summary>
        [SerializeField]
        [Tooltip("Anagram snap point of this handler.")]
        private AnagramSnapPoint _snapPoint;

        /// <summary>
        /// Snap sound effect.
        /// </summary>
        [SerializeField]
        [Tooltip("Snap sound effect.")]
        private AudioClip _snapClip;

        /// <summary>
        /// Adds listeners.
        /// </summary>
        private void OnEnable() => _snapPoint.LetterSnapped += PlaySnap;

        /// <summary>
        /// Removes listeners.
        /// </summary>
        private void OnDisable() => _snapPoint.LetterSnapped -= PlaySnap;

        /// <summary>
        /// Plays snap sound effect.
        /// </summary>
        private void PlaySnap() => AudioSource.PlayOneShot(_snapClip);
    }
}