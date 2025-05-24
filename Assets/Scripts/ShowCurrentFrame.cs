using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class ShowCurrentFrame : MonoBehaviour
{
    [SerializeField]
     PlayableDirector playableDirector;
    [SerializeField]
     TextMeshProUGUI frameText;
    [SerializeField]
     int frameRate = 30;

    void Update()
    {
        int currentFrame = (int)(playableDirector.time * frameRate);
        frameText.text = "Frame: " + currentFrame;
    }
}