using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.OrderingWords.Demo
{
    /// <summary>
    /// Handles the UI of the game.
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        /// <summary>
        /// Reference to the game manager of this scene.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the game manager of this scene")]
        private GameManager _orderingWordsManager;

        /// <summary>
        /// Reference to the game controller of this scene.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the game controller of this scene")]
        private OrderingWordsBoard _board;

        /// <summary>
        /// Reference to the audio manager of this scene.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the audio manager of this scene")]
        private AudioManager _audioManager;

        /// <summary>
        /// On enable subscribe to button events.
        /// </summary>
        private void OnEnable()
        {
            _orderingWordsManager.LevelEnded += OnLevelEnded;
            _board.IncorrectAnswerGiven += OnIncorrectAnswerGiven;
            _board.WordPanel.WordTap += OnWordTapped;
        }

        /// <summary>
        /// On disable unsubscribe to button events.
        /// </summary>
        private void OnDisable()
        {
            _orderingWordsManager.LevelEnded -= OnLevelEnded;
            _board.IncorrectAnswerGiven -= OnIncorrectAnswerGiven;
            _board.WordPanel.WordTap -= OnWordTapped;
        }

        /// <summary>
        /// When the level has ended.
        /// </summary>
        private void OnLevelEnded()
        {
            StartCoroutine(WaitForNextLevel());

            IEnumerator WaitForNextLevel()
            {
                yield return new WaitForSeconds(0.5f);
                _orderingWordsManager.NextLevel();
            }
        }

        /// <summary>
        /// Plays a sound clip when an incorrect answer is given.
        /// </summary>
        private void OnIncorrectAnswerGiven() => _audioManager.PlayAudioClip(AudioManager.GameSfx.NEGATIVE_FEEDBACK);

        /// <summary>
        /// Plays a sound clip when an incorrect answer is given.
        /// </summary>
        private void OnWordTapped() => _audioManager.PlayAudioClip(AudioManager.GameSfx.ITEM_CLICK);
    }
}
