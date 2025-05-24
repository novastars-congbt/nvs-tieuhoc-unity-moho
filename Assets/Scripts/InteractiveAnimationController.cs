using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Chức năng của hoạt hình tương tác: Trẻ lựa chọn timeline mình muốn
public class InteractiveAnimationController : MonoBehaviour
{
    [SerializeField] Button[] button;
    [SerializeField] GameObject[] timeline;

    public void SetTimeline(int index)
    {
        foreach (var but in button) 
        {
            but.gameObject.SetActive(false);
        }
        timeline[0].SetActive(false);
        timeline[index].SetActive(true);
    }
}
