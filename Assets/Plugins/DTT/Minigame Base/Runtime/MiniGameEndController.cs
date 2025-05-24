using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DTT.MinigameBase.UI
{
    public class MiniGameEndController : MonoBehaviour
        {
        public static MiniGameEndController instance;
        [Header("Màn hình hiển thị")]
        [SerializeField]
        [LabelText("Ảnh nền màn hình kết thúc")]
        Sprite sprBgEnd;
        GameObject displayGameEnd;
        [Header("Cài đặt màn hình chơi game")]
        [Header("Cài đặt màn hình kết thúc")]
        [SerializeField]
        [LabelText("Kiểu kết thúc")]
        TypeEnd typeEnd;
        [SerializeField]
        [LabelText("Có chơi tiếp")]
        bool haveNextGame;
        [SerializeField]
        [LabelText("Có tổng kết")]
        bool haveFade;
        Image bgGameEnd;
        [HideInInspector]
        public Transform displayGameEndHidden;
        [Header("Kết thúc thường")]
        GameObject frameEndStar;
        [Header("Kết thúc tính điểm")]
        [LabelText("Văn bản tính điểm")]
        [SerializeField]
        string strEndScore = "<b><i><size=40>Bạn đã đạt được\n</size><size=100>xxx\n</size><size=35>điểm</size></i></b>";
        GameObject frameEndScore;
        Text textEndScore;
        [HideInInspector]
        public int score;
        [Header("Kết thúc đáp án")]
        [SerializeField]
        [LabelText("Có ảnh đáp án")]
        bool haveImageAnswer;
        [SerializeField]
        [LabelText("Có chữ đáp án")]
        bool haveTextAnswer;
        [SerializeField]
        [LabelText("Ảnh nền khung chữ đáp án")]
        Sprite[] sprFrameTextAnswer;
        [SerializeField]
        [LabelText("Prefab câu trả lời")]
        GameObject prefabAnswer;
        GameObject frameEndAnswer;
        Transform listAnswer;
        [SerializeField]
        [LabelText("Số câu trả lời mặc định")]
        int defautNumberOfAnswer = 5;
        [HideInInspector]
        public int totalAnswer;
        [Header("Cài đặt thắng")]
        [HideInInspector]
        public bool isWin;
        [Header("Hiệu ứng")]
        [LabelText("Hiệu ứng chọn")]
        public ParticleSystem particleChoose;
        [LabelText("Hiệu ứng trả lời đúng")]
        public ParticleSystem particleCorrect;
        [LabelText("Hiệu ứng trả lời sai")]
        public ParticleSystem particleWrong;
        [LabelText("Hiệu ứng thắng")]
        public ParticleSystem particleWin;
        [LabelText("Hiệu ứng thắng nhỏ")]
        public ParticleSystem particleMiniWin;
        [LabelText("Hiệu ứng thua")]
        public ParticleSystem particleLose;
        [LabelText("Hiệu ứng mở rương")]
        public ParticleSystem particleOpenTreasure;
        [Header("Âm thanh")]
        [HideInInspector]
        public AudioSource audioSourceGameplay;
        [LabelText("Âm thanh chọn")]
        public AudioClip audioParticleChoose;
        [LabelText("Âm thanh trả lời đúng")]
        public AudioClip audioParticleCorrect;
        [LabelText("Âm thanh trả lời sai")]
        public AudioClip audioParticleWrong;
        [LabelText("Âm thanh thắng")]
        public AudioClip audioParticleWin;
        [LabelText("Âm thanh thắng nhỏ")]
        public AudioClip audioParticleMiniWin;
        [LabelText("Âm thanh thua")]
        public AudioClip audioParticleLose;
        [LabelText("Âm thanh mở rương")]
        public AudioClip audioParticleOpenTreasure;
        [LabelText("Âm thanh kết thúc game")]
        public AudioClip audioGameEnd;

        enum TypeEnd
        {
            normal, score, answer
        }

        private void OnEnable()
        {
            if (instance == null) instance = this;
            AttachAllObject();
            if (prefabAnswer == null) SetPrefabAnswer();
            SetGameEnd();
            displayGameEnd.SetActive(false);
        }

        private void OnDisable() {
            instance = null;
        }

        void AttachAllObject()
        {
            AttachObject(ref displayGameEnd, "displayGameEnd");
            AttachObject(ref bgGameEnd, "bgGameEnd");
            AttachObject(ref displayGameEndHidden, "displayGameEndHidden");
            AttachObject(ref frameEndStar, "frameEndStar");
            AttachObject(ref frameEndScore, "frameEndScore");
            AttachObject(ref textEndScore, "textEndScore");
            AttachObject(ref frameEndAnswer, "frameEndAnswer");
            AttachObject(ref listAnswer, "listAnswer");
            AttachObject(ref particleChoose, "particleChoose");
            AttachObject(ref particleCorrect, "particleCorrect");
            AttachObject(ref particleWrong, "particleWrong");
            AttachObject(ref particleWin, "particleWin");
            AttachObject(ref particleMiniWin, "particleMiniWin");
            AttachObject(ref particleLose, "particleLose");
            AttachObject(ref particleOpenTreasure, "particleOpenTreasure");
            AttachObject(ref audioSourceGameplay, "audioSourceGameplay");
        }

        void AttachObject(ref GameObject obj, string nameObjFind)
        {
            if (obj == null)
            {
                string str1 = nameObjFind.Substring(0, 1);
                string str2 = nameObjFind.Substring(1);
                str1 = str1.ToUpper();
                nameObjFind = str1 + str2;
                obj = transform.FindDeepChild<Transform>(nameObjFind).gameObject;
            }
        }

        void AttachObject(ref Transform obj, string nameObjFind)
        {
            if (obj == null)
            {
                string str1 = nameObjFind.Substring(0, 1);
                string str2 = nameObjFind.Substring(1);
                str1 = str1.ToUpper();
                nameObjFind = str1 + str2;
                obj = transform.FindDeepChild<Transform>(nameObjFind);
            }
        }

        void AttachObject<T>(ref T obj, string nameObjFind) where T : Component
        {
            //if (obj == null)
            //{
            string str1 = nameObjFind.Substring(0, 1);
            string str2 = nameObjFind.Substring(1);
            str1 = str1.ToUpper();
            nameObjFind = str1 + str2;
            //Debug.LogError("=================== T " + nameObjFind);
            obj = transform.FindDeepChild<T>(nameObjFind);
            //}
        }

        void ShowGame(GameObject gameObject)
        {
            switch (gameObject.name) {
                case "DisplayGameEnd":
                    displayGameEnd.SetActive(true);
                    break;
            }
        }

        #region Audio
        public void PlayAudio(AudioClip audioClip)
        {
            if (audioClip != null) audioSourceGameplay.PlayOneShot(audioClip);
        }
        #endregion


        #region Particle
        public void PlayParticle(ParticleSystem particle, AudioClip audioClip)
        {
            if (particle != null) particle.Play();
            PlayAudio(audioClip);
        }

        public void PlayParticle(ParticleSystem particle, AudioClip audioClip, Vector3 posTarget)
        {
            if (particle != null) particle.transform.position = new Vector3(posTarget.x, posTarget.y, particle.transform.position.z);
            PlayParticle(particle, audioClip);
        }

        public IEnumerator Play(string e, Vector3 p, AudioClip c = null)
        {
            switch (e)
            {
                case "choose":
                    PlayParticle(particleChoose, audioParticleChoose, p);
                    break;
                case "win":
                    PlayParticle(particleWin, audioParticleWin, p);
                    break;
                case "big":
                    PlayParticle(particleMiniWin, audioParticleMiniWin, p);
                    break;
                case "correct":
                    PlayParticle(particleCorrect, audioParticleCorrect, p);
                    break;
                case "wrong":
                    PlayParticle(particleWrong, audioParticleWrong, p);
                    break;
                case "lose":
                    PlayParticle(particleLose, audioParticleLose, p);
                    break;
            }

            yield return new WaitForSeconds(1f);

            if (c != null)
            {
                audioSourceGameplay.PlayOneShot(c);
                yield return new WaitForSeconds(1f);
            }
        }
        #endregion

        #region GameStart

        #endregion

        #region GameLevel

        #endregion

        #region GamePlay

        #endregion

        #region GameEnd

        public void ShowGameEnd()
        {
            ShowGame(displayGameEnd);
            PlayAudio(audioGameEnd);
            if (displayGameEndHidden != null && displayGameEndHidden.GetComponent<CanvasGroup>() != null) displayGameEndHidden.GetComponent<CanvasGroup>().alpha = 1;
            switch (typeEnd)
            {
                case TypeEnd.answer:
                    for (int i = 0; i < listAnswer.childCount; i++)
                    {
                        listAnswer.GetChild(i).gameObject.SetActive(false);
                    }
                    break;
            }
            switch (typeEnd)
            {
                case TypeEnd.score:
                    textEndScore.text = strEndScore;
                    textEndScore.text = textEndScore.text.Replace("xxx", "" + score);
                    textEndScore.text = textEndScore.text.Replace("\\n", "\n");
                    break;
                case TypeEnd.answer:
                    for (int i = 0; i < totalAnswer; i++)
                    {
                        GameObject answer = GetAnswer(i);
                        if (haveTextAnswer) {
                            if (sprFrameTextAnswer.Length > 0) answer.transform.FindDeepChild<Image>("FrameTextAnswer").sprite = sprFrameTextAnswer[i % sprFrameTextAnswer.Length];
                        }
                        answer.SetActive(true);
                    }
                    break;
            }

            PlayParticle(particleWin, audioParticleWin);
            if (haveFade) StartCoroutine(FadeDisplayGameEndHidden());
            //StartCoroutine(PlayParticleWin(1.5f));
        }

        void SetPrefabAnswer() {
            GameObject _prefabAnswer = Resources.Load<GameObject>("PrefabGames/Answer");
            prefabAnswer = Instantiate(_prefabAnswer, listAnswer);
            Transform imageAnswer = prefabAnswer.transform.FindDeepChild<Transform>("ImageAnswer");
            if (haveImageAnswer) imageAnswer.gameObject.SetActive(true);
            else imageAnswer.gameObject.SetActive(false);
            Transform frameTextAnswer = prefabAnswer.transform.FindDeepChild<Transform>("FrameTextAnswer");
            if (haveTextAnswer) 
            { 
                frameTextAnswer.gameObject.SetActive(true);
                if (sprFrameTextAnswer.Length == 0) frameTextAnswer.GetComponent<Image>().color = Color.clear;
                else frameTextAnswer.GetComponent<Image>().color = Color.white;
            }
            else frameTextAnswer.gameObject.SetActive(false);
        }

        void SetGameEnd()
        {
            if (sprBgEnd) bgGameEnd.sprite = sprBgEnd;
            frameEndStar.SetActive(false);
            frameEndScore.SetActive(false);
            frameEndAnswer.SetActive(false);
            switch (typeEnd)
            {
                case TypeEnd.normal:
                    frameEndStar.SetActive(true);
                    break;
                case TypeEnd.score:
                    frameEndScore.SetActive(true);
                    break;
                case TypeEnd.answer:
                    frameEndAnswer.SetActive(true);
                    break;
            }
        }

        public GameObject GetAnswer(int index)
        {
            if (index <= listAnswer.childCount - 1) return listAnswer.GetChild(index).gameObject;
            else
            {
                GameObject answer = Instantiate(prefabAnswer, listAnswer);
                return answer;
            }
        }

        public void SetTextForAnswer(int index, string str)
        {
            GetAnswer(index).transform.FindDeepChild<Text>("TextAnswer").text = str;
        }

        public void SetImageForAnswer(int index, Sprite spr)
        {
            GetAnswer(index).transform.FindDeepChild<Image>("ImageAnswer").sprite = spr;
        }

        IEnumerator FadeDisplayGameEndHidden()
        {
            Debug.LogError("============== co fade");
            yield return new WaitForSeconds(audioParticleWin.length + 1);
            CanvasGroup canvasGroup = displayGameEndHidden.GetComponent<CanvasGroup>();
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, 0.5f);
        }

        #endregion
    }
    public static class TransformExtensions
    {
        public static T FindDeepChild<T>(this Transform parent, string targetName) where T : Component
        {
            foreach (Transform child in parent)
            {
                if (child.name == targetName)
                    return child.GetComponent<T>();

                var result = child.FindDeepChild<T>(targetName);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
