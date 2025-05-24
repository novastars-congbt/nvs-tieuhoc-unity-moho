using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Novastars.MiniGame.LatBai;

using static Novastars.MiniGame.DuaXe.DataQuestions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Novastars.MiniGame.DuaXe
{
    [Serializable]
    public class QuestionData
    {
        public string QuestionString;
        public string AnswerAString;
        public string AnswerBString;

        public bool IsAnswerA;
        public bool IsAnswerB;

        public (bool, string) GetAnswer(int index)
        {
            if (index == 0)
            {
                return (IsAnswerA, AnswerAString);
            }
            else
            {
                return (IsAnswerB, AnswerBString);
            }
        }
    }

    public class DataManager : SingletonBehaviour<DataManager>
    {
        //[Space]
        //[Header("Các câu hỏi và đáp án")]
        //[LabelText("Số lượng câu hỏi mặc định")] public int DefaultQuesitonAmount;
        //[LabelText("Nội dung câu hỏi và đáp án")] public List<QuestionData> QuestionDatas = new List<QuestionData>();
        //[Space]
        [LabelText("Data")]public DataQuestions dataQuestions;
        [Header("Khung hướng dẫn"), TextArea]
        [LabelText("Nội dung khung hướng dẫn")] public string GuideString;
        

        #region Unity Funcion
        private new void Awake()
        {
            base.Awake();
        }
        #endregion
    }

//#if UNITY_EDITOR
//    [CustomPropertyDrawer(typeof(QuestionData))]
//    public class MissionDataDrawer : PropertyDrawer
//    {
//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            if (property.isExpanded)
//            {
//                return EditorGUIUtility.singleLineHeight * 9f;
//            }

//            return EditorGUIUtility.singleLineHeight;
//        }

//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            SerializedProperty questionP = property.FindPropertyRelative("QuestionString");
//            SerializedProperty answerA_stringP = property.FindPropertyRelative("AnswerAString");
//            SerializedProperty answerB_stringP = property.FindPropertyRelative("AnswerBString");
//            SerializedProperty answerA = property.FindPropertyRelative("IsAnswerA");
//            SerializedProperty answerB = property.FindPropertyRelative("IsAnswerB");

//            float spacing = 5;
//            Rect pos = EditorGUI.PrefixLabel(position, GUIContent.none);
//            pos.y += EditorGUIUtility.singleLineHeight + spacing;

//            Rect p1 = pos;
//            p1.height = EditorGUIUtility.singleLineHeight * 3;

//            pos.y += EditorGUIUtility.singleLineHeight * 3 + spacing;
//            Rect p2 = pos;
//            Rect p4 = p2;
//            p2.height = EditorGUIUtility.singleLineHeight * 2;
//            p2.width = pos.width * 0.925f;

//            p4.height = EditorGUIUtility.singleLineHeight;
//            p4.x += p2.width + pos.width * 0.025f;
//            p4.y += EditorGUIUtility.singleLineHeight * 0.5f;

//            pos.y += EditorGUIUtility.singleLineHeight * 2 + spacing;
//            Rect p3 = pos;
//            Rect p5 = p3;
//            p3.height = EditorGUIUtility.singleLineHeight * 2;
//            p3.width = pos.width * 0.925f;

//            p5.height = EditorGUIUtility.singleLineHeight;
//            p5.x += p3.width + pos.width * 0.025f;
//            p5.y += EditorGUIUtility.singleLineHeight * 0.5f;

//            EditorGUIUtility.labelWidth = 80f;
//            EditorGUI.BeginProperty(position, label, property);
//            property.isExpanded = EditorGUI.Foldout(new Rect(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

//            if (property.isExpanded)
//            {
//                EditorGUI.PropertyField(p1, questionP, GUIContent.none);

//                if (!answerA.boolValue && !answerB.boolValue)
//                {
//                    EditorGUI.PropertyField(p2, answerA_stringP, new GUIContent("Answer A"));
//                    EditorGUI.PropertyField(p3, answerB_stringP, new GUIContent("Answer B"));
//                    EditorGUI.PropertyField(p4, answerA, GUIContent.none, true);
//                    EditorGUI.PropertyField(p5, answerB, GUIContent.none, true);
//                }
//                else if (answerA.boolValue)
//                {
//                    GUI.color = Color.green;
//                    EditorGUI.PropertyField(p2, answerA_stringP, new GUIContent("Answer A"));
//                    EditorGUI.PropertyField(p4, answerA, GUIContent.none, true);
//                    GUI.color = Color.white;

//                    EditorGUI.PropertyField(p3, answerB_stringP, new GUIContent("Answer B"));
//                }
//                else if (answerB.boolValue)
//                {
//                    GUI.color = Color.green;
//                    EditorGUI.PropertyField(p3, answerB_stringP, new GUIContent("Answer B"));
//                    EditorGUI.PropertyField(p5, answerB, GUIContent.none, true);
//                    GUI.color = Color.white;

//                    EditorGUI.PropertyField(p2, answerA_stringP, new GUIContent("Answer A"));
//                }
//            }

//            EditorGUI.EndProperty();
//        }
//    }

//    [CustomEditor(typeof(DataManager))]
//    public class GameDataManagerEditor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            DrawDefaultInspector();

//            DataManager manager = (DataManager)target;
//            if (GUILayout.Button("Save Data"))
//            {
//                SaveScriptableObject(manager, manager.dataQuestions);
//            }
//        }

//        private void SaveScriptableObject(DataManager dataManager, DataQuestions dataQuestions)
//        {
//            dataQuestions.DefaultQuesitonAmount = dataManager.DefaultQuesitonAmount;
//            dataQuestions.QuestionDatas.Clear();
//            for (int i = 0; i < dataManager.QuestionDatas.Count; i++)
//            {
//                dataQuestions.QuestionDatas.Add(new DataQuestions.QuestionData());
//                dataQuestions.QuestionDatas[i].QuestionString = dataManager.QuestionDatas[i].QuestionString;
//                dataQuestions.QuestionDatas[i].AnswerAString = dataManager.QuestionDatas[i].AnswerAString;
//                dataQuestions.QuestionDatas[i].IsAnswerA = dataManager.QuestionDatas[i].IsAnswerA;
//                dataQuestions.QuestionDatas[i].AnswerBString = dataManager.QuestionDatas[i].AnswerBString;
//                dataQuestions.QuestionDatas[i].IsAnswerB = dataManager.QuestionDatas[i].IsAnswerB;
//            }
//            EditorUtility.SetDirty(dataQuestions);
//            AssetDatabase.SaveAssets();
//            Debug.Log("ScriptableObject data saved!");
//        }
//    }
//#endif
}