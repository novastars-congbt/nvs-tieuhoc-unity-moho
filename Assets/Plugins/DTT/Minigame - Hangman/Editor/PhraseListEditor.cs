using DTT.PublishingTools;
using UnityEditor;

namespace DTT.Hangman.Editor
{
    /// <summary>
    /// Draws the custom inspector for the phrase list.
    /// </summary>
    [CustomEditor(typeof(PhraseList)), DTTHeader("dtt.minigame-hangman")]
    public class PhraseListEditor : DTTInspector
    {
        /// <summary>
        /// Draws the custom inspector for the phrase list.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawDefaultInspector();
        }
    }
}