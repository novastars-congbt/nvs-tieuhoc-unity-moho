using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
//using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.LatBai
{
    public class DeskRegion : MonoBehaviour
    {
        [Header("Card Prefab")]
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private Transform _cardParrent;

        [Header("Animation")]
        [SerializeField] private Animator _deskRegionAnim;
        [SerializeField] private bool _isRegionOpen = false;

        [Header("Desk UI")]
        [SerializeField] private TextMeshProUGUI _guideTMP;
        [SerializeField] private Image _guideTextPannel;

        [Header("Card")]
        [SerializeField] private List<Card> cardSpawnList = new List<Card>();

        #region Unity Funcion
        private void Awake() => _deskRegionAnim = GetComponent<Animator>();
        private void OnDisable(){
            cardSpawnList.Clear();
            if (_cardParrent != null){
            // Detach each child from the parent
                foreach (Transform child in _cardParrent.transform){
                    Destroy(child.gameObject);
                }
            }
            else Debug.LogWarning("Parent GameObject is not assigned.");
        }
        private void OnEnable()
        {
            _guideTextPannel.sprite = UIManager.Instance.GuideTextPannelSprite;
            _isRegionOpen = false;
            SetupAllCard();
            SetupGuideText();
        }
        #endregion

        #region Public Method
        public void CloseRegion()
        {
            _deskRegionAnim.Play("Close", 0);
        }

        public void FoldDeskRegion()
        {
            //_isRegionOpen = !_isRegionOpen;

            /*if (_isRegionOpen)*/ _deskRegionAnim.Play("Open", 0);
            //else _deskRegionAnim.Play("Close", 0);
        }
        public void ShowCard(){
            foreach(var card in cardSpawnList){
                card.GetComponent<Card>().OpenCard();
                card.GetComponent<Card>()._cardOpening = true;
            }
        }
        public void CloseCard(){
            foreach(var card in cardSpawnList){
                card.GetComponent<Card>().CloseCard();
                card.GetComponent<Card>()._cardOpening = false;
            }
        }
        public bool IsFullCardOpen()
        {
            foreach (var card in cardSpawnList)
            {
                if (!card.IsCardOpen()) return false;    
            }

            return true;
        }
        #endregion

        #region Private Method
        private void SetupAllCard()
        {
            var isFullCard = DataManager.Instance.dataCard.isFullCard;
            List<DataCard.Card> cardDatas = new List<DataCard.Card>();
            foreach (var card in DataManager.Instance.dataCard.Cards) {
                cardDatas.Add(card);
            }
            //var cardDatas = DataManager.Instance.CardDatas;
            var currentCardAmount = GameManager.Instance.CurrentCardAmount;

            StartCoroutine(SetupCourotine());

            #region Local Funcion
            IEnumerator SetupCourotine()
            {
                yield return new WaitUntil(() => GameManager.Instance.DoneSetupConfig);

                currentCardAmount = GameManager.Instance.CurrentCardAmount;
                cardDatas = GameManager.Instance.IsSufferOn ? cardDatas.OrderBy(i => Guid.NewGuid()).ToList() : cardDatas;
                GameManager.Instance.CardSpawnSprites.Clear();
                for (int i = 0; i < currentCardAmount; i++)
                {
                    var cardScript_1 = Instantiate(_cardPrefab, _cardParrent).GetComponent<Card>();
                    var cardScript_2 = Instantiate(_cardPrefab, _cardParrent).GetComponent<Card>();

                    cardScript_1.Settup(isFullCard, cardDatas[i].CardSprite, cardDatas[i].CardLabel,cardDatas[i].CardAudio);
                    cardScript_2.Settup(isFullCard, cardDatas[i].CardSprite, cardDatas[i].CardLabel, cardDatas[i].CardAudio);

                    cardSpawnList.Add(cardScript_1);
                    cardSpawnList.Add(cardScript_2);
                    GameManager.Instance.CardSpawnSprites.Add(cardDatas[i].CardSprite);
                }
                // Suffer Card List
                cardSpawnList = cardSpawnList.OrderBy(i => Guid.NewGuid()).ToList();
                foreach (var cardChil in _cardParrent)
                {
                    var indexOf = cardSpawnList.FindIndex(card => card.Equals((cardChil as Transform).GetComponent<Card>()));

                    (cardChil as Transform).SetSiblingIndex(indexOf);
                }

                for (int childIndex = 0; childIndex < _cardParrent.childCount; childIndex++)
                {
                    _cardParrent.GetChild(childIndex).GetComponent<Card>().SetCardOrder(childIndex + 1);
                }
                ShowCard();
                yield return new WaitForSeconds(3);
                CloseCard();
            }
            
            #endregion
        }

        private void SetupGuideText()
        {
            _guideTMP.text = DataManager.Instance.GuideString;
        }
        #endregion
    }
}