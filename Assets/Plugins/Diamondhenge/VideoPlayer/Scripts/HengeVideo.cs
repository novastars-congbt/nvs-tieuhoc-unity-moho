using System;
using System.Collections;
using UnityEngine;
//using UnityEngine.InputSystem.Android.LowLevel;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Assets.Diamondhenge.HengeVideoPlayer
{
    /// <summary>
    /// A module that plays a video on a UI element.
    /// </summary>
    public class HengeVideo : MonoBehaviour
    {
        protected const float OVERLAY_FADE_SPEED = 10f;
        protected const float TOOLBAR_FADE_SPEED = 2f;
        protected const float PAUSE_OPACITY = 0.5f;
        protected const float MOUSE_MOVE_SENSITIVITY = 0.1f;

        [Tooltip("Parameters that affect video playback")]
        public VideoOptions videoOptions;
        [Tooltip("Parameters for the source that this Video will read from.")]
        public VideoSourceParam videoSource;
        [Tooltip("Parameters that control the volume of the playing video.")]
        public VideoAudio videoAudio;
        public string url;
        [Tooltip("Parameters for the video's toolbar.")]
        public VideoToolbar videoToolbar;
        [Tooltip("References to all of the modules that this script uses.")]
        public ObjectReferences objectReferences;

        public bool playOnAwake;
        public string nameClass;
        public string nameLesson;

        protected CanvasGroup toolbarCGroup;
        protected RawImage imageModule;
        protected VideoPlayer videoPlayerModule;
        protected RenderTexture renderTextureInst;

        protected IEnumerator darkenRoutine = null;
        protected IEnumerator watchProgress = null;
        protected IEnumerator fadeToolbar = null;
        protected IEnumerator toolbarReveal = null;

        protected bool toolbarVisible = true;
        protected bool videoStarted = false;
        protected Vector2 lastCursorPos = Vector2.zero;

        public void Awake()
        {
            imageModule = GetComponentInChildren<RawImage>();
            videoPlayerModule = GetComponentInChildren<VideoPlayer>();

            //videoToolbar.Initialize();
            videoToolbar.volumeButton.onToggle.AddListener((f) => videoAudio.OnMuteToggle(!f));

            videoAudio.SetVideoPlayer(videoPlayerModule);
            //objectReferences.volumeSlider.onValueChanged.AddListener((f) => SetVolume(f));

            videoToolbar.volumeButton.imageModule.gameObject.SetActive(false);
            objectReferences.volumeSlider.gameObject.SetActive(false);

            //LoadVideoData();
            //SetLooping(videoOptions.loop);
            //OnValidate();
            //if (!videoOptions.thumbnail)
            //{
            //    StartCoroutine(AwakeRoutine());
            //}
        }

        private void OnEnable()
        {
            if (!videoStarted) return;
            SetPauseState(!videoPlayerModule.isPaused);
        }

        void BackVideo()
        {
            UpdateVideoTime();
            SetPauseState(!videoPlayerModule.isPaused);
        }

        private void Start()
        {
            LoadVideoData();
            SetLooping(videoOptions.loop);
            OnValidate();
            if (!videoOptions.thumbnail)
            {
                StartCoroutine(AwakeRoutine());
            }
        }
        public IEnumerator AwakeRoutine()
        {
            //If there's no thumbnail, we load the video and play it for a single frame before pausing it.
            //This will make it look like the video is using the first frame as a thumbnail.
            //yield return PlayVideoRoutine();
            //SetPauseState(true);
            yield return null;
            StartCoroutine(PlayVideoRoutine());
        }

        /// <summary>
        /// Changes the thumbnail in the editor if the user sets a custom thumbnail in VideoOptions.
        /// </summary>
        private void OnValidate()
        {
            if (videoOptions.thumbnail)
            {
                objectReferences.background.sprite = videoOptions.thumbnail;
                objectReferences.background.color = Color.white;
            }
            else
            {
                objectReferences.background.sprite = null;
                objectReferences.background.color = Color.black;
            }
        }
        private void Update()
        {
            if (videoPlayerModule.isPlaying)
            {
                objectReferences.videoTimeSlider.value = (float)(videoPlayerModule.time / videoPlayerModule.length);
                Vector2 mouse_pos = Input.mousePosition;
                if (Vector3.Distance(lastCursorPos, mouse_pos) >= MOUSE_MOVE_SENSITIVITY)
                {
                    RevealToolbar();
                    lastCursorPos = mouse_pos;
                }
            }
        }

        /// <summary>
        /// Loads the video data stored in the VideoSource parameters into the videoPlayer module.
        /// </summary>
        public void LoadVideoData()
        {
            //if (!isWebGL)
            //{
            //    videoPlayerModule.clip = videoSource.videoClip;
            //}
            //else
            //{
            //    videoPlayerModule.url = url;
            //}
            videoPlayerModule.source = videoSource.source;
            if (videoSource.source == VideoSource.VideoClip)
            {
                videoPlayerModule.clip = videoSource.videoClip;
            }
            else
            {
                videoPlayerModule.url = System.IO.Path.Combine(Application.streamingAssetsPath, nameClass, nameLesson, videoSource.URL);
            }
        }
        /// <summary>
        /// Plays the currently loaded video.
        /// </summary>
        public void PlayVideo()
        {
            if(videoStarted)
            {
                SetPauseState(false);
            }
            else
            {
                StartCoroutine(PlayVideoRoutine());
            }
        }
        /// <summary>
        /// Toggles whether this video is playing or paused.
        /// </summary>
        public void TogglePauseState()
        {
            SetPauseState(!videoPlayerModule.isPaused);
        }
        /// <summary>
        /// Sets whether this video is paused.
        /// </summary>
        /// <param name="pause"></param>
        public void SetPauseState(bool pause)
        {
            if (pause)
            {
                if (toolbarReveal != null)
                {
                    StopCoroutine(toolbarReveal);
                }
                videoPlayerModule.Pause();
                FadeToolbar(true);
            }
            else
            {
                if (videoStarted)
                {
                    videoPlayerModule.Play();
                }
                else
                {
                    StartCoroutine(PlayVideoRoutine());
                }
                FadeToolbar(false, videoToolbar.fadeDelay);
            }
            videoToolbar.playButton.SetButtonState(pause);
            //if (darkenRoutine != null)
            //{
            //    StopCoroutine(darkenRoutine);
            //}
            //darkenRoutine = UpdateVideoDarkState();
            //StartCoroutine(darkenRoutine);
        }
        /// <summary>
        /// Sets whether or not this video will loop on playback.
        /// </summary>
        /// <param name="loop"></param>
        public void SetLooping(bool loop)
        {
            videoPlayerModule.isLooping = loop;
            videoOptions.loop = loop;
        }

        /// <summary>
        /// Sets the current time position of the current video's playback.
        /// </summary>
        /// <param name="time">The time position of the video on a scale of 0 to 1. 0 is the beginning. 1 is the end.</param>
        public void SetVideoTime(float time)
        {
            time = Mathf.Round(time * 10000f) / 10000f;
            videoPlayerModule.time = videoPlayerModule.length * time;
        }
        /// <summary>
        /// Updates the current video time so that it matches the value of the videoTimeSlider.
        /// </summary>
        public void UpdateVideoTime()
        {
            SetVideoTime(objectReferences.videoTimeSlider.value);
        }

        /// <summary>
        /// Sets the video's audio volume.
        /// </summary>
        /// <param name="vol"></param>
        public void SetVolume(float vol)
        {
            videoToolbar.volumeButton.SetButtonState(vol == 0f);
            videoAudio.SetVolume(vol);
        }

        /// <summary>
        /// Reveals the video toolbar for a few seconds.
        /// </summary>
        public void RevealToolbar()
        {
            if(toolbarReveal != null)
            {
                StopCoroutine(toolbarReveal);
            }
            toolbarReveal = RevealToolbarRoutine();
            StartCoroutine(toolbarReveal);
        }
        protected IEnumerator RevealToolbarRoutine()
        {
            FadeToolbar(true);
            while (fadeToolbar != null)
            {
                yield return null;
            }
            FadeToolbar(false, videoToolbar.fadeDelay);
            toolbarReveal = null;
        }

        /// <summary>
        /// Changes the darkness of the video display based on whether or not the video is paused.
        /// </summary>
        /// <returns></returns>
        protected IEnumerator UpdateVideoDarkState()
        {
            Image overlay = objectReferences.playStopOverlay;
            Color current_alpha = overlay.color;
            Color target_alpha = new Color(overlay.color.r, overlay.color.g, overlay.color.b,
                videoPlayerModule.isPaused ? PAUSE_OPACITY : 0f);
            float lerp = 0f;

            while (lerp < 1f)
            {
                lerp = Mathf.MoveTowards(lerp, 1f, OVERLAY_FADE_SPEED * Time.deltaTime);
                overlay.color = Color.Lerp(current_alpha, target_alpha, lerp);
                yield return null;
            }
            overlay.color = target_alpha;
            yield return null;

            darkenRoutine = null;
        }
        /// <summary>
        /// A coroutine that Plays the video. (Should be called after the video has been 
        /// loaded through LoadVideoData().
        /// </summary>
        /// <returns></returns>
        protected IEnumerator PlayVideoRoutine()
        {
            videoPlayerModule.Prepare();
            yield return null;
            while(!videoPlayerModule.isPrepared)
            {
                yield return null;
            }

            renderTextureInst = new RenderTexture((int)videoPlayerModule.width, (int)videoPlayerModule.height, 32);
            videoPlayerModule.targetTexture = renderTextureInst;
            objectReferences.background.sprite = null;
            objectReferences.background.color = Color.black;
            imageModule.color = Color.white;
            imageModule.texture = renderTextureInst;
            AspectRatioFitter aspect = GetComponentInChildren<AspectRatioFitter>();
            aspect.aspectRatio = (videoPlayerModule.width * 1f) / (videoPlayerModule.height * 1f);
            if (playOnAwake) SetPauseState(false);
            else SetPauseState(true);
            videoStarted = true;
            FadeToolbar(false, videoToolbar.fadeDelay);
        }
        /// <summary>
        /// Fades the toolbar to the given opacity.
        /// </summary>
        /// <param name="visible">If set to true, fades the toolbar to be opaque. If false, fades to be fully transparent.</param>
        /// <param name="delay">The delay before fading the toolbar.</param>
        protected void FadeToolbar(bool visible, float delay=0f)
        {
            if (fadeToolbar != null)
            {
                StopCoroutine(fadeToolbar);
            }
            fadeToolbar = FadeToolbarRoutine(visible, delay);
            StartCoroutine(fadeToolbar);
        }
        /// <summary>
        /// A coroutine that fades the toolbar.
        /// </summary>
        /// <param name="visible"></param>
        /// <returns></returns>
        protected IEnumerator FadeToolbarRoutine(bool visible, float delay=0f)
        {
            yield return new WaitForSeconds(delay);
            float alpha = objectReferences.toolbarCGroup.alpha;
            float target = visible ? 1f : 0f;
            float speed = visible ? OVERLAY_FADE_SPEED : TOOLBAR_FADE_SPEED;
            while (alpha != target)
            {
                alpha = Mathf.MoveTowards(alpha, target, Time.deltaTime * speed);
                objectReferences.toolbarCGroup.alpha = alpha;
                yield return null;
            }
            objectReferences.toolbarCGroup.alpha = target;
            fadeToolbar = null;
        }
    }

    [System.Serializable]
    public class VideoOptions
    {
        [Tooltip("If set to true, the video will loop on playback.")]
        public bool loop;
        [Tooltip("If set, the video will display this thumbnail before the user plays back.")]
        public Sprite thumbnail;
    }
    [System.Serializable]
    public class ObjectReferences
    {
        [Tooltip("The black bars to show on the video if the video aspect ration doesn't match the UI's aspect ration." +
            "We also use this to display custom thumbnails.")]
        public Image background;
        [Tooltip("The image module that will darken when this video is paused.")]
        public Image playStopOverlay;
        [Tooltip("The CanvasGroup that controls the alpha of the video toolbar.")]
        public CanvasGroup toolbarCGroup;
        [Tooltip("The Slider module that tracks the progress of the video.")]
        public Slider videoTimeSlider;
        [Tooltip("The Slider module that sets the volume of the video.")]
        public Slider volumeSlider;
    }
}
