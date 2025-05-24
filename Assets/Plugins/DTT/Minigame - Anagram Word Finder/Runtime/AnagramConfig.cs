using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DTT.MiniGame.Anagram
{
    /// <summary>
    /// Defines the settings for an Anagram game.
    /// </summary>
    [CreateAssetMenu(menuName = "DTT/Mini Game/Anagram/Config", fileName = "New Anagram Config")]
    public class AnagramConfig : ScriptableObject
    {
        /// <summary>
        /// Difficulty settings for the anagram game.
        /// </summary>
        [SerializeField]
        private AnagramDifficulty _difficulty = new AnagramDifficulty { maxWordLength = 5, minWordLength = 3 };

        /// <summary>
        /// Indicates if word selection is random.
        /// </summary>
        [LabelText("Ngẫu nhiên cụm từ")]
        [SerializeField]
        public bool random = false;

        /// <summary>
        /// Whether only a single word is the correct answer.
        /// </summary>
        [LabelText("Cụm từ chỉ có 1 từ")]
        [SerializeField]
        [Tooltip("Whether no anagrams should be used, but instead a single correct word.")]
        private bool _useSingleWord = false;

        /// <summary>
        /// Whether to use the default word list alongside custom words.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        [Tooltip("Whether the default word list should be used as well or not. Disable to only use custom words.")]
        private bool _useDefaultWords = true;

        /// <summary>
        /// Custom text file containing possible words.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        [Tooltip("Drag your custom text file containing possible words into this field.")]
        private TextAsset _customWords;

        [System.Serializable]
        public class WordAudioPair
        {
            [LabelText("Cụm từ")]
            public string word;
            [LabelText("Âm thanh cụm từ")]
            public AudioClip audioClip;
        }

        /// <summary>
        /// List of words and their associated audio clips.
        /// </summary>
        [ShowInInspector]
        [LabelText("Danh sách cụm từ")]
        [SerializeField]
        private List<WordAudioPair> wordAudioPairs = new List<WordAudioPair>();

        public List<WordAudioPair> WordAudioPairs => wordAudioPairs;

        private List<string> _words = new List<string>();

        /// <summary>
        /// The default words for the anagram game.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private TextAsset _defaultWords;

        /// <summary>
        /// All anagrams from this config.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private List<AnagramCollection> _allAnagrams;

        /// <summary>
        /// Indicates if the default words should be used.
        /// </summary>
        public bool UseDefaultWords => _useDefaultWords;

        /// <summary>
        /// The default words asset.
        /// </summary>
        public TextAsset DefaultWords => _defaultWords;

        /// <summary>
        /// The custom words asset.
        /// </summary>
        public TextAsset CustomWords => _customWords;

        /// <summary>
        /// The difficulty settings of the anagram game.
        /// </summary>
        public AnagramDifficulty Difficulty => _difficulty;

        /// <summary>
        /// Whether only a single word is the answer.
        /// </summary>
        public bool UseSingleWord => _useSingleWord;

        /// <summary>
        /// All possible anagrams from this config.
        /// </summary>
        internal List<AnagramCollection> AllAnagrams => _allAnagrams;

        /// <summary>
        /// Sets the difficulty of the anagram.
        /// </summary>
        /// <param name="difficulty">New difficulty settings.</param>
        public void SetDifficulty(AnagramDifficulty difficulty) => _difficulty = difficulty;
        private void OnEnable()
        {
            //if (_customWords != null)
            //{
            //    LoadWordsFromCSV();
            //}
            
        }
        /// <summary>
        /// Loads words from the CSV file, clearing existing entries and repopulating the list.
        /// </summary>
        public void LoadWordsFromCSV()
        {
            if (_customWords == null)
            {
                Debug.LogError("CSV file is not assigned.");
                return;
            }

            Debug.Log("Clearing existing words...");
            wordAudioPairs.Clear(); // Clear existing entries

            using (StringReader reader = new StringReader(_customWords.text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string word = line.Trim();
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        wordAudioPairs.Add(new WordAudioPair { word = word });
                        Debug.Log($"Added word: {word}"); // Log each word added
                    }
                }
            }

            Debug.Log($"Loaded {wordAudioPairs.Count} words from CSV."); // Confirm total words loaded
        }

        public List<AnagramCollection> GetAllAnagrams()
        {
            _words.Clear();
            foreach (var wordAudioPair in wordAudioPairs)
            {
                _words.Add(wordAudioPair.word);
            }
            List<string> remainingWords = new List<string>();
            remainingWords.AddRange(_words);
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
    }
}