using DTT.MinigameBase.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// A behaviour controlling the letters in the demo scene.
    /// </summary>
    public class DemoLetterController : LetterSectionController
    {
        /// <summary>
        /// Represents a letter to displayed in the scene.
        /// </summary>
        private struct Letter
        {
            /// <summary>
            /// The prefab used by the letters.
            /// </summary>
            public static GameObject prefab;

            /// <summary>
            /// The parent used by the letters.
            /// </summary>
            public static Transform parent;

            /// <summary>
            /// A callback for the letter being clicked.
            /// </summary>
            private Action<char, Vector3> _clickedCallback;

            /// <summary>
            /// The letter game object instance.
            /// </summary>
            private GameObject _instance;

            /// <summary>
            /// The letter character value.
            /// </summary>
            private char _value;

            /// <summary>
            /// Creates a new instance of the letter.
            /// </summary>
            /// <param name="value">The letter value.</param>
            /// <param name="clickedCallback">A callback for the letter being clicked.</param>
            /// <param name="removeOnClick">Whether the letter should disappear on click.</param>
            public Letter(char value, Action<char, Vector3> clickedCallback, bool removeOnClick)
            {
                _clickedCallback = clickedCallback;
                _value = value;

                _instance = Instantiate(prefab, parent);
                _instance.GetComponent<SelectableLetterDisplayer>().Setup(value, OnLetterClick, removeOnClick);
            }

            /// <summary>
            /// Destroys the letter.
            /// </summary>
            public void Destroy() => GameObject.Destroy(_instance);

            /// <summary>
            /// Called when the letter is being clicked to Invoke the callback and
            /// destroy the letter.
            /// </summary>
            private void OnLetterClick(Vector3 pos)
            {
                _clickedCallback.Invoke(_value, pos);
                
            }

        }

        /// <summary>
        /// The letters being displayed for the user to be clicked.
        /// </summary>
        private Letter[] _letters;

        /// <summary>
        /// Sets the static letter references.
        /// </summary>
        private void Awake()
        {
            // Set shared values between letters.
            Letter.prefab = LetterPrefab;
            Letter.parent = transform;
        }

        /// <summary>
        /// Called when the letters should be regenerated, it settings up
        /// the letters based on the game settings and letter characters.
        /// </summary>
        /// <param name="settings">The game settings.</param>
        /// <param name="letters">The letters to choose from.</param>
        protected override void OnGenerateLetters(HangmanSettings settings, char[] letters)
        {
            _letters = new Letter[letters.Length];
            for (int i = 0; i < letters.Length; i++)
            {
                char characterValue = settings.Casing.ApplyTo(letters[i]);
                _letters[i] = new Letter(characterValue, OnLetterClicked, settings.RemoveLettersOnSelected);
            }
        }

        /// <summary>
        /// Destroys old letters on the field.
        /// </summary>
        public override void Clear()
        {
            if (_letters == null)
                return;

            for (int i = 0; i < _letters.Length; i++)
                _letters[i].Destroy();

            _letters = null;
        }
    }
}


