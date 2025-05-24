using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomNumberPopUp : PopupProperties
{
    Tween tween;
    [SerializeField]
    private int value;
    [SerializeField]
    private int endValue;
    [SerializeField]
    private TextMeshProUGUI textValue;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] audioClip;

    private void OnEnable()
    {
        
    }

    int count = 0;
    bool isSpinning = false;

    public void BtnSpin()
    {
        if (isSpinning) return;
        isSpinning = true;
        endValue = Random.Range(1, SpinPopUp.instance.total + 1);
        audioSource.PlayOneShot(audioClip[0]);
        tween = DOTween.To(() => value, x => value = x, endValue, 3f)
            .OnUpdate(() =>
            {
                count++;
                if (count >= 10)
                {
                    count = 0;
                    value = Random.Range(1, SpinPopUp.instance.total + 1);
                    textValue.text = "" + value;
                }
                
            })
            .OnComplete(() =>
            {
                audioSource.PlayOneShot(audioClip[1]);
                value = endValue;
                textValue.text = "" + value;
                tween.Kill();
                isSpinning = false;
            });
    }

    public override void BtnClose()
    {
        tween.Kill();
        isSpinning = false;
        base.BtnClose();
    }
}
