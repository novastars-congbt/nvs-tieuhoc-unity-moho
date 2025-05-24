using DTT.PublishingTools;
using DTT.Utils.EditorUtilities;
using UnityEditor;

namespace DTT.Hangman.Editor
{
    /// <summary>
    /// Draws the custom editor for the hangman settings.
    /// </summary>
    //[CustomEditor(typeof(HangmanSettings)), DTTHeader("dtt.minigame-hangman")]
    public class HangmanSettingsEditor : DTTInspector
    {
        /// <summary>
        /// Holds references to the serialized properties of the hangman settings.
        /// </summary>
        private class SettingsProperties : SerializedPropertyCache
        {
            /// <summary>
            /// The useAlphabet property reference.
            /// </summary>
            public SerializedProperty useAlphabet => base[nameof(useAlphabet)];

            /// <summary>
            /// The additionalLetters property reference.
            /// </summary>
            public SerializedProperty additionalLetters => base[nameof(additionalLetters)];

            /// <summary>
            /// The customCharacters property reference.
            /// </summary>
            public SerializedProperty customCharacters => base[nameof(customCharacters)];

            /// <summary>
            /// The baseLivesOnScenarioParts property reference.
            /// </summary>
            public SerializedProperty baseLivesOnScenarioParts => base[nameof(baseLivesOnScenarioParts)];

            /// <summary>
            /// The lives property reference.
            /// </summary>
            public SerializedProperty lives => base[nameof(lives)];

            /// <summary>
            /// Creates a new instance of the serialized settings properties.
            /// </summary>
            /// <param name="serializedObject">The serialized object representation of the settings.</param>
            public SettingsProperties(SerializedObject serializedObject) : base(serializedObject) {}
        }

        /// <summary>
        /// The serialized hangman properties.
        /// </summary>
        private SettingsProperties _properties;

        /// <summary>
        /// Initialized the serialized hangman properties.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _properties = new SettingsProperties(serializedObject);
        }

        /// <summary>
        /// Draws the custom inspector of the hangman settins.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            _properties.UpdateRepresentation();

            DrawPropertiesExcluding(
                serializedObject,
                _properties.useAlphabet.name,
                _properties.additionalLetters.name,
                _properties.customCharacters.name,
                _properties.baseLivesOnScenarioParts.name,
                _properties.lives.name);


            // Draw use alphabet property and additional letters disabled based on the current value.
            EditorGUILayout.PropertyField(_properties.useAlphabet);

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(_properties.useAlphabet.boolValue);
            EditorGUILayout.PropertyField(_properties.additionalLetters);
            EditorGUILayout.PropertyField(_properties.customCharacters);
            EditorGUI.EndDisabledGroup();

            // Draw base lives on scenario parts property and lives disabled based on its current value.
            EditorGUILayout.PropertyField(_properties.baseLivesOnScenarioParts);
            EditorGUI.BeginDisabledGroup(_properties.baseLivesOnScenarioParts.boolValue);

            EditorGUILayout.PropertyField(_properties.lives);

            EditorGUI.EndDisabledGroup();

            // Apply changes if the gui has changed.
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}