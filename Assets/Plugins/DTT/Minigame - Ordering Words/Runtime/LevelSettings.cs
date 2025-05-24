/* using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace DTT.OrderingWords
{
    /// <summary>
    /// Holds the settings for the level of the game.
    /// </summary>
    [CreateAssetMenu(fileName = "Level_Settings_template", menuName = "DTT/MiniGame/OrderingWords/LevelSettings")]
    public class LevelSettings : ScriptableObject
    {
        /// <summary>
        /// The sentence for the level.
        /// </summary>
        [SerializeField]
        [Tooltip("The sentence for the level")]
        private string _sentence;

        /// <summary>
        /// The audio for the sentence.
        /// </summary>
        [SerializeField]
        [Tooltip("The audo for the sentence")]
        private AudioClip _audio;

        /// <summary>
        /// The draggable words for the level.
        /// </summary>
        [SerializeField]
        [Tooltip("The draggable words for the level")]
        private List<string> _draggableWords;

        /// <summary>
        /// The sentence for the level.
        /// </summary>
        public string Sentence => _sentence;
        public AudioClip Audio => _audio;

        /// <summary>
        /// The draggable words for the level.
        /// </summary>
        public ReadOnlyCollection<string> DraggableWords => _draggableWords.AsReadOnly();
    }
} */