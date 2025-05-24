using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Novastars.MiniGame.LatBai
{
    public class Card : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _cardBorder;
        [SerializeField] private Image _cardBack;
        [SerializeField] private Image _cardImgFull;
        [SerializeField] private Image _cardImgHalf;
        [SerializeField] private TextMeshProUGUI _cardLabelHalf;
        [SerializeField] private TextMeshProUGUI _cardNumber;

        [Header("Animation")]
        [SerializeField] private Animator _animator;

        private Sprite _cardSprite;
        private AudioClip _cardAudio;
        private bool _cardOpened = false;
        public bool _cardOpening = false;

        #region Unity Functions
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _cardBorder.sprite = UIManager.Instance.BorderCardSprite;
        }
        #endregion

        #region Public Method
        public void Settup(bool isFullCard, Sprite cardSprite, string cardLabel, AudioClip cardAudio)
        {
            _cardSprite = cardSprite;
            _cardAudio = cardAudio;
            _cardBack.sprite = UIManager.Instance.BackCardSprite;

            _cardImgFull.gameObject.SetActive(false);
            _cardImgHalf.gameObject.SetActive(false);
            _cardLabelHalf.gameObject.SetActive(false);

            if (isFullCard)
            {
                _cardImgFull.gameObject.SetActive(true);
                _cardImgFull.sprite = cardSprite;
            }
            else
            {
                _cardImgHalf.gameObject.SetActive(true);
                _cardLabelHalf.gameObject.SetActive(true);

                _cardImgHalf.sprite = cardSprite;
                _cardLabelHalf.text = cardLabel;
            }
        }

        public bool IsCardOpen() => _cardOpened;
        public Sprite GetCardSprite() => _cardSprite;

        public void SetCardOrder(int cardOrderNumber)
        {
            _cardNumber.text = cardOrderNumber.ToString();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_cardOpened) return;
            if (_cardOpening) return;
            if (!GameManager.Instance.CanClickCard()) return;

            _cardOpening = true;

            var spawnClickVFX = FeedbackManager.Instance.SpawnClickVFX();
            if (spawnClickVFX)
            {
                spawnClickVFX.transform.position = new Vector2(transform.position.x, transform.position.y);
            }
            FeedbackManager.Instance.PlayClickCardSFX();

            OpenCard();

            if (GameManager.Instance.OnHoldCard != null)
            {
                if (GameManager.Instance.OnHoldCard.GetCardSprite().Equals(_cardSprite))
                {
                    FeedbackManager.Instance.SpawnTrueVFX();
                    FeedbackManager.Instance.PlayTrueSFX(_cardAudio /*_cardSprite*/);

                    _cardOpened = true;
                    GameManager.Instance.OnHoldCard._cardOpened = true;
                    if (_cardAudio) GameManager.Instance.CheckWin(_cardAudio.length);
                    else GameManager.Instance.CheckWin(0f);

                    StartCoroutine(ResetTransform(0.5f));
                }
                else
                {
                    FeedbackManager.Instance.SpawnFalseVFX();
                    FeedbackManager.Instance.PlayFalseSFX();

                    StartCoroutine(CloseCardCourotine(0.5f));
                }
            }
            else
            {
                GameManager.Instance.OnHoldCard = this;
            }
        }
        #endregion

        #region Private Method
        public void OpenCard()
        {
            GameManager.Instance.ResetClickInterval();

            _animator.Play("Open");
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        public void CloseCard()
        {
            GameManager.Instance.ResetClickInterval();

            _animator.Play("Close");
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        private IEnumerator CloseCardCourotine(float timeWait)
        {
            yield return new WaitForSeconds(timeWait);

            GameManager.Instance.OnHoldCard._animator.Play("Close");
            GameManager.Instance.OnHoldCard.transform.localScale = new Vector3(1f, 1f, 1f);

            _animator.Play("Close");
            transform.localScale = new Vector3(1f, 1f, 1f);

            _cardOpening = false;
            GameManager.Instance.OnHoldCard._cardOpening = false;
            GameManager.Instance.OnHoldCard = null;
        }

        private IEnumerator ResetTransform(float timeWait)
        {
            yield return new WaitForSeconds(timeWait);

            GameManager.Instance.OnHoldCard.transform.localScale = new Vector3(1f, 1f, 1f);
            transform.localScale = new Vector3(1f, 1f, 1f);

            GameManager.Instance.OnHoldCard = null;
        }
        #endregion
    }
}