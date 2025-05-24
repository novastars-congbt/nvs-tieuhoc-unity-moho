using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//Dùng cho nút bắt đầu của PopUpCanvas
public class TimelineActivation : MonoBehaviour
{
    [SerializeField] PlayableDirector timeline;
    [SerializeField] GameObject startPanel;
    void Start()
    {
        //Dừng timeline và hiện panel
        timeline.Pause();
        startPanel.SetActive(true);
    }

    public void ActivateTimeline()
    {
        //Chạy timeline và ẩn panel
        timeline.Play();
        startPanel.SetActive(false);
    }
}
