using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Diamondhenge.HengeVideoPlayer
{
    [System.Serializable]
    public class VideoToolbar
    {
        [Tooltip("The delay is seconds after playing before the toolbar fades.")]
        public float fadeDelay;
        [Tooltip("The toggle button for playing and pausing the video.")]
        public ToggleButton playButton;
        [Tooltip("The toggle button for muting and unmuting the video's audio.")]
        public ToggleButton volumeButton;
        public void Initialize()
        {
            //playButton.Initialize();
            volumeButton.Initialize();
        }
    }

    /// <summary>
    /// A data structure that holds data for toggle buttons.
    /// These buttons switch between two icons and states.
    /// </summary>
    [System.Serializable]
    public class ToggleButton
    {
        [Tooltip("The image module of the button that will toggle.")]
        public Image imageModule;
        [Tooltip("The sprite to use when this button is in its ON state.")]
        public Sprite onSprite;
        [Tooltip("The sprite to use when this button is in its OFF state.")]
        public Sprite offSprite;
        [HideInInspector]
        public UnityEvent<bool> onToggle;

        protected bool isOn = false;

        /// <summary>
        /// Initializes this toggle button's events during runtime.
        /// </summary>
        public void Initialize()
        {
            imageModule.GetComponent<Button>().onClick.AddListener(() => ToggleState());
        }

        /// <summary>
        /// Sets whether this button is in it's ON state or OFF state.
        /// </summary>
        /// <param name="on"></param>
        public void SetButtonState(bool on)
        {
            isOn = on;
            imageModule.sprite = !on ? onSprite : offSprite;
            onToggle.Invoke(on);
        }
        /// <summary>
        /// Toggles this button state between its ON state or OFF state.
        /// </summary>
        public void ToggleState()
        {
            SetButtonState(!isOn);
        }
    }
}
