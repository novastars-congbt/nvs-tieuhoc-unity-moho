#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DTT.MiniGame.Anagram.Editor
{
    /// <summary>
    /// Handles generating and saving the list of all possible anagrams.
    /// Also handles drawing the refresh button for the <see cref="AnagramConfig"/> inspector.
    /// </summary>
    //[CustomEditor(typeof(AnagramConfig))]
    public class AnagramConfigEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Separators used to detect words.
        /// </summary>
        private readonly string[] _separators = new string[] { " , ", ", ", " ,", ",", "\r", "\n" };

        /// <summary>
        /// Reference to the config of this editor.
        /// </summary>
        private AnagramConfig _config;

        /// <summary>
        /// Serialized property holding all words.
        /// </summary>
        private SerializedProperty _propAllAnagrams;

        /// <summary>
        /// Gets the necessary references.
        /// </summary>
        private void OnEnable()
        {
            _config = (AnagramConfig)target;
            _propAllAnagrams = serializedObject.FindProperty("_allAnagrams");
        }

        /// <summary>
        /// Draws the refresh button.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Refresh Anagrams"))
            {
                _config.LoadWordsFromCSV(); // Call method from config
                RegenerateAnagrams();
                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Gets all anagrams from the given text files.
        /// This is called the first time playing the game.
        /// </summary>
        private void RegenerateAnagrams()
        {
            List<string> allWords = GetPossibleWords();
            List<AnagramCollection> allAnagrams = GetAllAnagrams(allWords);

            _propAllAnagrams.ClearArray();
            _propAllAnagrams.arraySize = allAnagrams.Count;

            // Sets the anagrams into the serialized property.
            for (int i = 0; i < _propAllAnagrams.arraySize; i++)
            {
                // Sets the list wrapper values to the current iterated anagram.
                SerializedProperty anagram = _propAllAnagrams.GetArrayElementAtIndex(i);
                anagram.Next(true);
                anagram.arraySize = allAnagrams[i].Count;

                for (int j = 0; j < anagram.arraySize; j++)
                    anagram.GetArrayElementAtIndex(j).stringValue = allAnagrams[i][j];
            }
        }

        /// <summary>
        /// Sorts the given list of words into a list containing anagrams.
        /// </summary>
        /// <param name="words">List of words to be sorted.</param>
        /// <returns>Double list with anagrams.</returns>
        private List<AnagramCollection> GetAllAnagrams(List<string> words)
        {
            List<string> remainingWords = new List<string>();
            remainingWords.AddRange(words);
            List<AnagramCollection> anagrams = new List<AnagramCollection>();

            // Stops once all words are sorted.
            while (remainingWords.Count != 0)
            {
                AnagramCollection currentAnagrams = new AnagramCollection();
                currentAnagrams.Add(remainingWords[0]);
                string currentWord = remainingWords[0];
                remainingWords.RemoveAt(0);

                // Looks for anagrams of this word.
                for (int i = 0; i < remainingWords.Count; i++)
                {
                    // Checks if the word is an anagram.
                    if (string.Concat(currentWord.OrderBy(c => c))
                        == string.Concat(remainingWords[i].OrderBy(c => c)))
                    {
                        currentAnagrams.Add(remainingWords[i]);
                        remainingWords.RemoveAt(i);
                        i--;
                    }
                }

                anagrams.Add(currentAnagrams);
            }
            return anagrams;
        }

        /// <summary>
        /// Gets the words from the default and/or custom text asset.
        /// </summary>
        private List<string> GetPossibleWords()
        {
            List<string> words = new List<string>();

            if (_config.UseDefaultWords)
                words.AddRange(SplitWords(_config.DefaultWords.text));

            if (_config.CustomWords != null)
                words.AddRange(SplitWords(_config.CustomWords.text));

            return words;
        }

        /// <summary>
        /// Splits the words with the <see cref="_separators"/>.
        /// </summary>
        /// <param name="allText">String of text to be split.</param>
        /// <returns>List of split words.</returns>
        private List<string> SplitWords(string allText) =>
            allText.Split(_separators, System.StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}

#endif