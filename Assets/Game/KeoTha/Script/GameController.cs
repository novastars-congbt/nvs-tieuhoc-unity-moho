using DG.Tweening;
using DTT.Utils.Extensions;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.KeoTha
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;
        [LabelText("Data")]
        public LevelManager levelManager;
        //[HideInInspector]
        public Canvas canvas;
        public int currentLevel = 0;
        public int scorePerCard = 10;
        public float score = 0;
        float scorePerWrong = 0;
        public Transform listDragObj;
        public Transform listStar;
        public Transform chooseDrag;
        public GameObject guide;
        public GameObject popupWin;
        public GameObject prefabStar;
        public DraggableObject prefabDragObj;
        public Text textScore;
        //float widthTitle;
        float timeAudioEffectTrue;
        [SerializeField]
        Vector2 multiplier = new Vector2(2f, 2f);
        bool needResetSize = false;
        Action actionAfterWaitReset;
        List<ContentSizeFitter> contentSizeFitters = new List<ContentSizeFitter>();
        List<HorizontalLayoutGroup> horizontalLayoutGroups = new List<HorizontalLayoutGroup>();
        //Vector2 sizeParentDrag;
        //Vector2 cellSizeParentDrag;
        public List<DraggableObject> draggableObjects = new List<DraggableObject>();
        public List<GameObject> stars = new List<GameObject>();
        public List<int> listIndex = new List<int>();
        public List<DropBound> dropBounds = new List<DropBound>();
        public List<MoveObj> moveObjs = new List<MoveObj>();
    
        private void OnEnable()
        {
            if (instance == null) instance = this;
            canvas = transform.parent.FindDeepChild<Canvas>("Canvas");
            ResetGame();
            //StartGame();
        }

        private void OnDisable()
        {
            instance = null;
        }

        void StartGame()
        {
            scorePerWrong = (float)scorePerCard/dropBounds.Count;
            listIndex.Clear();
            for (int i = 0; i < levelManager.dragObjs.Length; i++)
            {
                listIndex.Add(i);
            }
            listIndex.Shuffle();
            for (int i = 0; i < levelManager.dropBounds.Length; i++) {
                if (dropBounds[i].imgFrame && levelManager.sprDropBound) dropBounds[i].imgFrame.sprite = levelManager.sprDropBound;
                if (dropBounds[i].textName && levelManager.dropBounds[i].name.Length > 0) dropBounds[i].textName.text = levelManager.dropBounds[i].name;
            }
            for (int i = 0; i < levelManager.dragObjs.Length; i++)
            {
                draggableObjects[i].idComplete = levelManager.dragObjs[listIndex[i]].idComplete;
                if (levelManager.dragObjs[listIndex[i]].spr != null) { 
                    draggableObjects[i].image.sprite = levelManager.dragObjs[listIndex[i]].spr;
                    draggableObjects[i].image.type = Image.Type.Simple;
                    if (i == 0) {
                        GridLayoutGroup gridLayout = listDragObj.GetComponent<GridLayoutGroup>();
                        Debug.LogError("====================== width = " + levelManager.dragObjs[listIndex[i]].spr.texture.width);
                        gridLayout.cellSize = new Vector2(gridLayout.cellSize.y * levelManager.dragObjs[listIndex[i]].spr.rect.width / levelManager.dragObjs[listIndex[i]].spr.rect.height, gridLayout.cellSize.y);
                    }
                }
                else if (levelManager.sprDragObj != null) { 
                    draggableObjects[i].image.sprite = levelManager.sprDragObj;
                    draggableObjects[i].image.type = Image.Type.Sliced;
                }
                draggableObjects[i].textName.text = levelManager.dragObjs[listIndex[i]].name;
            }
        }

        void ResetGame()
        {
            popupWin.SetActive(false);
            chooseDrag.gameObject.SetActive(false);
            horizontalLayoutGroups.Clear();
            foreach (var obj in moveObjs) {
                obj.transform.position = obj.oldPos.position;
                if (obj.transform.GetComponent<HorizontalLayoutGroup>() != null) horizontalLayoutGroups.Add(obj.transform.GetComponent<HorizontalLayoutGroup>());
            }
            StartCoroutine(WaitReset<HorizontalLayoutGroup>(horizontalLayoutGroups, true, 0.5f));
            contentSizeFitters.Clear();
            foreach (var bound in dropBounds)
            {
                //bound.transform.position = bound.oldPos.position;
                if (bound.parentDrag == null) bound.parentDrag = bound.transform;
                GridLayoutGroup gridLayoutGroup = bound.parentDrag.GetComponent<GridLayoutGroup>();
                RectTransform rectTransform = bound.parentDrag.GetComponent<RectTransform>();
                // if (cellSizeParentDrag == Vector2.zero) cellSizeParentDrag = gridLayoutGroup.cellSize;
                // else if (gridLayoutGroup.cellSize != cellSizeParentDrag) gridLayoutGroup.cellSize = cellSizeParentDrag;
                if (needResetSize) {
                    gridLayoutGroup.cellSize = new Vector2(gridLayoutGroup.cellSize.x / multiplier.x, gridLayoutGroup.cellSize.y / multiplier.y);
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x / multiplier.x, rectTransform.sizeDelta.y / multiplier.y);
                }
                else bound.size = bound.parentDrag.GetComponent<RectTransform>().sizeDelta;
                contentSizeFitters.Add(bound.parentDrag.GetComponent<ContentSizeFitter>());
                //RectTransform rectTransform = bound.parentDrag.GetComponent<RectTransform>();
                //Debug.LogErrorFormat("======================= {0}, {1}, {2}, {3}", rectTransform.sizeDelta.x, rectTransform.sizeDelta.y, sizeParentDrag.x, sizeParentDrag.y);
                // if (sizeParentDrag == Vector2.zero) sizeParentDrag = rectTransform.sizeDelta;
                // else if (rectTransform.sizeDelta != sizeParentDrag) rectTransform.sizeDelta = sizeParentDrag;
                //if (widthTitle == 0) widthTitle = bound.title.GetComponent<RectTransform>().sizeDelta.x;
                //else StartCoroutine(WaitContentSize(bound/*, widthTitle*/));
                //bound.transform.localScale = Vector3.one;
            }
            // if (!needResetSize) ResetDragObjs();
            // else {
            //     actionAfterWaitReset += ResetDragObjs;
            //     StartCoroutine(WaitReset<ContentSizeFitter>(contentSizeFitters, false, 0.01f));
            // }
            ResetDragObjs();

            foreach (var star in stars)
            {
                star.SetActive(false);
            }
            if (levelManager.dragObjs.Length > listStar.childCount)
            {
                for (int i = listStar.childCount; i < levelManager.dragObjs.Length; i++)
                {
                    GameObject star = Instantiate(prefabStar, listStar);
                    star.transform.SetAsLastSibling();
                }
            }
            stars.Clear();
            for (int i = 0; i < levelManager.dragObjs.Length; i++)
            {
                stars.Insert(0, listStar.GetChild(i).gameObject);
            }
            foreach (var star in stars)
            {
                star.SetActive(true);
                star.transform.GetChild(0).gameObject.SetActive(false);
            }

            // CancelInvoke("SetInitPos");
            // Invoke("SetInitPos", 0.1f);
        }

        void ResetDragObjs() {
            if (listDragObj.GetComponent<GridLayoutGroup>() != null) listDragObj.GetComponent<GridLayoutGroup>().enabled = true;
            foreach (var obj in draggableObjects)
            {
                obj.transform.SetParent(listDragObj);
                obj.transform.SetAsLastSibling();
                obj.gameObject.SetActive(false);
            }
            if (levelManager.dragObjs.Length > listDragObj.childCount)
            {
                for (int i = listDragObj.childCount; i < levelManager.dragObjs.Length; i++)
                {
                    DraggableObject drag = Instantiate(prefabDragObj, listDragObj);
                    drag.transform.SetAsLastSibling();
                    drag.transform.localPosition = new Vector3(drag.transform.localPosition.x, drag.transform.localPosition.y, 0);
                }
            }
            
            draggableObjects.Clear();
            for (int i = 0; i < levelManager.dragObjs.Length; i++)
            {
                draggableObjects.Add(listDragObj.GetChild(i).GetComponent<DraggableObject>());
                draggableObjects[i].Reset();
            }
            CancelInvoke("SetInitPos");
            Invoke("SetInitPos", 0.1f);
            score = 0;
            SetScore();
            CancelInvoke("SetOrder");
            Invoke("SetOrder", 0.1f);
            StartGame();
        }

        void SetInitPos()
        {
            if (listDragObj.GetComponent<GridLayoutGroup>() != null)
            {
                listDragObj.GetComponent<GridLayoutGroup>().enabled = false;
                for (int i = 0; i < draggableObjects.Count; i++)
                {
                    draggableObjects[i].initPosition = draggableObjects[i].transform.position;
                }
            }
            chooseDrag.GetRectTransform().sizeDelta = prefabDragObj.GetRectTransform().sizeDelta;
        }

        void SetOrder() {
            for (int i = 0; i < levelManager.dragObjs.Length; i++)
            {
                draggableObjects[i].transform.GetComponent<Canvas>().sortingOrder = canvas.sortingOrder + 1;
            }
        }

        void SetFrameEnd()
        {
            contentSizeFitters.Clear();
            horizontalLayoutGroups.Clear();
            for (int i = 0;i < dropBounds.Count;i++)
            {
                //dropBounds[i].transform.DOMove(dropBounds[i].targetPos.position, 0.5f);
                GridLayoutGroup gridLayoutGroup = dropBounds[i].parentDrag.GetComponent<GridLayoutGroup>();
                RectTransform rectTransform = dropBounds[i].parentDrag.GetComponent<RectTransform>();
                //gridLayoutGroup.cellSize = cellSizeParentDrag * multiplier;
                gridLayoutGroup.cellSize = new Vector2(gridLayoutGroup.cellSize.x * multiplier.x, gridLayoutGroup.cellSize.y * multiplier.y);
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * multiplier.x, rectTransform.sizeDelta.y * multiplier.y);
                needResetSize = true;
                //contentSizeFitters.Add(dropBounds[i].parentDrag.GetComponent<ContentSizeFitter>());
                //StartCoroutine(WaitContentSize(dropBounds[i]/*, -1*/));
            }
            //StartCoroutine(WaitReset<ContentSizeFitter>(contentSizeFitters, false, 0));
            foreach (var obj in moveObjs) {
                obj.transform.DOMove(obj.targetPos.position, 0.5f);
                if (obj.GetComponent<HorizontalLayoutGroup>() != null) horizontalLayoutGroups.Add(obj.GetComponent<HorizontalLayoutGroup>());
            }
            StartCoroutine(WaitReset<HorizontalLayoutGroup>(horizontalLayoutGroups, true, 0));
        }

        IEnumerator WaitReset<T>(List<T> components, bool reset, float time /*dropBound*/ /*, float widthTitle*/)where T : Behaviour
        {
            // dropBound.parentDrag.GetComponent<ContentSizeFitter>().enabled = false;
            //yield return new WaitForSeconds(0.01f);
            foreach(var component in components) {
                yield return null;
                //else yield return new WaitForSeconds(time);
                component.enabled = !reset;
                if (time <= 0) yield return null;
                else yield return new WaitForSeconds(time);
                component.enabled = reset;
            }
            //dropBound.parentDrag.GetComponent<ContentSizeFitter>().enabled = true;
            //yield return null;
            // if (widthTitle < 0) dropBound.title.GetComponent<RectTransform>().sizeDelta = new Vector2(dropBound.parentDrag.GetComponent<RectTransform>().sizeDelta.x, dropBound.title.GetComponent<RectTransform>().sizeDelta.y);
            // else dropBound.title.GetComponent<RectTransform>().sizeDelta = new Vector2(widthTitle, dropBound.title.GetComponent<RectTransform>().sizeDelta.y);
            //dropBound.parentDrag.GetComponent<ContentSizeFitter>().enabled = false;
            actionAfterWaitReset?.Invoke();
            actionAfterWaitReset = null;
            //yield return new WaitForSeconds(0.01f);
            //dropBound.parentDrag.GetComponent<ContentSizeFitter>().enabled = true;
        }

        public void SetScore()
        {
            int count = 0;
            score = 0;
            for (int i = 0; i < draggableObjects.Count; i++)
            {
                if (draggableObjects[i].isCompleted)
                {
                    count++;
                    score += scorePerWrong * draggableObjects[i].wrongPerObj;
                }
            }
            textScore.text = "<size=60>xxx</size>";
            textScore.text = textScore.text.Replace("xxx", (int)Mathf.Ceil(score) + "");
            for (int i = 0; i < count; i++)
            {
                if (!stars[i].transform.GetChild(0).gameObject.activeSelf)
                {
                    stars[i].transform.GetChild(0).gameObject.SetActive(true);
                    if (MiniGameEndController.instance != null) MiniGameEndController.instance.PlayAudio(MiniGameEndController.instance.audioParticleCorrect);
                }
            }
        }

        public void SetEffectTrue(int idComplete) {
            Debug.LogError("========================= effecttrue " + idComplete);
            if (levelManager.effectTrues == null) return;
            LevelManager.EffectTrue effectTrue = Array.Find(levelManager.effectTrues, arr => arr.idCompletes.Contains(idComplete));
            if (effectTrue.Equals(default(LevelManager.EffectTrue))) { 
                Debug.LogError("========================= default effecttrue");
                timeAudioEffectTrue = 0;
                return;
            }
            for (int i = 0; i < effectTrue.idCompletes.Length; i++) {
                if (draggableObjects.Find(x => (x.idComplete == effectTrue.idCompletes[i] && x.isCompleted == false)) != null) { 
                    Debug.LogError("========================= false");
                    return;
                }
            }
            Debug.LogError("================= audioTrue " + effectTrue.audioTrue.name);
            if (effectTrue.audioTrue) timeAudioEffectTrue = effectTrue.audioTrue.length;
            else timeAudioEffectTrue = 0;
            if (MiniGameEndController.instance != null) MiniGameEndController.instance.PlayAudio(effectTrue.audioTrue);
        }

        public bool CheckCompleteGame()
        {
            for (int i = 0; i < draggableObjects.Count; i++)
            {
                if (draggableObjects[i].idComplete < 0 || !draggableObjects[i].isCompleted) return false;
            }
            return true;
        }

        public void SetWin()
        {
            //if (MiniGameEndController.instance != null) popupWin.SetActive(true);
            
            StartCoroutine(CouroutineWin());
            // if (MiniGameEndController.instance == null) popupWin.SetActive(true);
            // else
            // {
            //     MiniGameEndController.instance.score = (int)Mathf.Ceil(score);
            //     MiniGameEndController.instance.ShowGameEnd();
            // }
            // SetFrameEnd();
        }

        IEnumerator CouroutineWin()
        {
            // SetFrameEnd();
            // yield return new WaitForSeconds(1);
            yield return new WaitForSeconds(timeAudioEffectTrue);
            if (MiniGameEndController.instance == null) popupWin.SetActive(true);
            else
            {
                MiniGameEndController.instance.score = (int)Mathf.Ceil(score);
                MiniGameEndController.instance.ShowGameEnd();
            }
            SetFrameEnd();
        }

        public void BtnNext()
        {
            //currentLevel++;
            ResetGame();
            //StartGame();
        } 

        public void BtnRestart()
        {
            ResetGame();
            //StartGame();
        }

        public void BtnNova()
        {
            guide.SetActive(!guide.activeSelf);
        }
    }
}
