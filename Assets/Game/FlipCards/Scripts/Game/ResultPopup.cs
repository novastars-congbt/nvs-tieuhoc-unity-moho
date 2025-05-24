using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.LatBai
{
    public class ResultPopup : MonoBehaviour
    {
        [SerializeField] private GameObject _cardResultPreviewPref;
        [SerializeField] private Transform _cardResultParent;

        [SerializeField] private GameObject _normalResultPopup;
        [SerializeField] private GameObject _startResultPopup;

        #region Public Method
        public void Replay()
        {
            gameObject.SetActive(false);
            FeedbackManager.Instance.DestroyAllVFX();
            BaseGameUI.Instance.GoToMenu();
        }

        public void ShowNormal()
        {
            _normalResultPopup.SetActive(true);
            _startResultPopup.SetActive(false);

            ShowCardResultPreview();
        }

        public void ShowStar()
        {
            _normalResultPopup.SetActive(false);
            _startResultPopup.SetActive(true);

            ShowCardResultPreview();
        }
        #endregion

        #region Private Method
        private void ShowCardResultPreview()
        {
            foreach (Transform card in _cardResultParent) {
                Destroy(card.gameObject);
            }
            foreach (var cardSprite in GameManager.Instance.CardSpawnSprites)
            {
                Instantiate(_cardResultPreviewPref, _cardResultParent).GetComponentInChildren<Image>().sprite = cardSprite;
            }
        }
        #endregion
    }
}