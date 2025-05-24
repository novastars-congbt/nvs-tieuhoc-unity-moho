using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Novastars.MiniGame.LatBai
{
    public class UIManager : SingletonBehaviour<UIManager>
    {
        [Header("Cài đặt tuỳ chỉnh")]
        [LabelText("Bật bảng tuỳ chỉnh")] public bool TurnOnSetting;
        [LabelText("Đảo vị trí thẻ bài")] public bool TurnOnSetting_SufferCard;
        [LabelText("Hiển thị thanh chỉnh số lượng thẻ")] public bool TurnOnSetting_NumberCard;

        [Header("Cài đặt hình nền")]
        [LabelText("Ảnh nền")] public Sprite BackGroundSprite;

        [Header("Cài đặt menu chính")]
        [LabelText("Tên trò chơi")] public Sprite GameTitleSprite;
        [LabelText("Nút bắt đầu chơi")] public Sprite PlaySprite;
        [LabelText("Nút tuỳ chỉnh")] public Sprite SettingSprite;

        [Header("Cài đặt bảng câu hỏi thử thách")]
        [LabelText("Bật bảng câu hỏi")] public bool TurnOnMissonOutGame;

        [Header("Cài đặt khung câu hỏi thử thách")]
        [LabelText("Bật bảng câu hỏi")] public bool TurnOnMissonInGame;
        [LabelText("Bảng câu hỏi")] public Sprite MissionPannelSprite;
        [LabelText("Nút sang trái")] public Sprite ArrowLeft;
        [LabelText("Nút sang phải")] public Sprite ArrowRight;


        [Header("Cài đặt hiển thị thẻ bài")]
        [LabelText("Khung viền thẻ bài")] public Sprite BorderCardSprite;
        [LabelText("Mặt lưng thẻ bài")] public Sprite BackCardSprite;

        [Header("Cài đặt bảng hướng dẫn")]
        [LabelText("Bật bảng hướng dẫn và nút hiển thị")] public bool TurnOnGuideButton;
        [LabelText("Bảng hướng dẫn")] public Sprite GuideTextPannelSprite;
        [LabelText("Nút hiển thị bảng hướng dẫn")] public Sprite GuideButtonSprite;

        #region Unity Funcion
        private new void Awake()
        {
            base.Awake();
        }
        #endregion

        #region Public Method

        #endregion

        #region Private Method

        #endregion
    }
}