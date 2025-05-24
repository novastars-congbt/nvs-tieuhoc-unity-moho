using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DTT.MinigameBase.Advertisements;
using DTT.Utils.Extensions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Novastars.MiniGame.KeoTha {
    public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler 
    {
        public int id;
        public int idComplete;
        public int wrongPerObj;
        public Image image;
        public Text textName;
        public Vector3 initPosition;
        public bool isCompleted = false;
        public bool isBack = false;
        public CanvasGroup canvasGroup;
        public Tween tweenBack;

        RectTransform rectTransform;
        private void Awake() {
            initPosition = transform.position;
            rectTransform = GetComponent<RectTransform>();
        }

        public void SetUp() {
            initPosition = transform.position;
        }

        public void Reset() {
            isCompleted = false;
            wrongPerObj = GameController.instance.dropBounds.Count;
            transform.position = initPosition;
            canvasGroup.blocksRaycasts = true;
            image.raycastTarget = true;
            gameObject.SetActive(true);
        }

        public void Validation(Vector3 pos, Transform trans, bool isDrag) {
            if (isCompleted) {
                //GameAudio.instance.PlayCorrectVoices();
                GameController.instance.chooseDrag.gameObject.SetActive(false);
                if (trans != null) transform.parent = trans;
                transform.position = pos;
                transform.GetComponent<Canvas>().sortingOrder = GameController.instance.canvas.sortingOrder + 1;
                image.raycastTarget = false;
                GameController.instance.SetScore();
                GameController.instance.SetEffectTrue(idComplete);
                if (GameController.instance.CheckCompleteGame()) GameController.instance.SetWin();
           
                //canvasGroup.blocksRaycasts = true;
                //gameObject.SetActive(false);

                //GameController.instance.countCorrect++;
                //if ((GameController.instance.currentLevel == 0 && GameController.instance.countCorrect == 4) ||
                //    (GameController.instance.currentLevel == 2 && GameController.instance.countCorrect == 4) ||
                //    (GameController.instance.currentLevel != 0 && GameController.instance.countCorrect == 5)) {
                //GameController.instance.ShowResultUI();
                // }
                //} else {
                //    GameAudio.instance.PlayIncorrectVoices();
                //transform.DOMove(initPosition, 1);
            }
            else if (isDrag) 
            {
                isBack = true;
                transform.DOMove(initPosition, 0.5f).OnComplete(() =>
                {
                    isBack = false;
                    canvasGroup.blocksRaycasts = true;
                    transform.GetComponent<Canvas>().sortingOrder = GameController.instance.canvas.sortingOrder + 1;
                });
            }
            
        }

        public void OnBeginDrag(PointerEventData eventData) {
            Debug.LogError("==================== begin drag");
            if (isCompleted) return;
            if (isBack) return;
            canvasGroup.blocksRaycasts = false;
            transform.GetComponent<Canvas>().sortingOrder = GameController.instance.canvas.sortingOrder + 2;
        }

        public void OnDrag(PointerEventData eventData) {
            //Debug.LogError("================= drag");
            if (isCompleted) return;
            if (isBack) return;
            //rectTransform.anchoredPosition += eventData.delta;
            var position = Camera.main.ScreenToWorldPoint(eventData.position);
            position.z = transform.position.z;
            transform.position = position /*+ new Vector3(0, 5f, 0)*/;
        }

        public void OnEndDrag(PointerEventData eventData) {
            Debug.LogError("================ end drag");
            if(isCompleted) return;
            if (isBack) return;
            //canvasGroup.blocksRaycasts = true;
            Validation(initPosition, null, true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
                
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isCompleted) return;
            if (isBack) return;
            if (MiniGameEndController.instance != null) MiniGameEndController.instance.PlayAudio(MiniGameEndController.instance.audioParticleChoose);
            Transform choose = GameController.instance.chooseDrag;
            //if (choose.parent == transform && choose.gameObject.activeSelf)
            //{
            //    choose.gameObject.SetActive(false);
            //}
            //else
            //{
                choose.SetParent(transform);
                choose.SetAsLastSibling();
                choose.localPosition = Vector3.zero;
                choose.gameObject.SetActive(true);
            //}
        }
    }
}
