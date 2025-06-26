namespace NareshBisht
{
    /// <summary>
    /// Defines the possible sources for a video clip.
    /// </summary>
    public enum VideoClipSource
    {
        VideoClip,      // Use a Unity VideoClip asset.
        URL,            // Use a direct URL (e.g., web stream, local file path).
        StreamingAssets // Use a path relative to Unity's StreamingAssets folder.
    }
}