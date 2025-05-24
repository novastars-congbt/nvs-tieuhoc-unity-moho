using UnityEditor;
using Assets.Diamondhenge.HengeVideoPlayer;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

namespace Assets.Diamondhenge.HengeVideoPlayer.Editor
{
    [CustomPropertyDrawer(typeof(VideoSourceParam))]
    public class VideoSourceEditor : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            
            var prefixRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(prefixRect, property, label);
            bool expanded = property.isExpanded;
            if (expanded)
            {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 1;

                var rect1 = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
                SerializedProperty src_prop = property.FindPropertyRelative("source");
                EditorGUI.PropertyField(rect1, src_prop);

                var rect2 = new Rect(position.x, rect1.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
                SerializedProperty val_prop;
                if (src_prop.enumValueIndex == 0)
                {
                    val_prop = property.FindPropertyRelative("videoClip");
                }
                else
                {
                    val_prop = property.FindPropertyRelative("URL");
                }
                EditorGUI.PropertyField(rect2, val_prop);

                EditorGUI.indentLevel = indent;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float output = EditorGUIUtility.singleLineHeight;
            if(property.isExpanded)
            {
                output += EditorGUIUtility.singleLineHeight * 2f;
            }
            return output;
        }
    }
}
