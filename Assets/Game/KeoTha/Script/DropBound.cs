using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

namespace Novastars.MiniGame.KeoTha {
    public class DropBound : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        public int id;
        public Image imgFrame;
        public Text textName;
        public Transform parentDrag;
        public Vector2 size;
        //public Transform title;
        // public Transform oldPos;
        // public Transform targetPos;
        public void OnDrop(PointerEventData eventData) {
            Debug.LogError("============== OnDrop");
            GameObject dropped = eventData.pointerDrag;
            DraggableObject draggableObject = dropped.GetComponent<DraggableObject>();
            if (id == draggableObject.idComplete) {
                draggableObject.isCompleted = true;
            }
            else
            {
                if (MiniGameEndController.instance != null) MiniGameEndController.instance.PlayParticle(MiniGameEndController.instance.particleWrong, MiniGameEndController.instance.audioParticleWrong, transform.position);
                draggableObject.wrongPerObj--;
                if (draggableObject.wrongPerObj < 0) draggableObject.wrongPerObj = 0;
            }

            //if (parentDrag == null) draggableObject.Validation(transform.position, transform, true);
            /*else*/ draggableObject.Validation(parentDrag.position, parentDrag, true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.LogError("================== click");
            if (GameController.instance.chooseDrag.gameObject.activeSelf)
            {
                Debug.LogError("================== can click");
                DraggableObject draggableObject = GameController.instance.chooseDrag.transform.parent.GetComponent<DraggableObject>();
                if (draggableObject.isBack) return;
                if (id != draggableObject.idComplete)
                {
                    if (MiniGameEndController.instance != null) MiniGameEndController.instance.PlayParticle(MiniGameEndController.instance.particleWrong, MiniGameEndController.instance.audioParticleWrong, transform.position);
                    draggableObject.wrongPerObj--;
                    if (draggableObject.wrongPerObj < 0) draggableObject.wrongPerObj = 0;
                    return;
                }
                else draggableObject.isCompleted = true;
                draggableObject.Validation(transform.position, transform, false);
                if (transform.GetComponent<GridLayoutGroup>() != null)
                {
                    StartCoroutine(SetLayoutGroup());
                }
            }
        }

        IEnumerator SetLayoutGroup()
        {
            transform.GetComponent<GridLayoutGroup>().enabled = false;
            yield return 0.1f;
            transform.GetComponent<GridLayoutGroup>().enabled = true;
        }
    }
}
