#if UNITY_EDITOR

using DTT.PublishingTools;
using UnityEditor;

namespace DTT.Hangman.Editor
{
    /// <summary>
    /// Class that handles opening the editor window for the hangman minigame package.
    /// </summary>
    internal static class ReadMeOpener
    {
        /// <summary>
        /// Opens the readme for this package.
        /// </summary>
        [MenuItem("Tools/DTT/Hangman/ReadMe")]
        private static void OpenReadMe() => DTTEditorConfig.OpenReadMe("dtt.minigame-hangman");
    }
}
#endif