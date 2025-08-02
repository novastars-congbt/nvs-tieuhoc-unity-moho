using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Novastars.MiniGame.TimDiemKhacBiet {
    public class GameController : MonoBehaviour
    {
        public static GameController instance;
        public Text textCountDifferenceObj;
        public List<Picture> pictures;
        public int countDifferenceObj = 0;
        [SerializeField]
        AudioSource audioSource;
        [SerializeField]
        AudioClip[] audioClip;
        void OnEnable()
        {
            if (instance == null) instance = this;
            ResetGame();
            StartGame();
        }

        void OnDisable()
        {
            instance = null;
        }

        public void PlayAudioTrue() {
            audioSource.PlayOneShot(audioClip[0]);
        }

        public void PlayAudioFalse() {
            audioSource.PlayOneShot(audioClip[1]);
        }

        public bool CheckWin() {
            return countDifferenceObj <= 0;
        }

        public void SetWin() {
            StartCoroutine(CouroutineWin());
        }

        IEnumerator CouroutineWin() {
            yield return new WaitForSeconds(audioClip[0].length);
            if (MiniGameEndController.instance != null)
            {
                MiniGameEndController.instance.ShowGameEnd();
            }
            else
            {
                gameObject.SetActive(false);
                gameObject.SetActive(true);
            }
        }

        void StartGame(){
            for (int i = 0; i < pictures.Count; i++)
            {
                pictures[i].SetListDifferenceObject();
            }
            countDifferenceObj = pictures[0].differnceObjects.Count;
            textCountDifferenceObj.text = countDifferenceObj + "";
        }

        void ResetGame() {

            for (int i = 0; i < pictures.Count; i++)
            {
                for (int j = 0; j < pictures[i].differnceObjects.Count; j++)
                {
                    pictures[i].differnceObjects[j].Reset();
                }
            }
        }
    }
}
