using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for Button component

namespace NareshBisht
{
    /// <summary>
    /// This script demonstrates how to integrate and use the Cross-Platform Video Player Plugin.
    /// It handles the instantiation of the video player prefab and the loading of initial video clips.
    /// </summary>
    public class VideoPlayerDemo : MonoBehaviour
    {
        [Header("ASSET REFERENCES")] 
        [Tooltip("Assign the Video Player Canvas Prefab here. This prefab contains the VideoPlayerCrossPlatform component.")]
        [SerializeField]
        private GameObject videoPlayerPrefab; // Reference to the video player prefab to be instantiated.

        [Header("SCENE REFERENCES")] 
        [Tooltip("Assign the UI Button that will be used to show/activate the video player. Optional.")]
        [SerializeField] 
        private Button videoPlayerShowButton; // Reference to a UI Button that triggers the video player to show/hide.

        [Header("SETTINGS")]
        [Tooltip("If true, the first video will start playing automatically after it is loaded.")]
        [SerializeField] // Added [SerializeField] to expose in the Inspector
        private bool autoPlayVideo = false; // Determines if the first loaded video should start playing automatically.
        
        [Header("VIDEOS LIST")] 
        [Tooltip("Array of video clips to be played in the player. Configure these in the Inspector.")]
        [SerializeField] 
        private List<VideoClipInfo> videoClipInfoArray; // A list of VideoClipInfo objects, defining the videos to be loaded.

        private VideoPlayerCrossPlatform videoPlayerInstance; // Stores a reference to the instantiated VideoPlayerCrossPlatform component.
        
        /// <summary>
        /// Called when the script instance is being loaded.
        /// This method initializes the video player by instantiating the prefab,
        /// adding the specified video clips, loading the first clip, and setting up the show button.
        /// </summary>
        private void Start()
        {
            // --- Input Validation ---
            // Check if the video player prefab is assigned in the Inspector.
            if (videoPlayerPrefab == null)
            {
                Debug.LogError("VideoPlayerDemo: No video player prefab assigned. Please assign the 'Video Player Canvas' prefab in the Inspector.", this);
                return; // Stop execution if the prefab is missing to prevent NullReferenceExceptions.
            }
            
            // Check if any video clips are configured in the array.
            if (videoClipInfoArray == null || videoClipInfoArray.Count == 0)
            {
                Debug.LogError("VideoPlayerDemo: No Clips Found! Please add video clips to be played in the player in the Inspector.", this);
                return; // Stop execution if no clips are provided.
            }
            
            // --- Player Instantiation and Initialization ---
            // Instantiate the video player prefab. 
            // GetComponentInChildren is used to find the VideoPlayerCrossPlatform component,
            // including inactive children (due to 'true' argument), 
            // assuming it might be nested within the prefab's hierarchy and potentially inactive.
            videoPlayerInstance = Instantiate(videoPlayerPrefab).GetComponentInChildren<VideoPlayerCrossPlatform>(true);
            
            // Check if the VideoPlayerCrossPlatform component was successfully found.
            if (videoPlayerInstance == null)
            {
                Debug.LogError("VideoPlayerDemo: Could not find 'VideoPlayerCrossPlatform' component in the instantiated prefab. " +
                               "Ensure the prefab contains this component or its children.", this);
                return;
            }

            // Add the configured video clips from the Inspector to the video player instance.
            // The list is converted to an array as per the AddVideoClips method's signature.
            videoPlayerInstance.AddVideoClips(videoClipInfoArray.ToArray());
            
            // Activates the video player container, making it visible.
            // This step is performed before loading the video clip because
            // Unity's native VideoPlayer component requires its GameObject
            // to be active and enabled in the hierarchy for proper video preparation and playback.
            // This ensures that the video player UI is also visible while the video is loading.
            videoPlayerInstance.ShowVideoPlayer();
            
            // Load and prepare the first video clip in the list (index 0).
            // The 'autoPlayVideo' boolean determines if the video will start playing immediately after loading.
            videoPlayerInstance.LoadVideoClip(0, autoPlayVideo);

            // --- UI Button Setup ---
            // Check if a show button is assigned before adding a listener.
            if (videoPlayerShowButton != null)
            {
                // Add a listener to the button's onClick event.
                // When the button is clicked, a lambda expression will execute,
                // calling the videoPlayerInstance.ShowVideoPlayer() method.
                videoPlayerShowButton.onClick.AddListener(() =>
                {
                    videoPlayerInstance.ShowVideoPlayer();
                });
            }
            else
            {
                // Log a warning if the show button is not assigned, as its intended functionality won't work.
                Debug.LogWarning("VideoPlayerDemo: No 'Video Player Show Button' assigned. " + 
                                 "The video player will not be made visible by the button click, " +
                                 "but it is made visible via ShowVideoPlayer() method call.");
            }
        }
    }
}