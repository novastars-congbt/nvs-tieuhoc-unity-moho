using UnityEditor;
using DTT.PublishingTools;
using UnityEngine;

namespace DTT.OrderingWords.Editor
{
    /// <summary>
    /// Editor class that handles the inspector GUI for the level settings.
    /// </summary>
    //[CustomEditor(typeof(LevelSettings))]
    //[DTTHeader("dtt.ordering-words")]
    public class LevelSettingsEditor : DTTInspector
    {
        /// <summary>
        /// The max length for the sentence of the exercise.
        /// </summary>
        private int sentenceLength = 100;

        /// <summary>
        /// Sets a min a max for the array size of the draggable words parameter.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawDefaultInspector();

            // Sentence text value
            SerializedProperty sentence = serializedObject.FindProperty("_sentence");
            string text = sentence.stringValue;

            if(text.Length >= 100)
                EditorGUILayout.TextArea("Sentence text has reached the max allowed amount.", GUI.skin.GetStyle("HelpBox"));

            else if (text.Length != 0)
                text = text.Substring(0, (int)Mathf.Min(text.Length, sentenceLength));

            sentence.stringValue = text;

            // Array size
            SerializedProperty draggableWords = serializedObject.FindProperty("_draggableWords");
            int wordAmount = draggableWords.arraySize;

            int newSize = wordAmount;
            if(wordAmount > 6)
                newSize = 6;

            else if(wordAmount < 2)
                newSize = 2;

            draggableWords.arraySize = newSize;
            serializedObject.ApplyModifiedProperties();
        }
    }
}