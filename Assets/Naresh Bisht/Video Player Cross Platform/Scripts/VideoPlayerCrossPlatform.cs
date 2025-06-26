using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using UnityEngine.EventSystems;

namespace NareshBisht
{
    /// <summary>
    /// A custom video player script for Unity, designed to be cross-platform.
    /// It provides a UI for playback controls, fullscreen toggling, volume control,
    /// and video playlist management.
    /// </summary>
    public class VideoPlayerCrossPlatform : MonoBehaviour
    {
        // --- UI REFERENCES ---
        // These fields are serialized to be assigned in the Unity Inspector,
        // connecting the script to the various UI elements and the VideoPlayer component.
        [Header("REFERENCES")]
        [Tooltip("Array of video clips to be played in the player.")]
        [SerializeField] private List<VideoClipInfo> videoClipInfoArray;
        [Tooltip("The root GameObject containing the video player UI and video display.")]
        [SerializeField] private GameObject videoPlayerContainer;
        [Tooltip("The Unity VideoPlayer component responsible for video playback.")]
        [SerializeField] public VideoPlayer videoPlayer;
        [Tooltip("Button covering the video frame, typically used for toggling controls or play/pause.")]
        [SerializeField] private Button videoFrame;
        [Tooltip("RawImage used to display the video texture.")]
        [SerializeField] private RawImage videoDisplay;
        [Tooltip("CanvasGroup controlling the visibility and interactability of the playback controls.")]
        [SerializeField] private CanvasGroup controls;
        [Tooltip("RectTransform of the bottom control bar.")]
        [SerializeField] private RectTransform bottomControls;
        [Tooltip("Image for displaying a thumbnail before video playback.")]
        [SerializeField] private Image thumbnailImage;
        [Tooltip("Transform of the loading icon, which rotates to indicate buffering.")]
        [SerializeField] private Transform loadingIcon;
        [Tooltip("Button to play/pause the video.")]
        [SerializeField] private Button playButton;
        [Tooltip("Button to play the previous video in the playlist.")]
        [SerializeField] private Button prevButton;
        [Tooltip("Button to play the next video in the playlist.")]
        [SerializeField] private Button nextButton;
        [Tooltip("Button to replay the current video from the beginning.")]
        [SerializeField] private Button replayButton;
        [Tooltip("Button to toggle fullscreen mode.")]
        [SerializeField] private Button fullscreenButton;
        [Tooltip("Button to toggle audio mute/unmute.")]
        [SerializeField] private Button audioButton;
        [Tooltip("Image overlayed on the audio button to indicate mute status.")]
        [SerializeField] private Image audioMuteImage;
        [Tooltip("Slider to control the audio volume.")]
        [SerializeField] private Slider audioSlider;
        [Tooltip("Large play/pause button typically positioned in the center of the video.")]
        [SerializeField] private Button centerPlayButton;
        [Tooltip("Image component of the center play/pause button.")]
        [SerializeField] private Image centerPlayButtonImage;
        [Tooltip("Button to close or hide the video player.")]
        [SerializeField] private Button closeButton;
        [Tooltip("TextMeshProUGUI component for displaying current time and total duration.")]
        [SerializeField] private TextMeshProUGUI videoNameText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI subtitlesText;
        [SerializeField] private RectTransform subtitlesTextContainer;
        [Tooltip("Slider representing the video's progress bar.")]
        [SerializeField] private Slider progressBar;
        [Tooltip("CanvasScaler for dynamically adjusting UI element sizes based on screen resolution.")]
        [SerializeField] private CanvasScaler canvasScaler;

        // --- SPRITES ---
        // Sprites for changing button icons based on player state.
        [Header("SPRITES")]
        [Tooltip("Sprite for the play button icon.")]
        [SerializeField] private Sprite playSprite;
        [Tooltip("Sprite for the pause button icon.")]
        [SerializeField] private Sprite pauseSprite;
        [Tooltip("Sprite for the fullscreen entry button icon.")]
        [SerializeField] private Sprite fullscreenEnterSprite;
        [Tooltip("Sprite for the fullscreen exit button icon.")]
        [SerializeField] private Sprite fullscreenExitSprite;

        // --- SETTINGS ---
        // Configurable settings for the video player's behavior.
        [Header("SETTINGS")]
        [Tooltip("Determines if the video player starts in fullscreen mode.")]
        [SerializeField] private bool enableKeyboardShortcuts = true;
        [Tooltip("Determines if the video player starts in fullscreen mode.")]
        [SerializeField] private bool isFullscreen = true;
        [Tooltip("If true, the video display will maintain its aspect ratio.")]
        [SerializeField] private bool preserveVideoAspectRatio = true;
        [Tooltip("If true, pressing the native back button (e.g., Android) closes the player.")]
        [SerializeField] private bool nativeBackButtonClosesPlayer = true;
        [Tooltip("Default text to display for video duration when not available (e.g., '--:--').")]
        [SerializeField] private string defaultVideoDuration = "--:--";
        [Tooltip("Delay in seconds before controls automatically hide when inactive.")]
        [SerializeField] private float controlsAutoHideDelay = 3f;
        [Tooltip("Speed at which controls fade in and out.")]
        [SerializeField] private float controlsFadeSpeed = 3f;
        [Tooltip("Speed of the fullscreen transition animation.")]
        [SerializeField] private float fullscreenAnimSpeed = 5f;
        [Tooltip("Interval in seconds to re-prepare the video player if an error occurs.")]
        [SerializeField] private float reprepareInterval = 2f;
        [Tooltip("The time threshold in seconds after which the loading icon is displayed while the video is loading or buffering.")]
        [SerializeField] private float loadingAppearanceThreshold = 1.0f;
        [Tooltip("Speed at which the loading icon rotates.")]
        [SerializeField] private float loadingIconRotationSpeed = 5f;
        [Tooltip("Default volume level for the video player (0 to 1).")]
        [SerializeField, Range(0, 1)] private float defaultVolume = 1f;
        [Tooltip("Adjusts the reference resolution of the CanvasScaler, effectively scaling UI controls.")]
        [SerializeField, Range(-0.5f, 0.5f)] private float controlsSize = 0f;
        
        [Header("KEYBOARD SHORTCUTS:")] 
        [Header("-> Avoid using the 'SPACE' key as a shortcut to prevent unexpected behaviours.\n For more details check Docs or HandleKeyboardShortcuts().")]
        [SerializeField] private KeyCode playPauseToggleKey = KeyCode.P;
        [SerializeField] private KeyCode seekForwardKey = KeyCode.RightArrow;
        [SerializeField] private KeyCode seekBackwardKey = KeyCode.LeftArrow;
        [SerializeField] private KeyCode volumeUpKey = KeyCode.UpArrow;
        [SerializeField] private KeyCode volumeDownKey = KeyCode.DownArrow;
        [SerializeField] private KeyCode nextVideoKey = KeyCode.N;
        [SerializeField] private KeyCode prevVideoKey = KeyCode.B;
        [SerializeField] private KeyCode restartVideoKey = KeyCode.R;
        [SerializeField] private KeyCode muteKey = KeyCode.M;
        [SerializeField] private KeyCode fullScreenKey = KeyCode.F;

        // --- MOBILE-ONLY SETTINGS ---
        // Settings specifically for mobile platform orientation.
        [Header("MOBILE-ONLY SETTINGS")]
        [Tooltip("Determines whether the orientation of the device should be modified when the Video Player is in fullscreen mode.")]
        [SerializeField] private bool modifyOrientation = false;
        [Tooltip("Default orientation of the device when the Video Player is not in fullscreen mode. This setting is specific to mobile platforms. ")]
        [SerializeField] private Orientation defaultOrientation;
        [Tooltip("This variable represents the desired orientation of the device when \nthe Video Player enters fullscreen mode. This setting is specific to mobile platforms.")]
        [SerializeField] private Orientation fullscreenOrientation;

        // --- PRIVATE/PROTECTED MEMBERS ---
        // Internal variables to manage player state and cached components.
        private Image playButtonImage;
        private Animator centerPlayButtonAnimator;
        private RectTransform videoPlayerRT;
        private RectTransform displayRT;
        private RenderTexture originalRenderTexture; // The original render texture assigned to the video player
        private AspectRatioFitter displayAspectRatioFitter;

        private bool isStarted;          // True once video playback has begun.
        private bool isPlaying;          // True if the video is currently playing.
        private bool isPrepared;         // True once the video player has finished preparing the video.
        private bool isSeeking;          // True while the video player is seeking to a new time.
        private bool isFrameReady;       // True when a new frame is ready to be displayed.
        private bool isErrorReceived;    // True if an error occurred during video playback.
        private bool isProgressBarHeld;  // True if the user is dragging the progress bar slider.
        private bool isAutoPlay;         // True if the current video clip is set to autoplay.
        private bool isVideoBuffering;   // True if the current video clip is buffering or stuck.
        private bool hasVideoBufferingStarted; // True if the video time is significantly out of sync with the internal clock time.

        private int currentClipIndex = 0; // Index of the currently playing video clip in the array.

        private float controlsAlpha = 1f;          // Current alpha value for controls visibility.
        private float controlsVisibleTimeElapsed = 0f; // Time since controls were last made visible/interacted with.
        private float fullscreenTimeElapsed = 0f;  // Time elapsed during fullscreen transition.
        private float prepareStartTime = 0f;       // Time when the video preparation started.
        private float subtitlesTextContainerInitialYPosition; // Stores the initial Y-position of the UI container that displays subtitles. This is used for animations or positioning adjustments.
        
        private double seekTime;    // The target time for seeking operations.
        private double videoLength; // The total duration of the current video.
        private double previousVideoTime = 0f; // Previous time of video.
        private double previousProgressSliderValue = 0f; // Progress slider value before buffering 
        private double videoStuckDuration = 0f; // Time elapsed since video got stuck.
        private double referenceClock = 0; // An extra clock (in seconds) used to track video playback time, primarily for detecting and correcting playback drift.
        private double driftedDuration = 0; // Accumulates the amount of time (in seconds) during which the video playback has drifted from the reference clock. Used in drift detection logic.

        private string videoLengthStr; // Formatted string of the video's total duration.
        private string elapsedTime;    // Formatted string of the current elapsed video time.

        // Store original RectTransform values to revert from fullscreen.
        // Cannot use simple localscale method for fullscreen toggle
        // as localscale will also shrink the video controls size
        private Vector2 originalPlayerAnchorMin;
        private Vector2 originalPlayerAnchorMax;
        private Vector2 originalPlayerOffsetMin;
        private Vector2 originalPlayerOffsetMax;
        private Vector2 originalDisplayAnchorMin;
        private Vector2 originalDisplayAnchorMax;
        private Vector2 originalDisplayOffsetMin;
        private Vector2 originalDisplayOffsetMax;
        
        private Vector2 refResolution; // Stores the original reference resolution of the CanvasScaler.
        
        private List<SubtitleEntry> subtitleEntries; // A list to store all parsed subtitle entries, loaded from subtitle file. Each entry would contain text and time ranges.
        private SubtitleEntry currentSubtitleEntry; // Holds the subtitle entry that is currently displayed on the screen based on the video's current playback time.

        // --- PROPERTIES ---
        public float videoPlayerVolume
        {
            get { return videoPlayer.GetDirectAudioVolume(0); }
            set
            {
                videoPlayer?.SetDirectAudioVolume(0, Mathf.Clamp(value, 0f, 1f)); // Set volume for audio output 0.
            }
        }

        protected virtual void Awake()
        {
            // Get and cache component references to avoid repeated GetComponent calls.
            playButtonImage = playButton?.GetComponent<Image>();
            centerPlayButtonAnimator = centerPlayButton?.GetComponent<Animator>();
            videoPlayerRT = videoPlayer?.GetComponent<RectTransform>(); 
            displayRT = videoDisplay?.GetComponent<RectTransform>();
            displayAspectRatioFitter = videoDisplay?.GetComponent<AspectRatioFitter>();
        }

        protected virtual void OnEnable()
        {
            // Subscribe to VideoPlayer events for playback state changes.
            if (videoPlayer != null)
            {
                videoPlayer.prepareCompleted += OnPrepareComplete;
                videoPlayer.seekCompleted += OnSeekComplete;
                videoPlayer.loopPointReached += OnLoopPointReached;
                videoPlayer.frameReady += OnFrameReady;
                videoPlayer.errorReceived += OnErrorReceived;
                videoPlayer.frameDropped += OnFrameDropped;
            }

            // Load and prepare the first video clip based on initial settings.
            if (videoClipInfoArray != null && videoClipInfoArray.Count > 0)
            {
                LoadVideoClip(currentClipIndex, isAutoPlay);
            }
            else
            {
                Debug.LogWarning("VideoClipInfoArray is empty. Please assign video clips in the Inspector.");
            }

            // Start coroutine to automatically re-prepare video if an error occurs.
            StartCoroutine(AutoVideoPrepare());
            
            StartCoroutine(CheckForDrifting());
        }

        protected virtual void OnDisable()
        {
            // Unsubscribe from VideoPlayer events to prevent memory leaks and null reference exceptions.
            if (videoPlayer != null)
            {
                videoPlayer.prepareCompleted -= OnPrepareComplete;
                videoPlayer.seekCompleted -= OnSeekComplete;
                videoPlayer.loopPointReached -= OnLoopPointReached;
                videoPlayer.frameReady -= OnFrameReady;
                videoPlayer.errorReceived -= OnErrorReceived;
                videoPlayer.frameReady -= OnFrameReady;
            }

            // Restore default screen orientation when the player is disabled.
            UpdateScreenOrientation(defaultOrientation);

            // Stop all running coroutines to prevent them from continuing after disable.
            StopAllCoroutines();
        }

        protected virtual void Start()
        {
            // Store the original render texture to clone it for each video.
            if (videoPlayer != null)
            {
                originalRenderTexture = videoPlayer.targetTexture;
            }

            // Add listeners to UI buttons. Null-conditional operators ensure no errors if buttons are not assigned.
            videoFrame?.onClick.AddListener(OnFrameClick);
            playButton?.onClick.AddListener(OnPlayButtonClick);
            prevButton?.onClick.AddListener(OnPrevButtonClick);
            nextButton?.onClick.AddListener(OnNextButtonClick);
            replayButton?.onClick.AddListener(ReplayVideo);
            audioButton?.onClick.AddListener(ToggleAudio);
            audioSlider?.onValueChanged.AddListener(OnAudioSliderValueChanged);
            centerPlayButton?.onClick.AddListener(OnCenterButtonClick);
            closeButton?.onClick.AddListener(OnCloseButtonClick);
            fullscreenButton?.onClick.AddListener(OnFullscreenButtonClick);

            if (subtitlesTextContainer != null)
                subtitlesTextContainerInitialYPosition = subtitlesTextContainer.anchoredPosition.y;

            // Initialize volume.
            SetVolume(defaultVolume, true);
            
            // If skipOnDrop is false video player sometimes does not trigger OnFrameDropped callback.
            videoPlayer.skipOnDrop = true; 
            
            // Store original RectTransform properties for the video player container and display.
            // This allows for proper scaling and positioning when toggling fullscreen.
            if (videoPlayerRT != null)
            {
                originalPlayerAnchorMin = videoPlayerRT.anchorMin;
                originalPlayerAnchorMax = videoPlayerRT.anchorMax;
                originalPlayerOffsetMin = videoPlayerRT.offsetMin;
                originalPlayerOffsetMax = videoPlayerRT.offsetMax;

                // If starting in fullscreen, set the video player's RectTransform accordingly.
                if (isFullscreen)
                {
                    videoPlayerRT.anchorMin = Vector2.zero;
                    videoPlayerRT.anchorMax = Vector2.one;
                    videoPlayerRT.offsetMin = Vector2.zero;
                    videoPlayerRT.offsetMax = Vector2.zero;
                }
            }

            if (displayRT != null)
            {
                originalDisplayAnchorMin = displayRT.anchorMin;
                originalDisplayAnchorMax = displayRT.anchorMax;
                originalDisplayOffsetMin = displayRT.offsetMin;
                originalDisplayOffsetMax = displayRT.offsetMax;
            }

            // Update the fullscreen icon based on the initial fullscreen state.
            UpdateFullscreenIcon();

            // Store the reference resolution from the CanvasScaler for dynamic UI scaling.
            if (canvasScaler != null)
            {
                refResolution = canvasScaler.referenceResolution;
            }

            if (Application.isMobilePlatform)
            {
                if (centerPlayButtonAnimator != null)
                {
                    centerPlayButtonAnimator.enabled = !Application.isMobilePlatform;
                }
            }
            else
            {
                if (centerPlayButton != null && videoPlayer != null)
                {
                    centerPlayButton.transform.SetParent(videoPlayer.transform, true);
                }
            }
        }

        protected virtual void Update()
        {
            HandleKeyboardShortcuts();
            
            CheckIfVideoIsBuffering();
            
            // Handle native back button press (e.g., on Android devices).
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnNativeBackButtonPressed();
            }
            
            UpdateTimeText();
            UpdateProgressBar();
            UpdateControlsSize();
            UpdateScreenSize();
            UpdateControlsVisibility();
            UpdateScreenOrientation(isFullscreen ? fullscreenOrientation : defaultOrientation);
            UpdateLoadingIcon();
            UpdateReferenceClock();
            UpdateSubtitles();

            previousVideoTime = videoPlayer.time;
        }

        /// <summary>
        /// Adds a collection of new video clips to the player's existing list of playable videos.
        /// These clips will be appended to the end of the current video list.
        /// </summary>
        /// <param name="videoClipInfoArray">An array of <see cref="VideoClipInfo"/> objects to be added.</param>
        public virtual void AddVideoClips(VideoClipInfo[] videoClipInfoArray)
        {
            // Adds all elements from the provided array to the internal list of video clips.
            this.videoClipInfoArray.AddRange(videoClipInfoArray);
        }
        
        /// <summary>
        /// Updates a reference clock used for tracking video playback time, primarily for drift detection.
        /// This clock only advances when the video is actively playing, not seeking, no errors are present,
        /// and not buffering.
        /// </summary>
        protected virtual void UpdateReferenceClock()
        {
            // Only update the reference clock if the video player is actually playing,
            // not in a seeking state, no errors have occurred, the custom 'isPlaying' flag is true,
            // and the video is not currently buffering.
            if (videoPlayer.isPlaying && !isSeeking && !isErrorReceived && isPlaying && !isVideoBuffering)
            {
                // Advance the reference clock based on the time elapsed since the last frame (Time.deltaTime)
                // and scaled by the current playback speed of the video player.
                referenceClock += (Time.deltaTime * videoPlayer.playbackSpeed);
            }
        }
        
        /// <summary>
        /// A coroutine that continuously checks for video-audio desynchronization (drift) during playback.
        /// If a significant drift is detected, it attempts to resynchronize the video by seeking to the reference clock time.
        /// </summary>
        /// <returns>An IEnumerator for coroutine execution.</returns>
        protected virtual IEnumerator CheckForDrifting()
        {
            // Loop indefinitely while the coroutine is active.
            while (true)
            {
                // Skip drift checking if the video is not prepared, not started, not playing,
                // currently seeking, an error has occurred, or is buffering, or the native player is not playing.
                if (!isPrepared || !isStarted || !isPlaying || isSeeking || isErrorReceived || isVideoBuffering || !videoPlayer.isPlaying)
                {
                    yield return null; // Wait for the next frame before re-checking conditions.
                    continue; // Skip the rest of the loop iteration.
                }

                // Calculate the absolute difference between the native video player's current time
                // and the plugin's internal reference clock.
                double timeDelta = Mathf.Abs((float)(videoPlayer.time - referenceClock));

                // If the time difference (drift) exceeds a threshold (0.5 seconds),
                // it indicates potential desynchronization.
                if (timeDelta > 0.5f)
                {
                    // Accumulate the duration for which the video has been drifted.
                    driftedDuration += Time.deltaTime;
                    
                    // If the accumulated drifted duration exceeds a second threshold (0.2 seconds),
                    // consider the video truly drifted and attempt a resync.
                    if (driftedDuration >= 0.2f)
                    {
                        Debug.Log("Resyncing drifted video!"); // Log message for debugging resync.
                        
                        // Seek the native video player to the time of the reference clock to resynchronize.
                        Seek(referenceClock);

                        // Reset the drifted duration accumulator.
                        driftedDuration = 0;
                        
                        yield return new WaitForSeconds(1); // Wait for 1 second after seeking to allow the player to settle.
                    }
                }
                else
                {
                    // If the time difference is within the acceptable range, reset the drifted duration.
                    driftedDuration = 0.0f;
                }
                
                yield return null; // Wait for the next frame before the next check.
            }
        }
        
        /// <summary>
        /// Detects if the video is stuck or buffering, likely due to a network issue.
        /// </summary>
        protected virtual void CheckIfVideoIsBuffering()
        {
            // Do not use videoPlayer.isPlaying in-place of isPlaying as videoPlayer.isPlaying
            // get auto set to false when video player encounters an error
            if (!isPlaying && !isErrorReceived)
            {
                videoStuckDuration = 0.0f;
                isVideoBuffering = false;
                return;
            }

            double currentVideoTime = videoPlayer.time;

            // Check if the video time hasn't changed significantly since the last frame
            if (Mathf.Approximately((float)currentVideoTime, (float)previousVideoTime))
            {
                videoStuckDuration += Time.deltaTime;

                // If the stuck timer exceeds the threshold, consider the video as stuck
                if (videoStuckDuration >= loadingAppearanceThreshold)
                {
                    isVideoBuffering = true;
                    hasVideoBufferingStarted = true;
                    
                    previousProgressSliderValue = progressBar.value;
                    
                    // Debug.Log("Slider stuck at value: " + GetFormattedTime(videoLength * progressBar.value);
                }
                else
                {
                    isVideoBuffering = false;

                    if (hasVideoBufferingStarted)
                    {
                        hasVideoBufferingStarted = false;
                        
                        referenceClock = (float)(videoLength * previousProgressSliderValue);
                    }
                }
            }
            else
            {
                videoStuckDuration = 0.0f;
            }
        }

        /// <summary>
        /// Handles keyboard input for various video player controls, such as play/pause, seeking, volume,
        /// next/previous video, restart, mute, and fullscreen toggling.
        /// </summary>
        private void HandleKeyboardShortcuts()
        {
            // ⚠️ NOTE: Avoid using the 'SPACE' key directly as a shortcut while UI buttons are interactable.
            //
            // Reason:
            // In Unity, clicking a UI Button makes it the currently selected object.
            // By default, In Unity pressing the 'Space' or 'Enter' key triggers the selected button,
            // since they are configured as alternate inputs for the "Submit" axis in the Input Manager.
            //
            // So if you:
            //   - Click a button with the mouse
            //   - Then press the Space key
            // ... the button is triggered again (due to UI), and if you're also checking for
            // Input.GetKeyDown(KeyCode.Space), your logic will run twice.
            //
            // ✅ To prevent this:
            // 1. After any button click, clear the selected UI object:
            //      EventSystem.current.SetSelectedGameObject(null);
            //    This disables the Space key from re-triggering the UI button.
            //
            // 2. (Optional) If you want to fully disable Space from triggering UI buttons globally,
            //    go to **Project Settings > Input Manager > Submit**, and:
            //      - Remove "space" from the list of Alt Positive Buttons.
            //    This tells Unity to stop using Space as a default activation key for UI buttons.
            //
            // 3. You can manually deselect any currently selected UI element using:
            //     EventSystem.current.SetSelectedGameObject(null);
            // 📌 Recommendation:
            // Avoid using the Space key as a global shortcut if your UI has buttons,
            // or ensure no UI element is selected when handling spacebar input.
            // You can deselect any active 

            // Only process keyboard shortcuts if they are enabled.
            if (!enableKeyboardShortcuts)
            {
                return;
            }
            
            if (Input.GetKeyDown(playPauseToggleKey))
            {
                TogglePlay();
            }

            // Check for seek forward key press.
            if (Input.GetKeyDown(seekForwardKey))
            {
                // Only allow seeking if the video is prepared.
                if (isPrepared)
                {
                    // Calculate a step size equivalent to 5 seconds, normalized to the video's total length.
                    float step = (float)(5f / videoLength); 
                    // Update the progress bar's value without triggering its OnValueChanged event,
                    // clamping the value between 0 and 1.
                    progressBar.SetValueWithoutNotify(Mathf.Clamp01(progressBar.value + step)); 

                    // Seek the video to the new position calculated from the progress bar's value.
                    Seek(videoLength * progressBar.value);    
                }
            }

            // Check for seek backward key press.
            if (Input.GetKeyDown(seekBackwardKey))
            {
                // Only allow seeking if the video is prepared.
                if (isPrepared)
                {
                    // Calculate a step size equivalent to 5 seconds, normalized to the video's total length.
                    float step = (float)(5f / videoLength); 
                    // Update the progress bar's value without triggering its OnValueChanged event,
                    // clamping the value between 0 and 1.
                    progressBar.SetValueWithoutNotify(Mathf.Clamp01(progressBar.value - step)); 

                    // Seek the video to the new position calculated from the progress bar's value.
                    Seek(videoLength * progressBar.value);
                }
            }


            if (Input.GetKeyDown(volumeUpKey))
            {
                SetVolume(videoPlayerVolume + 0.1f, true);
            }

            if (Input.GetKeyDown(volumeDownKey))
            {
                SetVolume(videoPlayerVolume - 0.1f, true);
            }

            if (Input.GetKeyDown(nextVideoKey))
            {
                PlayNextVideoClip(false); // Do not autoplay the next clip by default.
            }

            if (Input.GetKeyDown(prevVideoKey))
            {
                PlayPrevVideoClip(false); // Do not autoplay the previous clip by default.
            }

            if (Input.GetKeyDown(restartVideoKey))
            {
                ReplayVideo();
            }

            if (Input.GetKeyDown(muteKey))
            {
                ToggleAudio();
            }

            if (Input.GetKeyDown(fullScreenKey))
            {
                OnFullscreenButtonClick();
            }
        }

        /// <summary>
        /// Converts a given number of seconds into a formatted time string (e.g., "01:23" or "01:02:03").
        /// The format adapts based on whether the total video length exceeds an hour.
        /// </summary>
        /// <param name="seconds">The time in seconds to format.</param>
        /// <returns>A string representing the formatted time.</returns>
        private string GetFormattedTime(double seconds)
        {
            // Create a TimeSpan object from the input seconds.
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            // Create a TimeSpan object for the total video length (assuming 'videoLength' is available).
            TimeSpan videoTotalTime = TimeSpan.FromSeconds(videoLength);
            
            string formattedTime;
            // Determine the format based on whether the total video duration is 1 hour or more.
            if (videoTotalTime.TotalHours >= 1)
            {
                // Format as hours:minutes:seconds (e.g., "01:23:45").
                formattedTime = time.ToString(@"hh\:mm\:ss"); 
            }
            else
            {
                // Format as minutes:seconds (e.g., "01:23").
                formattedTime = time.ToString(@"mm\:ss");
            }

            return formattedTime;
        }

        /// <summary>
        /// Parses a subtitle file (expected to be in SRT format) located in the StreamingAssets folder.
        /// The parsed subtitles are stored in the 'subtitleEntries' list.
        /// </summary>
        protected virtual void ParseSubtitles(string url)
        {
            // Construct the full path to the subtitle file (e.g., "StreamingAssets/sub.srt").
            string path = Path.Combine(Application.streamingAssetsPath, url);

            // Check if the subtitle file exists at the specified path.
            if (File.Exists(path))
            {
                // If the file exists, read its entire content and parse it using SubtitleParser.
                subtitleEntries = SubtitleParser.ParseSrt(File.ReadAllText(path));
            }
            else
            {
                // Log a warning if the subtitle file is not found.
                Debug.LogWarning("Subtitles file not found at: " + path);
            }
        }
        
        /// <summary>
        /// Callback triggered when the VideoPlayer drops a frame during playback.
        /// </summary>
        /// <param name="source">The VideoPlayer instance that dropped the frame.</param>
        protected virtual void OnFrameDropped(VideoPlayer source)
        {
            
        }
        
        /// <summary>
        /// Callback for when the VideoPlayer has finished preparing the video.
        /// Initializes video properties and updates UI elements accordingly.
        /// </summary>
        /// <param name="vp">The VideoPlayer instance that completed preparation.</param>
        protected virtual void OnPrepareComplete(VideoPlayer vp)
        {
            isPrepared = true;
            UpdateRenderTexture((uint)vp.width, (uint)vp.height);

            videoLength = vp.length;
            videoLengthStr = GetFormattedTime(videoLength);

            if (timeText != null)
            {
                timeText.text = $"00:00 / {videoLengthStr}";
            }

            // If an error occurred previously, attempt to play or pause based on the last state.
            if (isErrorReceived)
            {
                if (isPlaying)
                {
                    Play(); // Attempt to resume playback after an error (e.g., internet connection restored).
                }
                else
                {
                    Pause();
                }
            }

            // If a seek operation was pending before preparation, complete it now.
            // OnSeekComplete is not invoked if you seek before the video is prepared.
            if (isSeeking)
            {
                Seek(videoLength * progressBar.value);
            }

            isErrorReceived = false; // Clear error flag after successful preparation.
        }

        /// <summary>
        /// Callback for when the VideoPlayer has completed a seek operation.
        /// Corrects the seek state and deselects any UI elements.
        /// </summary>
        /// <param name="vp">The VideoPlayer instance that completed the seek.</param>
        protected virtual void OnSeekComplete(VideoPlayer vp)
        {
            // Deselect any UI element(progressSlider)
            EventSystem.current?.SetSelectedGameObject(null);
        }

        /// <summary>
        /// Callback for when the VideoPlayer reaches the end of the current clip.
        /// Proceeds to play the next video in the playlist.
        /// </summary>
        /// <param name="vp">The VideoPlayer instance that reached its loop point.</param>
        protected virtual void OnLoopPointReached(VideoPlayer vp)
        {
            PlayNextVideoClip();
        }

        /// <summary>
        /// Callback for when a new video frame is ready.
        /// Ensures the video player renders the new frame immediately to prevent black screens.
        /// </summary>
        /// <param name="source">The VideoPlayer instance.</param>
        /// <param name="frameIdx">The index of the ready frame.</param>
        protected virtual void OnFrameReady(VideoPlayer source, long frameIdx)
        {
            isFrameReady = true;
            isSeeking = false;

            // Disable future frame ready events until needed again (e.g., after a seek or new video).
            source.sendFrameReadyEvents = false;
        }

        /// <summary>
        /// Callback for when the VideoPlayer encounters an error.
        /// Sets an error flag and prepares for re-preparation.
        /// </summary>
        /// <param name="source">The VideoPlayer instance.</param>
        /// <param name="error">The error message.</param>
        protected virtual void OnErrorReceived(VideoPlayer source, string error)
        {
            Debug.LogError($"Video Player Error: {error}");
            isErrorReceived = true;
            // Set isSeeking to true to force a re-seek to the current time after preparation.
            // This helps restore playback from the correct point if the video time reset due to error.
            isSeeking = true;
        }

        // --- UI BUTTON EVENT HANDLERS ---
        // These methods are called when their corresponding UI buttons are clicked.

        /// <summary>
        /// Handles clicks on the video frame. Toggles controls on mobile, toggles play/pause on desktop.
        /// </summary>
        protected virtual void OnFrameClick()
        {
            if (Application.isMobilePlatform)
            {
                ToggleControlsVisibility();
            }
            else
            {
                TogglePlay();
            }
        }

        /// <summary>
        /// Handles clicks on the main play/pause button.
        /// </summary>
        protected virtual void OnPlayButtonClick()
        {
            TogglePlay();
        }

        /// <summary>
        /// Handles clicks on the previous video button.
        /// </summary>
        protected virtual void OnPrevButtonClick()
        {
            PlayPrevVideoClip(false); // Do not autoplay the previous clip by default.
        }

        /// <summary>
        /// Handles clicks on the next video button.
        /// </summary>
        protected virtual void OnNextButtonClick()
        {
            PlayNextVideoClip(false); // Do not autoplay the next clip by default.
        }

        /// <summary>
        /// Handles value changes on the audio slider.
        /// </summary>
        /// <param name="value">The new slider value (volume).</param>
        protected virtual void OnAudioSliderValueChanged(float value)
        {
            SetVolume(value);
        }

        /// <summary>
        /// Handles clicks on the large center play button.
        /// </summary>
        protected virtual void OnCenterButtonClick()
        {
            TogglePlay();
        }

        /// <summary>
        /// Handles clicks on the close button.
        /// </summary>
        protected virtual void OnCloseButtonClick()
        {
            CloseVideoPlayer();
        }

        /// <summary>
        /// Handles the native back button press. Closes the player if configured.
        /// </summary>
        protected virtual void OnNativeBackButtonPressed()
        {
            if (nativeBackButtonClosesPlayer)
            {
                CloseVideoPlayer();
            }
        }

        /// <summary>
        /// Handles clicks on the fullscreen toggle button.
        /// </summary>
        protected virtual void OnFullscreenButtonClick()
        {
            ToggleFullscreen();
        }

        /// <summary>
        /// Called by the EventTrigger component when the progress bar slider's value changes (e.g., dragged by user).
        /// Seeks the video to the corresponding time.
        /// </summary>
        public virtual void OnProgressBarValueChanged()
        {
            // Calculate the target seek time based on the slider's value.
            Seek(videoLength * progressBar.value);
        }

        /// <summary>
        /// Restarts the current video from the beginning.
        /// </summary>
        protected virtual void ReplayVideo()
        {
            Seek(0); // Seek to the beginning of the video.
        }

        // --- UI UPDATE METHODS ---
        // These methods are called periodically (e.g., in Update) to refresh UI elements.

        /// <summary>
        /// Updates the time display text (current time / total duration).
        /// </summary>
        protected virtual void UpdateTimeText()
        {
            if (timeText == null) return;

            if (isPrepared)
            {
                // Calculate elapsed time based on progress bar value (more accurate during seeking).
                double time = videoLength * progressBar.value;

                if (!double.IsNaN(time))
                {
                    elapsedTime = GetFormattedTime(time);
                }

                // If at the very end of the video, ensure the elapsed time matches total duration.
                if (!isErrorReceived && videoPlayer.frame == (long)(videoPlayer.frameCount - 1))
                {
                    elapsedTime = videoLengthStr;
                }

                timeText.text = $"{elapsedTime} / {videoLengthStr}";
            }
        }

        /// <summary>
        /// Updates the video progress bar slider.
        /// </summary>
        protected virtual void UpdateProgressBar()
        {
            // Update the progress bar only if video is prepared, not seeking, and not being held by user.
            if (isPrepared && !isSeeking && !isProgressBarHeld && driftedDuration < 0.05f)
            {
                float progress = (float)(videoPlayer.time / videoLength);

                if (!float.IsNaN(progress))
                {
                    progressBar?.SetValueWithoutNotify(progress); // Update without triggering OnValueChanged event.
                }
            }
        }

        /// <summary>
        /// Dynamically adjusts the size of the UI controls based on the `ControlsSize` setting.
        /// </summary>
        protected virtual void UpdateControlsSize()
        {
            if (canvasScaler != null)
            {
                // Adjust the canvas's reference resolution to scale UI elements.
                canvasScaler.referenceResolution = refResolution - (refResolution * controlsSize);
            }
        }

        /// <summary>
        /// Smoothly animates the video player's RectTransform between normal and fullscreen modes.
        /// </summary>
        protected virtual void UpdateScreenSize()
        {
            if (videoPlayerRT == null) return;

            fullscreenTimeElapsed += Time.deltaTime;
            // Calculate interpolation factor, clamping it to ensure it stays within 0 and 1.
            float t = fullscreenTimeElapsed / fullscreenAnimSpeed;

            if (isFullscreen)
            {
                if (fullscreenTimeElapsed < fullscreenAnimSpeed)
                {
                    // Lerp towards fullscreen anchors and offsets.
                    videoPlayerRT.anchorMin = Vector2.Lerp(videoPlayerRT.anchorMin, Vector2.zero, t);
                    videoPlayerRT.anchorMax = Vector2.Lerp(videoPlayerRT.anchorMax, Vector2.one, t);
                    videoPlayerRT.offsetMin = Vector2.Lerp(videoPlayerRT.offsetMin, Vector2.zero, t);
                    videoPlayerRT.offsetMax = Vector2.Lerp(videoPlayerRT.offsetMax, Vector2.zero, t);
                }
            }
            else
            {
                if (fullscreenTimeElapsed < fullscreenAnimSpeed)
                {
                    // Lerp back to original anchors and offsets.
                    videoPlayerRT.anchorMin = Vector2.Lerp(videoPlayerRT.anchorMin, originalPlayerAnchorMin, t);
                    videoPlayerRT.anchorMax = Vector2.Lerp(videoPlayerRT.anchorMax, originalPlayerAnchorMax, t);
                    videoPlayerRT.offsetMin = Vector2.Lerp(videoPlayerRT.offsetMin, originalPlayerOffsetMin, t);
                    videoPlayerRT.offsetMax = Vector2.Lerp(videoPlayerRT.offsetMax, originalPlayerOffsetMax, t);
                }
            }
        }

        /// <summary>
        /// Updates the icon of the fullscreen button based on the current fullscreen state.
        /// </summary>
        protected virtual void UpdateFullscreenIcon()
        {
            if (fullscreenButton?.GetComponent<Image>() == null) return;

            // Set the appropriate sprite for fullscreen enter or exit.
            fullscreenButton.GetComponent<Image>().sprite = isFullscreen ? fullscreenExitSprite : fullscreenEnterSprite;
        }

        /// <summary>
        /// Manages the visibility of the video player controls, including auto-hiding.
        /// </summary>
        protected virtual void UpdateControlsVisibility()
        {
            if (controls == null) return;

            bool shouldShowControls = false;

            // Determine if controls should be shown based on platform-specific input or state.
            if (Application.isMobilePlatform)
            {
                // On mobile, show controls if the bottom control bar is touched/moused over,
                // or if the video hasn't started or an error occurred.
                shouldShowControls = RectTransformUtility.RectangleContainsScreenPoint(bottomControls, Input.mousePosition) || !isStarted || isErrorReceived;
            }
            else
            {
                // On desktop, show controls if mouse moves, a mouse button is pressed,
                // or if the bottom control bar is moused over, or if video hasn't started or error occurred.
                shouldShowControls = Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || Input.GetMouseButton(0) ||
                                     RectTransformUtility.RectangleContainsScreenPoint(bottomControls, Input.mousePosition) ||
                                     !isStarted || isErrorReceived;
            }

            if (shouldShowControls)
            {
                ShowControls(); // Reset auto-hide timer and set controls to fully visible.
            }

            // If auto-hide delay is met, set controls to fade out.
            if (controlsVisibleTimeElapsed >= controlsAutoHideDelay)
            {
                controlsAlpha = 0;
            }

            // Smoothly interpolate the alpha of the CanvasGroup to fade controls.
            controls.alpha = Mathf.MoveTowards(controls.alpha, controlsAlpha, controlsFadeSpeed * Time.deltaTime);

            if (subtitlesTextContainer != null)
            {
                subtitlesTextContainer.anchoredPosition = new Vector2(subtitlesTextContainer.anchoredPosition.x,
                    Mathf.Clamp(subtitlesTextContainerInitialYPosition * controls.alpha, 50, subtitlesTextContainerInitialYPosition));
            }
            
            controlsVisibleTimeElapsed += Time.deltaTime; // Increment timer.
        }

        /// <summary>
        /// Sets the screen orientation for mobile platforms.
        /// </summary>
        /// <param name="orientation">The target screen orientation.</param>
        protected virtual void UpdateScreenOrientation(Orientation orientation)
        {
            // Only apply orientation changes on mobile platforms and if modifyOrientation is enabled.
            if (!Application.isMobilePlatform || !modifyOrientation)
            {
                return;
            }

            // Set Unity's Screen.orientation based on the chosen enum.
            switch (orientation)
            {
                case Orientation.AutoRotation:
                    Screen.orientation = ScreenOrientation.AutoRotation;
                    break;
                case Orientation.Potrait:
                    Screen.orientation = ScreenOrientation.Portrait;
                    break;
                case Orientation.Landscape:
                    // For general landscape, determine if it's left or right based on device orientation.
                    if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
                        Screen.orientation = ScreenOrientation.LandscapeRight;
                    else
                        Screen.orientation = ScreenOrientation.LandscapeLeft;
                    break;
                case Orientation.LandscapeLeft:
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    break;
                case Orientation.LandscapeRight:
                    Screen.orientation = ScreenOrientation.LandscapeRight;
                    break;
            }
        }
        
        /// <summary>
        /// Manages the visibility and rotation of the loading icon.
        /// </summary>
        protected virtual void UpdateLoadingIcon()
        {
            if (loadingIcon == null) return;
            
            loadingIcon.gameObject.SetActive(isVideoBuffering);
            
            // Rotate the loading icon for visual feedback.
            loadingIcon.transform.Rotate(-Vector3.forward * (loadingIconRotationSpeed * 100 * Time.deltaTime));
        }

        protected virtual void UpdateSubtitles()
        {
            if (subtitleEntries == null || subtitleEntries.Count == 0 || subtitlesText == null)
            {
                subtitlesTextContainer.gameObject.SetActive(false);
                return;
            }
            
            if (subtitlesTextContainer ==null)
            {
                return;
            }
            
            var currentTime = TimeSpan.FromSeconds(videoPlayer.time);

            // Only update if the subtitle changed
            foreach (SubtitleEntry subtitle in subtitleEntries)
            {
                if (currentTime >= subtitle.startTime && currentTime <= subtitle.endTime)
                {
                    if (currentSubtitleEntry != subtitle)
                    {
                        currentSubtitleEntry = subtitle;
                        subtitlesTextContainer.gameObject.SetActive(true);
                        subtitlesText.text = currentSubtitleEntry.text;
                    }
                    return;
                }
            }

            // If no subtitle matched
            if (currentSubtitleEntry != null)
            {
                subtitlesTextContainer.gameObject.SetActive(false);
                subtitlesText.text = null;
            }
        }
        
        // --- PLAYER CONTROL METHODS ---

        /// <summary>
        /// Toggles the visibility state of the controls (fade in/out).
        /// </summary>
        public virtual void ToggleControlsVisibility()
        {
            // Toggle the target alpha for controls.
            controlsAlpha = (controlsAlpha == 0) ? 1 : 0;
            controlsVisibleTimeElapsed = 0; // Reset timer to prevent immediate re-hiding.
        }

        /// <summary>
        /// Forces the controls to be fully visible and resets the auto-hide timer.
        /// </summary>
        public virtual void ShowControls()
        {
            controlsAlpha = 1;
            controlsVisibleTimeElapsed = 0;
        }

        /// <summary>
        /// Toggles between playing and pausing the video.
        /// </summary>
        public virtual void TogglePlay()
        {
            if (isPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        /// <summary>
        /// Plays the current video.
        /// </summary>
        /// <param name="playCenterButtonAnim">Whether to play the center play button animation.</param>
        public virtual void Play(bool playCenterButtonAnim = true)
        {
            if (!isStarted)
            {
                isStarted = true; // Mark as started once play is initiated.
            }

            isPlaying = true;
            videoPlayer?.Play();

            SetPauseSprite(); // Change the play button icon to pause.

            if (playCenterButtonAnim)
            {
                centerPlayButtonAnimator?.Play("Toggle Play", 0, 0); // Play animation if an animator is present.
            }

            thumbnailImage?.gameObject.SetActive(false); // Hide the thumbnail when playing.

            ShowControls(); // Ensure controls are visible when playback starts.
        }

        /// <summary>
        /// Pauses the current video.
        /// </summary>
        /// <param name="playCenterButtonAnim">Whether to play the center play button animation.</param>
        public virtual void Pause(bool playCenterButtonAnim = true)
        {
            isPlaying = false;
            videoPlayer?.Pause();

            SetPlaySprite(); // Change the pause button icon to play.

            if (playCenterButtonAnim)
            {
                centerPlayButtonAnimator?.Play("Toggle Play", 0, 0); // Play animation if an animator is present.
            }

            ShowControls(); // Ensure controls are visible when paused.
        }

        /// <summary>
        /// Sets the play/pause button sprites to indicate "Play" state.
        /// </summary>
        protected virtual void SetPlaySprite()
        {
            if (playButtonImage != null)
            {
                playButtonImage.sprite = playSprite;
            }

            if (centerPlayButtonImage != null)
            {
                centerPlayButtonImage.sprite = playSprite;
                // Adjust position for the play icon (often has a triangular shape).
                centerPlayButtonImage.rectTransform.localPosition = new Vector3(7.5f, 0, 0);
            }
        }

        /// <summary>
        /// Sets the play/pause button sprites to indicate "Pause" state.
        /// </summary>
        protected virtual void SetPauseSprite()
        {
            if (playButtonImage != null)
            {
                playButtonImage.sprite = pauseSprite;
            }

            if (centerPlayButtonImage != null)
            {
                centerPlayButtonImage.sprite = pauseSprite;
                // Reset position for the pause icon.
                centerPlayButtonImage.rectTransform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// Toggles the audio mute state.
        /// </summary>
        public virtual void ToggleAudio()
        {
            // Toggle mute
            videoPlayer.SetDirectAudioMute(0, !videoPlayer.GetDirectAudioMute(0));
            
            SetAudioMuteImageActive(videoPlayer.GetDirectAudioMute(0));
            
            if (!videoPlayer.GetDirectAudioMute(0) && videoPlayer.GetDirectAudioVolume(0) < 0.05f)
            {
                SetVolume(0.05f, true);
            }
        }

        /// <summary>
        /// Sets the video player's audio volume.
        /// </summary>
        /// <param name="value">The desired volume (0 to 1).</param>
        /// <param name="setAudioSliderValue">If true, the audio slider's value will also be updated.</param>
        public virtual void SetVolume(float value, bool setAudioSliderValue = false)
        {
            videoPlayerVolume = value;

            // Cannot use videoPlayerVolume here as videoPlayer.SetDirectAudioVolume() does not set volume instantly
            // so if you use videoPlayer.GetDirectAudioVolume() it return old volume
            if (value <= 0)
            {
                videoPlayer.SetDirectAudioMute(0, true);

                SetAudioMuteImageActive(true);
            }
            else
            {
                videoPlayer.SetDirectAudioMute(0, false);
                
                SetAudioMuteImageActive(false);
            }

            // Update the audio slider's visual position.
            if (audioSlider != null && setAudioSliderValue)
            {
                audioSlider.SetValueWithoutNotify(value);
            }
        }

        private void SetAudioMuteImageActive(bool isActive)
        {
            if (audioMuteImage != null)
            {
                audioMuteImage.enabled = isActive;
            }
        }

        /// <summary>
        /// Plays the next video clip in the `videoClipInfoArray`.
        /// Loops back to the beginning if the end is reached.
        /// </summary>
        /// <param name="autoPlay">If true, the next video will automatically start playing.</param>
        public virtual void PlayNextVideoClip(bool autoPlay = false)
        {
            currentClipIndex = (currentClipIndex + 1) % videoClipInfoArray.Count; // Cycle through clips.
            LoadVideoClip(currentClipIndex, autoPlay);
        }

        /// <summary>
        /// Plays the previous video clip in the `videoClipInfoArray`.
        /// Loops to the end if the beginning is reached.
        /// </summary>
        /// <param name="autoPlay">If true, the previous video will automatically start playing.</param>
        public virtual void PlayPrevVideoClip(bool autoPlay = false)
        {
            currentClipIndex--;
            // Handle wrapping around to the end of the array.
            if (currentClipIndex < 0)
            {
                currentClipIndex = videoClipInfoArray.Count - 1;
            }
            LoadVideoClip(currentClipIndex, autoPlay);
        }

        /// <summary>
        /// Toggles the fullscreen state of the video player.
        /// This method updates the internal fullscreen flag, resets the fullscreen animation timer,
        /// and updates the visual icon of the fullscreen button.
        /// </summary>
        public virtual void ToggleFullscreen()
        {
            isFullscreen = !isFullscreen; // Invert the boolean flag to toggle fullscreen state.
            fullscreenTimeElapsed = 0;    // Reset the timer used for fullscreen transition animations.
            UpdateFullscreenIcon();       // Update the icon displayed on the fullscreen button to reflect the new state.
        }
        
        /// <summary>
        /// Changes the currently playing video clip.
        /// Stops the current video, prepares the new one, and updates UI.
        /// </summary>
        /// <param name="videoClipIndex">The <see cref="index"/> of the video to load.</param>
        /// <param name="autoPlay">If true, the new video will automatically start playing after preparation.</param>
        public virtual void LoadVideoClip(int videoClipIndex, bool autoPlay = true)
        {
            // Stop current playback and prepare for new video.
            videoPlayer?.Stop();
            videoPlayer.sendFrameReadyEvents = true; // Enable frame ready events for new video.

            // Release the old render texture to free up memory and prevent visual artifacts.
            // A new render texture will be created in UpdateRenderTexture.
            videoPlayer.targetTexture?.Release();

            SetPlaySprite(); // Reset play/pause button to "Play" state.

            // Reset all playback state flags.
            isStarted = false;
            isPlaying = false;
            isPrepared = false;
            isSeeking = false;
            isFrameReady = false;
            isErrorReceived = false;
            isProgressBarHeld = false;
            isVideoBuffering = false;
            isAutoPlay = autoPlay;

            seekTime = 0;
            videoStuckDuration = 0;
            previousVideoTime = 0;
            referenceClock = 0;
            driftedDuration = 0;
            progressBar?.SetValueWithoutNotify(0); // Reset progress bar to 0.

            currentSubtitleEntry = null;
            subtitleEntries = null;

            currentClipIndex = videoClipIndex;
            
            var videoClipInfo = videoClipInfoArray[currentClipIndex];
            
            // Update time display with default or provided duration.
            if (timeText != null)
            {
                timeText.text = $"00:00 / {(string.IsNullOrWhiteSpace(videoClipInfo.duration) ? defaultVideoDuration : videoClipInfo.duration)}";
            }

            if (videoNameText != null)
            {
                videoNameText.text = videoClipInfo.videoName;
            }
            
            if (subtitlesTextContainer != null)
                subtitlesTextContainer.gameObject.SetActive(false);

            // Handle thumbnail display.
            if (videoClipInfo.thumbnail != null)
            {
                if (thumbnailImage != null)
                {
                    thumbnailImage.gameObject.SetActive(true);
                    thumbnailImage.sprite = videoClipInfo.thumbnail;
                    thumbnailImage.preserveAspect = preserveVideoAspectRatio;
                }
            }
            else
            {
                if (thumbnailImage != null)
                    thumbnailImage.gameObject.SetActive(false); // Hide thumbnail if none is provided.
            }

            prepareStartTime = Time.time; // Record start time for loading icon threshold.

            centerPlayButtonAnimator?.Play("Default", 0, 0); // Reset center play button animation.

            // Set video player source based on VideoClipInfo.
            switch (videoClipInfo.source)
            {
                case VideoClipSource.VideoClip:
                    videoPlayer.source = VideoSource.VideoClip;
                    videoPlayer.clip = videoClipInfo.clip;
                    break;
                case VideoClipSource.URL:
                    videoPlayer.source = VideoSource.Url;
                    videoPlayer.url = videoClipInfo.url;
                    break;
                case VideoClipSource.StreamingAssets:
                    videoPlayer.source = VideoSource.Url;
                    // Combine StreamingAssets path with the provided URL for local files.
                    videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoClipInfo.url);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(videoClipInfo.subtitlesUrl))
                ParseSubtitles(videoClipInfo.subtitlesUrl);

            videoPlayer.Prepare(); // Start preparing the new video.

            // Play or pause immediately based on autoPlay setting.
            if (isAutoPlay)
            {
                Play(isAutoPlay);
            }
            else
            {
                Pause(isAutoPlay);
            }
        }

        /// <summary>
        /// Creates or updates the Render Texture used for video display, adjusting its resolution
        /// and aspect ratio based on video properties and settings.
        /// </summary>
        /// <param name="width">The width of the video frame.</param>
        /// <param name="height">The height of the video frame.</param>
        protected virtual void UpdateRenderTexture(uint width, uint height)
        {
            // Instantiate a new Render Texture from the original to ensure a clean slate
            // and correct dimensions for the new video.
            RenderTexture rt = Instantiate(originalRenderTexture);
            rt.width = (int)width;
            rt.height = (int)height;

            videoPlayer.targetTexture = rt; // Assign the new render texture to the VideoPlayer.
            videoDisplay.texture = rt;     // Assign the same render texture to the RawImage for display.

            if (preserveVideoAspectRatio)
            {
                if (displayAspectRatioFitter != null)
                {
                    displayAspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                    displayAspectRatioFitter.aspectRatio = (float)width / (float)height;
                }
            }
            else
            {
                // If aspect ratio is not preserved, reset the RawImage's RectTransform to its original state.
                if (displayAspectRatioFitter != null)
                {
                    displayAspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.None;
                }

                if (displayRT != null)
                {
                    displayRT.anchorMin = originalDisplayAnchorMin;
                    displayRT.anchorMax = originalDisplayAnchorMax;
                    displayRT.offsetMin = originalDisplayOffsetMin;
                    displayRT.offsetMax = originalDisplayOffsetMax;
                }
            }
        }

        /// <summary>
        /// Seeks the video to a specific time.
        /// </summary>
        /// <param name="time">The target time in seconds to seek to.</param>
        public virtual void Seek(double time)
        {
            // If the video hasn't started playing yet, initiate playback.
            if (!isStarted)
            {
                Play();
            }

            // Only update prepareStartTime if we are initiating a new seek.
            if (!isSeeking)
            {
                prepareStartTime = Time.time;
            }

            time = Mathf.Clamp((float)time, (float)0, (float)videoPlayer.length);

            referenceClock = (float)time;
            
            float progress = (float)(time / videoLength);

            if (!float.IsNaN(progress))
            {
                progressBar?.SetValueWithoutNotify(progress); // Update without triggering OnValueChanged event.
            }
            
            isSeeking = true;       // Set seeking flag.
            seekTime = time;        // Store target seek time.
            videoPlayer.time = seekTime; // Set VideoPlayer's time.

            isFrameReady = false;           // Reset frame ready flag.
            videoPlayer.sendFrameReadyEvents = true; // Enable frame ready events for post-seek update.
        }

        /// <summary>
        /// Called by the EventTrigger component when the progress bar is held or released.
        /// Updates the `isProgressBarHeld` flag.
        /// </summary>
        /// <param name="isHeld">True if the progress bar is currently being dragged, false otherwise.</param>
        public virtual void SetProgressBarStatus(bool isHeld)
        {
            isProgressBarHeld = isHeld;
        }

        /// <summary>
        /// Coroutine that periodically attempts to re-prepare the video player if an error is detected.
        /// This helps recover from network interruptions or other playback issues.
        /// </summary>
        protected virtual IEnumerator AutoVideoPrepare()
        {
            while (true)
            {
                if (isErrorReceived)
                {
                    yield return new WaitForSeconds(reprepareInterval); // Wait before attempting re-preparation.
                    videoPlayer?.Prepare(); // Attempt to prepare the video again.
                }
                else
                {
                    yield return null; // Continue to the next frame.
                }
            }
        }

        /// <summary>
        /// Activates the video player container, making it visible.
        /// </summary>
        public virtual void ShowVideoPlayer()
        {
            videoPlayerContainer?.SetActive(true);
        }

        /// <summary>
        /// Deactivates the video player container, hiding it.
        /// </summary>
        public virtual void CloseVideoPlayer()
        {
            // Optionally, you might want to stop the video before closing the player.
            // videoPlayer?.Stop();
            videoPlayerContainer?.SetActive(false);
        }

        /// <summary>
        /// Handles application pause/resume events, particularly for mobile.
        /// Pauses the video when the application goes into the background.
        /// </summary>
        /// <param name="isApplicationPaused">True if the application is paused, false otherwise.</param>
        protected void OnApplicationPause(bool isApplicationPaused)
        {
            if (Application.isMobilePlatform)
            {
                if (isApplicationPaused)
                {
                    Pause(false); // Pause without playing the center button animation.
                }
            }

            ShowControls(); // Always show controls on application resume for better UX.
        }
    }
}