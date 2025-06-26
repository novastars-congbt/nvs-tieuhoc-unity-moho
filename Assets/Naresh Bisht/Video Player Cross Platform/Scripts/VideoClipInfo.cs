using System;
using UnityEngine;
using UnityEngine.Video;

namespace NareshBisht
{
    /// <summary>
    /// Represents information about a video clip, including its source, URL/clip, thumbnail, and duration.
    /// This struct is designed to be easily configurable in the Unity Inspector.
    /// </summary>
    [Serializable]
    public struct VideoClipInfo
    {
        [Header("VIDEO SOURCE")]
        public VideoClipSource source; // Determines if the video is loaded from a Unity VideoClip asset or a URL.
        public VideoClip clip;         // Reference to the Unity VideoClip asset (if source is VideoClip).
        public string url;             // URL or path to the video file (if source is URL or StreamingAssets).
        
        [Header("OPTIONAL PROPERTIES")]
        public string videoName;        // Optional name of the video clip
        public Sprite thumbnail;       // Optional thumbnail image to display before video playback.
        public string duration;        // Optional string representation of the video's duration (e.g., "05:30").
        public string subtitlesUrl;     // URL or path to the subtitles file (HTTP(s) URL or StreamingAssets Path).
    }
}