using System.Collections.Generic;
using UnityEditor;
using DTT.Utils.EditorUtilities;
using System.Linq;
using DTT.PublishingTools;
using UnityEditorInternal;
using UnityEngine;

namespace DTT.Trivia.Editor
{
    /// <summary>
    /// Custom editor for the question manager.
    /// </summary>
    [DTTHeader("dtt.minigame-trivia", "Question Manager")]
    //[CustomEditor(typeof(QuestionManager))]
    internal class QuestionManagerEditor : DTTInspector
    {
        /// <summary>
        /// The target class.
        /// </summary>
        private QuestionManager _questionManager;

        /// <summary>
        /// Editor list with all questions.
        /// </summary>
        private List<Question> _allQuestionsInProject;

        /// <summary>
        /// Dictionary with all questions and a boolean if they should be in the quiz.
        /// </summary>
        private Dictionary<Question, bool> _questionDictionary;

        /// <summary>
        /// Reorderable list for the questions.
        /// </summary>
        private ReorderableList _reorderableList;

        /// <summary>
        /// Loads all questions and initializes the dictionary.
        /// </summary>
        protected override void OnEnable()
        {
            //base.OnEnable();
            //_questionManager = (QuestionManager)target;
            //_allQuestionsInProject = AssetDatabaseUtility.LoadAssets<Question>().ToList();
            //_questionDictionary = _allQuestionsInProject.ToDictionary(x => x, x => _questionManager.AllQuestions.Contains(x));

            //CheckForDifferencesInLists();

            //_reorderableList = new ReorderableList(_questionManager.AllQuestions, typeof(Question), true, true, false, false)
            //{
            //    drawElementCallback = DrawListItems,
            //    drawHeaderCallback = DrawListHeader
            //};
        }

        /// <summary>
        /// Draws the questions in the reorderable list.
        /// </summary>
        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused) => EditorGUI.LabelField(new Rect(rect.x, rect.y, 300, EditorGUIUtility.singleLineHeight), _questionManager.AllQuestions[index].Title);

        /// <summary>
        /// Draws the header of the reorderable list.
        /// </summary>
        private void DrawListHeader(Rect rect) => EditorGUI.LabelField(rect, "Selected Questions");

        /// <summary>
        /// Presents all questions with a toggle next to them.
        /// Sends only checked questions to the question manager.
        /// </summary>
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            //serializedObject.Update();
            //EditorGUI.BeginChangeCheck();
            //DrawPropertiesExcluding(serializedObject, "m_Script", "_allQuestions");

            //EditorGUILayout.Space(20);

            //_reorderableList.DoLayoutList();

            //EditorGUILayout.LabelField("All Questions", EditorStyles.boldLabel);

            //EditorGUI.indentLevel += 1;

            //for (int i = 0; i < _allQuestionsInProject.Count; i++)
            //{
            //    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            //    EditorGUILayout.LabelField(_allQuestionsInProject[i].Title);
            //    _questionDictionary[_allQuestionsInProject[i]] = EditorGUILayout.Toggle(_questionDictionary[_allQuestionsInProject[i]]);
            //    EditorGUILayout.EndHorizontal();
            //}

            //EditorGUI.indentLevel -= 1;

            //if (EditorGUI.EndChangeCheck())
            //{
            //    foreach (var item in _questionDictionary)
            //    {
            //        int index = 0;
            //        if (index <= _questionManager.AllQuestions.Count)
            //        {
            //            if (!_questionManager.AllQuestions.Contains(item.Key) && item.Value)
            //                _questionManager.AllQuestions.Add(item.Key);

            //            if (_questionManager.AllQuestions.Contains(item.Key) && !item.Value)
            //                _questionManager.AllQuestions.Remove(item.Key);
            //        }

            //        index++;
            //    }

            //    EditorUtility.SetDirty(_questionManager);
            //    AssetDatabase.SaveAssets();
            //    serializedObject.ApplyModifiedProperties();
            //}

            //CheckForDifferencesInLists();
        }

        private void CheckForDifferencesInLists()
        {
            // First check for a change in the lists size.
            if (_questionDictionary.Where(x => x.Value == true).ToList().Count != _questionManager.AllQuestions.Count)
            {
                // Create a list of only questions that are not found in the dictionary of questions.
                //List<Question> toRemove = _questionManager.AllQuestions.Where(x => !_questionDictionary.ContainsKey(x)).ToList();

                // Manually delete the items of that list.
                //foreach (Question item in toRemove)
                //    _questionManager.AllQuestions.Remove(item);

                // Save changes/
                EditorUtility.SetDirty(_questionManager);
                AssetDatabase.SaveAssets();
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}