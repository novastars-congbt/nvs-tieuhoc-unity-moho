#if UNITY_EDITOR

using DTT.PublishingTools;
using UnityEditor;

namespace DTT.PackageTemplate.Editor
{
    /// <summary>
    /// Class that handles opening the editor window for the package template package.
    /// </summary>
    internal static class ReadMeOpener
    {
        /// <summary>
        /// Opens the readme for this package.
        /// </summary>
        [MenuItem("Tools/DTT/MiniGame/Anagram/ReadMe")]
        private static void OpenReadMe() => DTTEditorConfig.OpenReadMe("dtt.minigame-anagram");
    }
}
#endif