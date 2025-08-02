using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.TimDiemKhacBiet {
    public class DiffernceObject : MonoBehaviour
    {
        public int id;
        [SerializeField]
        Image circle;
        public bool isChoose;

        public void ChooseTrueDifferenceObj() {
            if (isChoose) return;
            GameController.instance.PlayAudioTrue();
            for (int i = 0; i < GameController.instance.pictures.Count; i++) {
                GameController.instance.pictures[i].differnceObjects[id].isChoose = true;
                GameController.instance.pictures[i].differnceObjects[id].AnimationCircle();
                GameController.instance.pictures[i].differnceObjects[id].GetComponent<Button>().enabled = false;
            }
            GameController.instance.countDifferenceObj--;
            GameController.instance.textCountDifferenceObj.text = GameController.instance.countDifferenceObj + "";
            if (GameController.instance.CheckWin()) GameController.instance.SetWin();

        }

        public void AnimationCircle() {
            circle.fillAmount = 0;
            circle.gameObject.SetActive(true);
            DOTween.To(() => circle.fillAmount, x => circle.fillAmount = x, 1, 0.5f).SetEase(Ease.Linear);
        } 

        public void Reset() {
            isChoose = false;
            circle.fillAmount = 0;
            circle.gameObject.SetActive(false);
            transform.GetComponent<Button>().enabled = true;
        }

    }
}
