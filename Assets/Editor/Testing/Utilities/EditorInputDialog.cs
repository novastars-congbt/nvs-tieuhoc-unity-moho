// Assets/Test_TieuHoc/Editor/Utilities/EditorInputDialog.cs
using UnityEngine;
using UnityEditor;

namespace Test_TieuHoc.Validation
{
    /// <summary>
    /// Hộp thoại đơn giản để nhập text trong Editor
    /// </summary>
    public class EditorInputDialog : EditorWindow
    {
        public static string Show(string title, string message, string defaultText = "")
        {
            // Tạo cửa sổ
            var window = ScriptableObject.CreateInstance<EditorInputDialog>();
            window.titleContent = new GUIContent(title);
            window.message = message;
            window.inputText = defaultText;
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 150);
            window.ShowModalUtility();
            
            return window.resultText;
        }
        
        private string message = "";
        private string inputText = "";
        private string resultText = "";
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField(message, EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);
            
            // Input field
            GUI.SetNextControlName("InputField");
            inputText = EditorGUILayout.TextField(inputText);
            
            GUILayout.Space(10);
            
            // Buttons
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Hủy", GUILayout.Width(100)))
            {
                this.Close();
            }
            
            if (GUILayout.Button("OK", GUILayout.Width(100)))
            {
                resultText = inputText;
                this.Close();
            }
            
            GUILayout.EndHorizontal();
            
            // Focus input field
            EditorGUI.FocusTextInControl("InputField");
            
            // Enter key
            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)
            {                
                resultText = inputText;
                this.Close();
            }
        }
        
        private void OnLostFocus()
        {
            // Không đóng cửa sổ khi mất focus
        }
    }
}