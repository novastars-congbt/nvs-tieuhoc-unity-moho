using DTT.MiniGame.Anagram.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.MiniGame.Anagram.Demo
{
    /// <summary>
    /// Handles audio for an <see cref="AnagramLetter"/> component.
    /// </summary>
    public class LetterAudioHandler : AudioSourceReferencer
    {
        /// <summary>
        /// Anagram letter of this handler.
        /// </summary>
        [SerializeField]
        [Tooltip("Anagram letter of this handler.")]
        private AnagramLetter _letter;

        /// <summary>
        /// Pickup sound effect.
        /// </summary>
        [SerializeField]
        [Tooltip("Pickup sound effect.")]
        private AudioClip _pickUpClip;

        /// <summary>
        /// Drop sound effect.
        /// </summary>
        [SerializeField]
        [Tooltip("Drop sound effect.")]
        private AudioClip _dropClip;

        /// <summary>
        /// Adds listeners.
        /// </summary>
        private void OnEnable()
        {
            _letter.PickUp += PlayPickup;
            _letter.Drop += PlayDrop;
        }

        /// <summary>
        /// Removes listeners.
        /// </summary>
        private void OnDisable()
        {
            _letter.PickUp -= PlayPickup;
            _letter.Drop -= PlayDrop;
        }

        /// <summary>
        /// Plays pickup sound effect.
        /// </summary>
        /// <param name="letter">Picked up letter.</param>
        private void PlayPickup(AnagramLetter letter) => AudioSource.PlayOneShot(_pickUpClip);

        /// <summary>
        /// Plays drop sound effect.
        /// </summary>
        /// <param name="letter">Dropped letter.</param>
        private void PlayDrop(AnagramLetter letter) => AudioSource.PlayOneShot(_dropClip);
    }
}