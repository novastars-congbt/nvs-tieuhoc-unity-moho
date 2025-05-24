using System.Collections.Generic;
//using System.Collections.ObjectModel;
using UnityEngine;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

namespace DTT.OrderingWords
{
    /// <summary>
    /// Holds the settings for a single level.
    /// </summary>
    [System.Serializable]
    public class LevelSettings
    {
        /// <summary>
        /// The sentence for the level.
        /// </summary>
        [LabelText("Câu hoàn chỉnh")]
        [SerializeField]
        [Tooltip("The sentence for the level")]
        private string _sentence;

        /// <summary>
        /// The audio for the sentence.
        /// </summary>
        [SerializeField]
        [LabelText("Âm thanh hoàn thành câu")]
        [Tooltip("The audio for the sentence")]
        private AudioClip _audio;

        /// <summary>
        /// The draggable words for the level.
        /// </summary>
        [SerializeField]
        [LabelText("Danh sách cụm từ")]
        [Tooltip("The draggable words for the level")]
        private List<string> _draggableWords = new List<string>();

        /// <summary>
        /// The sentence for the level.
        /// </summary>
        public string Sentence => _sentence;

        /// <summary>
        /// The audio for the sentence.
        /// </summary>
        public AudioClip Audio => _audio;

        /// <summary>
        /// The draggable words for the level.
        /// </summary>
        public List<string> DraggableWords => _draggableWords;
    }

    /// <summary>
    /// Holds the settings for the game and its levels.
    /// </summary>
    [CreateAssetMenu(fileName = "Game_Settings_template", menuName = "DTT/MiniGame/OrderingWords/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        /// <summary>
        /// The level settings for all the levels of the game.
        /// </summary>
        //[Header("Level Settings")]
        [LabelText("Danh sách câu")]
        [SerializeField]
        [Tooltip("The level settings for all the levels of the game")]
        private List<LevelSettings> _levelSettings = new List<LevelSettings>();

        /// <summary>
        /// The possible colors for the draggable words.
        /// </summary>
        //[Header("Game Settings")]
        [LabelText("Màu hiển thị ban đầu")]
        [SerializeField]        
        [Tooltip("The possible colors for the draggable words")]
        private List<Color> _possibleWordColor = new List<Color>();

        /// <summary>
        /// The color for the incorrect animation of the word.
        /// </summary>
        [SerializeField]
        [LabelText("Màu khi chọn sai")]
        [Tooltip("The color for the incorrect animation of the word")]
        private Color _incorrectColor = Color.red;

        /// <summary>
        /// The level at which the minigame should start.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        [LabelText("Cấp bắt đầu")]
        [Tooltip("The level at which the minigame should start")]
        private int _startOnLevel = 0;

        /// <summary>
        /// Path to the CSV file for importing levels.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        [Tooltip("Path to CSV file (relative to Assets folder)")]
        private string _csvFilePath = "Data/levels.csv";

        /// <summary>
        /// The level settings for all the levels of the game.
        /// </summary>
        public LevelSettings[] LevelSettings => _levelSettings.ToArray();

        /// <summary>
        /// The possible colors for the draggable words.
        /// </summary>
        public List<Color> PossibleWordColor => _possibleWordColor;

        /// <summary>
        /// The color for the incorrect animation of the word.
        /// </summary>
        public Color IncorrectColor => _incorrectColor;

        /// <summary>
        /// The level at which the minigame should start.
        /// </summary>
        [LabelText("Bắt đầu từ câu số")]
        public int StartOnLevel => _startOnLevel;

        //#if UNITY_EDITOR
        ///// <summary>
        ///// Imports level data from a CSV file.
        ///// </summary>
        //[ContextMenu("Import Levels From CSV")]
        //private void ImportFromCSV()
        //{
        //    string fullPath = Path.Combine(Application.dataPath, _csvFilePath);
        //    if (!File.Exists(fullPath))
        //    {
        //        Debug.LogError($"CSV file not found at: {fullPath}");
        //        return;
        //    }

        //    string[] lines = File.ReadAllLines(fullPath);
        //    _levelSettings.Clear();

        //    // Skip header row
        //    for (int i = 1; i < lines.Length; i++)
        //    {
        //        string[] data = lines[i].Split(',');
        //        if (data.Length < 2) continue;

        //        var level = new LevelSettings();
        //        SetLevelData(level, data[0], data[1].Split('|').ToList());
        //        _levelSettings.Add(level);
        //    }

        //    EditorUtility.SetDirty(this);
        //    AssetDatabase.SaveAssets();
        //}

        //private void SetLevelData(LevelSettings settings, string sentence, List<string> words)
        //{
        //    var sentenceField = typeof(LevelSettings).GetField("_sentence", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        //    var wordsField = typeof(LevelSettings).GetField("_draggableWords", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
        //    sentenceField?.SetValue(settings, sentence);
        //    wordsField?.SetValue(settings, words);
        //}
        //#endif
    }
}