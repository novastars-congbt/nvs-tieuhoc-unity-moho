using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Novastars.MiniGame.LatBai
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        [Header("Cài đặt tuỳ chọn")]
        [LabelText("Đảo vị trí thẻ bài")] public bool IsSufferOn;
        [LabelText("Số lượng thẻ bài")] public int CurrentCardAmount;
        [ReadOnly][LabelText("Tuỳ chỉnh đã thực hiện")] public bool DoneSetupConfig = false;

        [Header("Thẻ bài đang sử dụng")]
        [LabelText("Thẻ bài đang mở")] public Card OnHoldCard;
        [LabelText("Thẻ bài được tạo")] public List<Sprite> CardSpawnSprites = new List<Sprite>();

        [Header("Script đang sử dụng")]
        [LabelText("Script điều khiển giao diện")][SerializeField] private GameplayUI _gameplayUI;

        private float clickInterval = 0.75f;

        #region Unity Funcion
        private new void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            clickInterval -= Time.deltaTime;
        }
        #endregion

        #region Public Method
        public bool CanClickCard() => clickInterval <= 0;
        public void ResetClickInterval() => clickInterval = 0.75f;

        public void CheckWin(float time)
        {
            if (_gameplayUI.GetDeskRegion().IsFullCardOpen())
            {
                StartCoroutine(ShowResultCourotine(time + 2.5f));
            }
        }
        #endregion

        #region Private Method
        private IEnumerator ShowResultCourotine(float timeWait)
        {
            yield return new WaitForSeconds(timeWait);
            if (MiniGameEndController.instance != null) {
                MiniGameEndController.instance.ShowGameEnd();
            }
            else 
            {
                PopupManager.Instance.ShowResult();
                FeedbackManager.Instance.SpawnVictoryVFX();
                FeedbackManager.Instance.PlayVictorySFX();
            }
        }
        #endregion
    }
}