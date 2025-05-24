using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// A behaviour managing a hint button.
    /// </summary>
    public class HintButton : MonoBehaviour
    {
        /// <summary>
        /// The game service.
        /// </summary>
        [SerializeField]
        private HangmanService _service;

        /// <summary>
        /// The hint renderer.
        /// </summary>
        [SerializeField]
        private Text _hintRenderer;
        [SerializeField]
        private AudioSource _hintAudible;
        
        /// <summary>
        /// Whether the hint should be random. If set to false, the first hint will always be used.
        /// </summary>
        [SerializeField]
        private bool _randomHint = true;
        
        /// <summary>
        /// The button to be clicked.
        /// </summary>
        private Button _button;

        /// <summary>
        /// Starts listening to the button.
        /// </summary>
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }
        /// <summary>
        /// Called when the button is clicked, it will get the
        /// the phrase descriptions and display one as hint.
        /// </summary>
        private void OnClick()
        {
            string[] descriptions = _service.CurrentPhrase.descriptions;
            //AudioClip[] audible = _service.CurrentPhrase.audible;
            _hintRenderer.gameObject.SetActive(true);
            if (descriptions == null || descriptions.Length == 0)
            {
                _hintRenderer.text = "Không có gợi ý";
            }
            else
            {
                int r = Random.Range(0, descriptions.Length);
                string hint = _randomHint ? descriptions[r] : descriptions[0];
                _hintRenderer.text = $"{hint}";
                //if (r < audible.Length) _hintAudible.PlayOneShot(audible[r]);
            }
        }
    }
}
