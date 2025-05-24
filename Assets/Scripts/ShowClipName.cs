using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

public class ShowClipName : MonoBehaviour
{
    [SerializeField]
     TextMeshProUGUI animationNameText;
    [SerializeField]
     PlayableDirector playableDirector;
    [SerializeField]
     List<string> clipNames = new List<string>();

    private int previousClipIndex = -1;

    private void Start()
    {
        if (playableDirector != null)
        {
            PopulateClipNames();
        }
    }

    void Update()
    {
        if (playableDirector != null && animationNameText != null && clipNames.Count > 0)
        {
            double currentTime = playableDirector.time;
            int currentClipIndex = FindCurrentClipIndex(currentTime);

            if (currentClipIndex != -1 && currentClipIndex != previousClipIndex)
            {
                animationNameText.text = clipNames[currentClipIndex];
                previousClipIndex = currentClipIndex;
            }
        }
    }

    private void PopulateClipNames()
    {
        foreach (var output in playableDirector.playableAsset.outputs)
        {
            var track = output.sourceObject as AnimationTrack;

            if (track != null)
            {
                foreach (var clip in track.GetClips())
                {
                    clipNames.Add(clip.displayName);
                }
            }
        }
    }

    private int FindCurrentClipIndex(double time)
    {
        int clipIndex = 0;

        foreach (var output in playableDirector.playableAsset.outputs)
        {
            var track = output.sourceObject as AnimationTrack;

            if (track != null)
            {
                foreach (var clip in track.GetClips())
                {
                    if (time >= clip.start && time <= clip.end)
                    {
                        return clipIndex;
                    }
                    clipIndex++;
                }
            }
        }

        return -1;
    }
}