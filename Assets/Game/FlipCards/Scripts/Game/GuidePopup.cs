using Assets.Diamondhenge.HengeVideoPlayer;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Novastars.MiniGame.LatBai
{
    public class GuidePopup : MonoBehaviour
    {
        [SerializeField] private GameObject _videoObject;
        [SerializeField] private GameObject _textObject;
        [SerializeField] private GameObject _imgObject;

        [Space]
        [SerializeField] private HengeVideo _hengeVideoPlayer;
        [SerializeField] private VideoPlayer  _videoPlayer;
        [SerializeField] private TextMeshProUGUI _guildePopupTMP;
        [SerializeField] private Image _guidePopupImg;

        #region Unity Funcion
        private void Start()
        {
            SetupGuidePopup();
        }
        #endregion

        #region Public Method
        public void LoadGuide()
        {
            if (DataManager.Instance.IsUseClip) StartCoroutine(_hengeVideoPlayer.AwakeRoutine());
        }
        #endregion

        #region Private Method
        private void SetupGuidePopup()
        {
            _videoObject.SetActive(false);
            _textObject.SetActive(false);
            _imgObject.SetActive(false);


            if (DataManager.Instance.IsUseText)
            {
                _textObject.SetActive(true);
                _guildePopupTMP.text = DataManager.Instance.GuidePopupString;
            }
            else if (DataManager.Instance.IsUseSprite)
            {
                _imgObject.SetActive(true);
                _guidePopupImg.sprite = DataManager.Instance.GuidePopupSprite;
            }
            else if (DataManager.Instance.IsUseClip)
            {
                _videoObject.SetActive(true);
                _hengeVideoPlayer.videoSource.videoClip = DataManager.Instance.GuidePopupClip;
                _videoPlayer.clip = DataManager.Instance.GuidePopupClip;
            }

            gameObject.SetActive(false);
        }
        #endregion
    }
}