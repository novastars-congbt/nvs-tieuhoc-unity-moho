using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Novastars.MiniGame.LatBai
{
    // [Serializable]
    // public class CardSFX
    // {
    //     public Sprite CardSprite;
    //     public AudioClip CardAudio;

    //     public CardSFX(Sprite cardSprite)
    //     {
    //         CardSprite = cardSprite;
    //     }
    // }

    //[ExecuteAlways]
    public class FeedbackManager : SingletonBehaviour<FeedbackManager>
    {
        Vector3 worldPosition;
        [LabelText("Vị trí tạo hiệu ứng")] [SerializeField] private Transform _parrentSpawn;

        [Header("Hiệu ứng nền")]
        [LabelText("Nhạc nền")] public AudioClip BackgroundMusic;

        [Header("Hiệu ứng nút bắt đầu và tuỳ chỉnh")]
        [LabelText("Hiệu ứng âm thanh")] public AudioClip ClickButtonSFX;
        [LabelText("Hiệu ứng hình ảnh")] public GameObject ClickButtonVFX;

        [Header("Hiệu ứng hoàn thành màn chơi")]
        [LabelText("Hiệu ứng âm thanh")] public AudioClip VictorySFX;
        [LabelText("Hiệu ứng hình ảnh")] public GameObject VictoryVFX;

        [Header("Hiệu ứng chọn thẻ bài")]
        [LabelText("Hiệu ứng âm thanh")] public AudioClip ClickCardSFX;
        [LabelText("Hiệu ứng hình ảnh")] public GameObject ClickCardVFX;

        [Header("Hiệu ứng chọn thẻ đúng")]
        [LabelText("Hiệu ứng âm thanh")] public AudioClip RightCardSFX;
        [LabelText("Hiệu ứng hình ảnh")] public GameObject RightCardVFX;

        [Header("Hiệu ứng chọn thẻ sai")]
        [LabelText("Hiệu ứng âm thanh")] public AudioClip WrongCardSFX;
        [LabelText("Hiệu ứng hình ảnh")] public GameObject WrongCardVFX;

        [Space]
        [Header("Hiệu ứng thẻ bài")]
        [LabelText("Đồng bộ hiệu ứng thẻ bài")][Tooltip("Chọn khi thay đổi hình ảnh của thẻ bài")] [SerializeField] private bool setSysnCardData_ChiAnKhiCanSyncData;
        //[LabelText("Hiệu ứng âm thanh")] public List<CardSFX> audioCards = new List<CardSFX>();
        private List<DataCard.Card> cardDatas = new List<DataCard.Card>();

        private AudioSource cardAudioSource;

        #region Unity Funcion
        private new void Awake()
        {
            base.Awake();

            cardAudioSource = GetComponent<AudioSource>();
        }

        void OnEnable()
        {
            DestroyAllVFX();
            SyncCardDatas();
        }

//#if UNITY_EDITOR
        private void Update()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            worldPosition = Camera.main.ScreenToWorldPoint(mousePos);            
            //if (!Application.isPlaying) SyncCardDatas();
        }
//#endif
        #endregion

        #region Public Method
        public GameObject SpawnClickButtonVFX()
        {
            _parrentSpawn.position= new Vector2(worldPosition.x,worldPosition.y);
            var vfx = ClickButtonVFX ? Instantiate(ClickButtonVFX, _parrentSpawn) : null;
            if (vfx)
            {
                if (vfx.GetComponentInChildren<ParticleSystem>().main.loop) Destroy(vfx, 10);
                else Destroy(vfx, 1.5f);
            }

            return vfx;
        }

        public GameObject SpawnVictoryVFX()
        {
            _parrentSpawn.position= new Vector2(0,0);
            var vfx = VictoryVFX ? Instantiate(VictoryVFX, _parrentSpawn) : null;
            vfx.transform.localScale = new Vector3 (3, 3, 3);
            return vfx;
        }

        public GameObject SpawnClickVFX()
        {
            _parrentSpawn.position= new Vector2(worldPosition.x,worldPosition.y);
            var vfx = ClickCardVFX ? Instantiate(ClickCardVFX, _parrentSpawn) : null;
            if (vfx)
            {
                if (vfx.GetComponentInChildren<ParticleSystem>().main.loop) Destroy(vfx, 10);
                else Destroy(vfx, 1.5f);
            }

            return vfx;
        }

        public GameObject SpawnTrueVFX()
        {
            _parrentSpawn.position= new Vector2(worldPosition.x+1,worldPosition.y+1);
            var vfx = RightCardVFX ? Instantiate(RightCardVFX, _parrentSpawn) : null;
            if (vfx)
            {
                if (vfx.GetComponentInChildren<ParticleSystem>().main.loop) Destroy(vfx, 10);
                else Destroy(vfx, 1.5f);
            }

            return vfx;
        }

        public GameObject SpawnFalseVFX()
        {
            _parrentSpawn.position= new Vector2(worldPosition.x+1,worldPosition.y+1);
            var vfx = WrongCardVFX ? Instantiate(WrongCardVFX, _parrentSpawn) : null;
            if (vfx)
            {
                if (vfx.GetComponentInChildren<ParticleSystem>().main.loop) Destroy(vfx, 10);
                else Destroy(vfx, 1.5f);
            }

            return vfx;
        }

        public void DestroyAllVFX() {
            foreach (Transform vfx in _parrentSpawn) Destroy(vfx.gameObject);
        }

        public void PlayBackgroundMusic()
        {
            if (BackgroundMusic)
            {
                cardAudioSource.clip = BackgroundMusic;
                cardAudioSource.Play();
            }
        }

        public void PlayButtonClickSFX()
        {
            if (ClickButtonSFX) cardAudioSource.PlayOneShot(ClickButtonSFX);
        }

        public void PlayVictorySFX()
        {
            if (VictorySFX) cardAudioSource.PlayOneShot(VictorySFX);
        }

        public void PlayClickCardSFX()
        {
            if (ClickCardSFX) cardAudioSource.PlayOneShot(ClickCardSFX);
        }

        public void PlayTrueSFX(AudioClip cardAudio /*Sprite cardSprite*/)
        {
            StartCoroutine(PlayTrueAndCardAudioSequence(cardAudio /*cardSprite*/));
        }

        public void PlayFalseSFX()
        {
            if (WrongCardSFX) cardAudioSource.PlayOneShot(WrongCardSFX);
        }

        private IEnumerator PlayTrueAndCardAudioSequence(AudioClip cardAudio /*Sprite cardSprite*/)
        {
            // Play the true SFX
            if (RightCardSFX)
            {
                cardAudioSource.PlayOneShot(RightCardSFX);

                // Wait for the duration of the RightCardSFX
                yield return new WaitForSeconds(RightCardSFX.length);
            }

            // Play the card audio
            PlayCardAudio(cardAudio /*cardSprite*/);
        }

        public void PlayCardAudio(AudioClip cardAudio /*Sprite cardSprite*/)
        {
            //var cardAudio = audioCards.Find(audioCard => audioCard.CardSprite.Equals(cardSprite)).CardAudio;
            /* if (cardAudio)*/ cardAudioSource.PlayOneShot(cardAudio);
        }
        #endregion

        #region Private Method
        private void SyncCardDatas()
        {
            //if (!setSysnCardData_ChiAnKhiCanSyncData) return;

            //audioCards.Clear();

            cardDatas.Clear();
            foreach(var card in DataManager.Instance.dataCard.Cards) {
                cardDatas.Add(card);
            }
            //cardDatas = DataManager.Instance.dataCard.Cards;
            // cardDatas.ForEach(cardData =>
            // {
            //     if (audioCards.Find(audioCard => audioCard.CardSprite == cardData.CardSprite) == null)
            //     {
            //         audioCards.Add(new CardSFX(cardData.CardSprite));
            //     }
            // });
        }
        #endregion
    }

#if UNITY_EDITOR
    //[CustomPropertyDrawer(typeof(CardSFX))]
    public class CardSFXDrawer : PropertyDrawer
    {
        const float imageHeight = 30;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var cardSpriteProp = property.FindPropertyRelative("CardSprite");

            if (cardSpriteProp.propertyType == SerializedPropertyType.ObjectReference && (cardSpriteProp.objectReferenceValue as Sprite) != null)
            {
                return EditorGUI.GetPropertyHeight(cardSpriteProp, label, true) + imageHeight + 5;
            }

            return EditorGUI.GetPropertyHeight(cardSpriteProp, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty cardSpriteProp = property.FindPropertyRelative("CardSprite");
            SerializedProperty cardSFXProp = property.FindPropertyRelative("CardAudio");

            Rect pos = EditorGUI.PrefixLabel(position, label);
            int imgPreviewWidth = 50;
            int imgPreviewHeight = 50;
            float spacing = 5;
            float leftOverWidthRatio = imgPreviewWidth / pos.width;

            Rect p1 = pos;
            Rect p2 = new Rect(pos.x, pos.y, pos.width * (1 - leftOverWidthRatio) - 5, pos.height);
            p2.x += imgPreviewWidth + spacing * 1;

            if (cardSpriteProp.objectReferenceValue as Sprite)
            {
                Texture texturePreview = (cardSpriteProp.objectReferenceValue as Sprite).texture;
                EditorGUI.DrawPreviewTexture(new Rect(p1.x, p1.y, imgPreviewWidth, imgPreviewHeight), texturePreview, null, scaleMode: ScaleMode.StretchToFill);
            }

            EditorGUI.PropertyField(p2, cardSFXProp, GUIContent.none);
        }
    }
#endif
}
