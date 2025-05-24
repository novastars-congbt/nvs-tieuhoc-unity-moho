using DTT.PublishingTools;
using UnityEditor;

namespace DTT.OrderingWords.Editor
{
    /// <summary>
    /// Editor class that handles the inspector GUI for the level settings.
    /// </summary>
    //[CustomEditor(typeof(GameSettings))]
    //[DTTHeader("dtt.ordering-words")]
    public class GameSettingsEditor : DTTInspector
    {
        /// <summary>
        /// Draws custom editor with DTT Header.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawDefaultInspector();
        }
    }
}