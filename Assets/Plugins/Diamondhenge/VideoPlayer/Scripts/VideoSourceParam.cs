using UnityEngine;
using UnityEngine.Video;

namespace Assets.Diamondhenge.HengeVideoPlayer
{
    /// <summary>
    /// Data structure that contains parameters for the source that this Video will read from.
    /// </summary>
    [System.Serializable]
    public class VideoSourceParam
    {
        [Tooltip("The source that this Video will read from.")]
        public VideoSource source;
        [Tooltip("The URL that this Video will read from.")]
        public string URL;
        [Tooltip("The video file that this Video will read from.")]
        public VideoClip videoClip;
    }
}
