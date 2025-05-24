using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.MiniGame.Anagram
{
    /// <summary>
    /// Handles generating the set of possible anagrams used in the anagram game.
    /// </summary>
    internal class AnagramGenerator
    {
        /// <summary>
        /// Current configuration of the game.
        /// </summary>
        private AnagramConfig _currentConfig;

        /// <summary>
        /// All anagrams.
        /// </summary>
        private List<AnagramCollection> _allAnagrams = new List<AnagramCollection>();

        /// <summary>
        /// All anagrams with the specified allowed min and max length.
        /// </summary>
        private List<AnagramCollection> _possibleAnagrams = new List<AnagramCollection>();

        /// <summary>
        /// Random number generator.
        /// </summary>
        private System.Random _random = new System.Random();

        /// <summary>
        /// Tracks indices of used anagrams to prevent repetition.
        /// </summary>
        private HashSet<int> _usedIndices = new HashSet<int>();

        /// <summary>
        /// If the anagrams list hasn't been generated yet, it generates it.
        /// It will then choose a word from the anagrams and return it, ensuring no repetition.
        /// </summary>
        /// <param name="config">Configuration of the anagram game.</param>
        /// <returns>Non-repeating list of words which are anagrams of each other.</returns>
        internal AnagramCollection Generate(AnagramConfig config)
        {
            _currentConfig = config;

            _allAnagrams = config.GetAllAnagrams();

            // Checks if there are any anagrams.
            if (_allAnagrams.Count == 0)
            {
                Debug.LogError("Minigame Anagram Error: No anagrams found, make sure you regenerated the anagrams on" +
                    " the AnagramConfig file.");
                return new AnagramCollection();
            }

            _possibleAnagrams = GetAnagramsWithSize(_currentConfig.Difficulty.minWordLength, _currentConfig.Difficulty.maxWordLength);

            // Checks if there are possible anagrams with the given word length.
            if (_possibleAnagrams.Count == 0)
            {
                Debug.LogError("Minigame Anagram Error: No words found with given min and max size.");
                return new AnagramCollection();
            }

            // Reset used indices if all anagrams have been used
            if (_usedIndices.Count >= _possibleAnagrams.Count)
            {
                Reset();
            }

            int selectedIndex;

            // Select an index based on the 'random' flag
            if (_currentConfig.random)
            {
                // Choose a random index that hasn't been used
                do
                {
                    selectedIndex = _random.Next(0, _possibleAnagrams.Count);
                } while (_usedIndices.Contains(selectedIndex));
            }
            else
            {
                // Select the next unused index sequentially
                selectedIndex = 0;
                while (_usedIndices.Contains(selectedIndex) && selectedIndex < _possibleAnagrams.Count)
                {
                    Debug.LogError("selectedIndex: " + selectedIndex);
                    selectedIndex++;
                }  
            }

            // Mark the index as used
            _usedIndices.Add(selectedIndex);
            Debug.LogError("selectedIndexLast: " + selectedIndex);
            AnagramCollection selectedAnagram = _possibleAnagrams[selectedIndex];

            // Return a list with a single word.
            if (_currentConfig.UseSingleWord)
                return new AnagramCollection() { GetRandomWordFromAnagram(selectedAnagram) };

            return selectedAnagram;
        }

        /// <summary>
        /// Gets a single random word from an anagram list.
        /// </summary>
        /// <param name="anagram">List of anagrams.</param>
        /// <returns>Random word from anagrams.</returns>
        private string GetRandomWordFromAnagram(AnagramCollection anagram)
        {
            int randomIndex = _random.Next(0, anagram.Count);
            return anagram[randomIndex];
        }

        public void Reset()
        {
            _usedIndices.Clear();
        }

        /// <summary>
        /// Gets the anagrams that have a certain word length.
        /// </summary>
        /// <param name="minLength">Minimum word length.</param>
        /// <param name="maxLength">Maximum word length.</param>
        /// <returns>All anagrams within the min and max length.</returns>
        private List<AnagramCollection> GetAnagramsWithSize(uint minLength, uint maxLength) =>
             _allAnagrams.Where(l => l[0].Length >= minLength && l[0].Length <= maxLength).ToList();
    }
}