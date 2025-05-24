using UnityEngine;
using TMPro;
using UnityEngine.Playables;

public class DisplayClipName : MonoBehaviour
{
    [SerializeField]
     Animator animator;
    [SerializeField]
     TextMeshProUGUI animationNameText;
    [SerializeField]
     PlayableDirector playableDirector;

    void Update()
    {
        if (animator != null && animationNameText != null && playableDirector != null)
        {
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                animationNameText.text = clipInfo[0].clip.name;
            }
        }
    }
}