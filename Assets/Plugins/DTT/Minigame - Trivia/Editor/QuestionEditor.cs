using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using DTT.PublishingTools;

namespace DTT.Trivia.Editor
{
    /// <summary>
    /// Custom editor for all question types.
    /// </summary>
    [DTTHeader("dtt.minigame-trivia", "Question")]
    //[CustomEditor(typeof(Question), true)]
    internal class QuestionEditor : DTTInspector
    {
        /// <summary>
        /// Class for the textures.
        /// </summary>
        private EditorTextures _textures;

        /// <summary>
        /// Texture for the question object.
        /// </summary>
        private Question _question;

        /// <summary>
        /// List of the answers shown in the editor.
        /// </summary>
        private readonly List<Answer> _editorAnswers = new List<Answer>();

        /// <summary>
        /// Used for the answers foldout.
        /// </summary>
        private readonly List<bool> _toggled = new List<bool>();

        /// <summary>
        /// List of found answer types.
        /// </summary>
        private Type[] _answerTypes;

        /// <summary>
        /// Tyoe of the question object.
        /// </summary>
        private Type _objectType;

        /// <summary>
        /// Index of the chosen Answer type from the dropdown.
        /// </summary>
        private int _implementationTypeIndex;

        /// <summary>
        /// Serialized list of answers.
        /// </summary>
        private SerializedProperty _answers;

        /// <summary>
        /// Used to foldout the editor answers title.
        /// </summary>
        private bool _answersFolded = true;

        /// <summary>
        /// Menu used for the dropdown.
        /// </summary>
        private GenericMenu _menu;

        /// <summary>
        /// Reference to the FieldInfo.
        /// </summary>
        private System.Reflection.FieldInfo fieldInfo;

        /// <summary>
        /// Creates styles and textures for the box that represents if a question is correct.
        /// Chooses the right path by checking if the package has been released or not.
        /// Chooses the right color variant for the bin icon.
        /// </summary>
        protected override void OnEnable()
        {
            //base.OnEnable();
            //_textures = new EditorTextures();
        }

        /// <summary>
        /// Draws the custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            //serializedObject.Update();
            //EditorGUI.BeginChangeCheck();
            //_question = (Question)target;
            //_answers = serializedObject.FindProperty("_answers");
            //_objectType = serializedObject.targetObject.GetType();
            //_menu = new GenericMenu();

            //DrawQuestionDetails();
            //EditorGUILayout.Space(30);
            //DrawAnswers();
            //EditorGUILayout.Space(20);
            //DrawErrorFields();

            //if (EditorGUI.EndChangeCheck())
            //    serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws the question details.
        /// </summary>
        private void DrawQuestionDetails()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(_objectType.Name, EditorStyles.boldLabel);
            EditorGUI.indentLevel += 1;
            DrawPropertiesExcluding(serializedObject, "m_Script", "_answers");
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws the list with answers.
        /// Able to display any type of answer and also mix them.
        /// </summary>
        private void DrawAnswers()
        {
            DrawAnswersControls();

            if (!_answersFolded)
                return;

            DrawAnswersEntries();
        }

        /// <summary>
        /// Draws the list controls.
        /// </summary>
        private void DrawAnswersControls()
        {
            EditorGUILayout.BeginHorizontal();

            _answersFolded = EditorGUILayout.Foldout(_answersFolded, "Answers");
            if (_answerTypes == null)
                _answerTypes = GetImplementations<Answer>().Where(impl => !impl.IsSubclassOf(typeof(UnityEngine.Object))).ToArray();

            for (int i = 0; i < _answerTypes.Length; i++)
                _menu.AddItem(new GUIContent(_answerTypes[i].Name), false, OnTypeSelected, i);

            GUILayout.FlexibleSpace();
            if (EditorGUILayout.DropdownButton(new GUIContent(_answerTypes[_implementationTypeIndex].Name), FocusType.Keyboard))
            {
                _answerTypes = GetImplementations<Answer>().Where(impl => !impl.IsSubclassOf(typeof(UnityEngine.Object))).ToArray();
                _menu.ShowAsContext();
            }

            void OnTypeSelected(object obj) => _implementationTypeIndex = (int)obj;

            if (GUILayout.Button("+", GUILayout.MaxWidth(30), GUILayout.MaxHeight(20)))
            {
                object handle = Activator.CreateInstance(_answerTypes[_implementationTypeIndex]);
                _question._answers.Add((Answer)handle);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws each Answer in the list.
        /// </summary>
        private void DrawAnswersEntries()
        {
            EditorGUI.indentLevel += 1;
            fieldInfo = _objectType.GetField(_answers.propertyPath);

            if (fieldInfo.GetValue(serializedObject.targetObject) == null)
                return;

            _editorAnswers.Clear();
            _editorAnswers.AddRange((List<Answer>)fieldInfo.GetValue(serializedObject.targetObject));

            for (int i = 0; i < _answers.arraySize; i++)
            {
                _toggled.Add(false);
                EditorGUILayout.BeginHorizontal(EditorStyles.textArea);

                _toggled[i] = EditorGUILayout.Foldout(_toggled[i], "");

                switch (_editorAnswers[i])
                {
                    default:
                        break;
                    case AnswerAudio answerAudio:
                        SerializedProperty audio = _answers.GetArrayElementAtIndex(i).FindPropertyRelative("_audio");
                        audio.objectReferenceValue = EditorGUILayout.ObjectField(audio.objectReferenceValue, typeof(AudioClip), true);
                        break;
                    case AnswerImage answerImage:
                        SerializedProperty image = _answers.GetArrayElementAtIndex(i).FindPropertyRelative("_image");
                        image.objectReferenceValue = EditorGUILayout.ObjectField(image.objectReferenceValue, typeof(Sprite), true, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        break;
                    case AnswerFull answerFull:
                        EditorGUILayout.BeginVertical();
                        SerializedProperty fullaudio = _answers.GetArrayElementAtIndex(i).FindPropertyRelative("_audio");
                        fullaudio.objectReferenceValue = EditorGUILayout.ObjectField(fullaudio.objectReferenceValue, typeof(AudioClip), true);
                        EditorGUILayout.Space();
                        SerializedProperty fullimage = _answers.GetArrayElementAtIndex(i).FindPropertyRelative("_image");
                        fullimage.objectReferenceValue = EditorGUILayout.ObjectField(fullimage.objectReferenceValue, typeof(Sprite), true, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        EditorGUILayout.EndVertical();
                        break;
                    case Answer answer:
                        SerializedProperty body = _answers.GetArrayElementAtIndex(i).FindPropertyRelative("_body");
                        if (body == null)
                            return;
                        body.stringValue = EditorGUILayout.TextField(body.stringValue ?? "");
                        break;
                }

                EditorGUILayout.Space();

                if (_answers.GetArrayElementAtIndex(i).FindPropertyRelative("_isCorrect").boolValue)
                    GUILayout.Box(_textures.BoxTextureCorrect, _textures.BoxStyleCorrect, GUILayout.MaxWidth(EditorGUIUtility.singleLineHeight), GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
                else
                    GUILayout.Box(_textures.BoxTexture, _textures.BoxStyle, GUILayout.MaxWidth(EditorGUIUtility.singleLineHeight), GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));

                EditorGUILayout.Space();

                if (GUILayout.Button(_textures.BinIcon, GUILayout.MaxWidth(25), GUILayout.MaxHeight(20)))
                {
                    _answers.DeleteArrayElementAtIndex(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();

                if (!_toggled[i])
                    continue;

                EditorGUI.indentLevel += 1;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                SerializedProperty isCorrect = _answers.GetArrayElementAtIndex(i).FindPropertyRelative("_isCorrect");
                isCorrect.boolValue = EditorGUILayout.Toggle("Is Correct? ", isCorrect.boolValue);

                switch (_editorAnswers[i])
                {
                    case AnswerAudio answerAudio:
                        SerializedProperty ansbody = _answers.GetArrayElementAtIndex(i).FindPropertyRelative("_body");
                        ansbody.stringValue = EditorGUILayout.TextField("Body: ", ansbody.stringValue ?? "");
                        break;
                    case AnswerImage answerImage:
                        SerializedProperty imgbody = _answers.GetArrayElementAtIndex(i).FindPropertyRelative("_body");
                        imgbody.stringValue = EditorGUILayout.TextField("Body: ", imgbody.stringValue ?? "");
                        break;
                    case AnswerFull answerFull:
                        SerializedProperty fullbody = _answers.GetArrayElementAtIndex(i).FindPropertyRelative("_body");
                        fullbody.stringValue = EditorGUILayout.TextField("Body: ", fullbody.stringValue ?? "");
                        break;
                    default:
                        break;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel -= 1;
            }
            EditorGUI.indentLevel -= 1;
        }

        /// <summary>
        /// Draws the error fields.
        /// </summary>
        private void DrawErrorFields()
        {
            if (_answers.arraySize < 1)
                return;

            Answer[] correctAnswers = _editorAnswers.Where(x => x.IsCorrect == true).ToArray();

            if (_editorAnswers.Count < 2)
                EditorGUILayout.HelpBox("There must be at least two answers.", MessageType.Warning);
            else if (correctAnswers.Length < 1)
                EditorGUILayout.HelpBox("No correct answer selected.", MessageType.Warning);
        }

        /// <summary>
        /// Gets all implementation of type T.
        /// </summary>
        /// <typeparam name="T">The base class.</typeparam>
        /// <returns>An array of the found types.</returns>
        private static Type[] GetImplementations<T>()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());

            var interfaceType = typeof(T);

            return types.Where(p => interfaceType.IsAssignableFrom(p) && !p.IsAbstract).ToArray();
        }
    }
}