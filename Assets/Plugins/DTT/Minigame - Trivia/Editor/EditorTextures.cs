using DTT.PublishingTools;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DTT.Trivia.Editor
{
    /// <summary>
    /// Class containing the textures and styles used in the custom editors.
    /// </summary>
    internal class EditorTextures
    {
        /// <summary>
        /// Texture for the bin icon.
        /// </summary>
        public Texture2D BinIcon => EditorGUIUtility.isProSkin ? BinIconLight : BinIconDark;

        /// <summary>
        /// Texture for the light bin icon.
        /// </summary>
        public Texture2D BinIconLight { get; private set; }

        /// <summary>
        /// Texture for the dark bin icon.
        /// </summary>
        public Texture2D BinIconDark { get; private set; }

        /// <summary>
        /// Texture for the green box.
        /// </summary>
        public Texture2D BoxTextureCorrect { get; private set; }

        /// <summary>
        /// Texture for the red box.
        /// </summary>
        public Texture2D BoxTexture { get; private set; }

        /// <summary>
        /// Style for the green box.
        /// </summary>
        public GUIStyle BoxStyleCorrect { get; private set; }

        /// <summary>
        /// Style for the red box.
        /// </summary>
        public GUIStyle BoxStyle;

        /// <summary>
        /// Icon path for development.
        /// </summary>
        private readonly string _packageBasePath = Path.Combine("Packages", "dtt.minigame-trivia", "Demo", "Textures");

        /// <summary>
        /// Icon path for release.
        /// </summary>
        private readonly string _assetsBasePath = Path.Combine("Assets", "DTT", DTTEditorConfig.GetAssetJson("dtt.minigame-trivia").displayName, "Demo", "Textures");

        /// <summary>
        /// Initializes the textures and styles.
        /// </summary>
        public EditorTextures()
        {
            AssetJson assetJson = DTTEditorConfig.GetAssetJson("dtt.minigame-trivia");
            string relevantPath = assetJson.assetStoreRelease ? _assetsBasePath : _packageBasePath;

            BinIconLight = EditorGUIUtility.Load(Path.Combine(relevantPath, "Bin Light.png")) as Texture2D;
            BinIconDark = EditorGUIUtility.Load(Path.Combine(relevantPath, "Bin Dark.png")) as Texture2D;

            BoxTextureCorrect = new Texture2D(1, 1);
            BoxTextureCorrect.SetPixel(0, 0, Color.green);
            BoxTextureCorrect.Apply();

            BoxTexture = new Texture2D(1, 1);
            BoxTexture.SetPixel(0, 0, Color.red);
            BoxTexture.Apply();

            BoxStyleCorrect = new GUIStyle(GUIStyle.none);
            BoxStyleCorrect.normal.background = BoxTextureCorrect;

            BoxStyle = new GUIStyle(GUIStyle.none);
            BoxStyle.normal.background = BoxTexture;
        }
    }
}

