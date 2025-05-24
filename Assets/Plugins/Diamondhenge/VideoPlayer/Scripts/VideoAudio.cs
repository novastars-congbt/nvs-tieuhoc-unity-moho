using System;
using UnityEngine;
using UnityEngine.Video;

namespace Assets.Diamondhenge.HengeVideoPlayer
{
    [System.Serializable]
    /// <summary>
    /// A data structure that stores parameters for video playback audio.
    /// </summary>
    public class VideoAudio
    {
        [Tooltip("The volume of this video.")]
        [Range(0f, 1f)]
        public float volume;
        //[Tooltip("Where the volume for this video will be output.")]
        //public VideoAudioOutputMode audioOutputMode;

        protected VideoPlayer videoPlayerModule;

        /// <summary>
        /// Sets the video player that this data structure will manage the volume for.
        /// </summary>
        /// <param name="player"></param>
        public void SetVideoPlayer(VideoPlayer player)
        {
            videoPlayerModule = player;
        }

        /// <summary>
        /// Sets the volume of the currently playing video.
        /// </summary>
        /// <param name="vol"></param>
        public void SetVolume(float vol)
        {
            volume = vol;
            videoPlayerModule.SetDirectAudioVolume(0, vol);
        }
        /// <summary>
        /// Mutes the audio of the currently playing video.
        /// </summary>
        public void Mute()
        {
            videoPlayerModule.SetDirectAudioVolume(0, 0f);
        }

        /// <summary>
        /// Called whenever the mute function is toggled ON or OFF.
        /// </summary>
        /// <param name="on"></param>
        public void OnMuteToggle(bool on)
        {
            if(on)
            {
                SetVolume(volume);
            }
            else
            {
                Mute();
            }
        }
    }
}
