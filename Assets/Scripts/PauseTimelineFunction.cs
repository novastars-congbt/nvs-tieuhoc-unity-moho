using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PauseTimelineFunction : MonoBehaviour
{
    [SerializeField]
     PlayableDirector timeline;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (timeline.state == PlayState.Playing)
            {
                timeline.Pause();
            }
            else
            {
                timeline.Resume();
            }
        }
    }
}
