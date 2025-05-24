using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Novastars.MiniGame.DuaXe
{
    public class UIManager : SingletonBehaviour<UIManager>
    {
        [Header("Cài đặt các bảng menu")]
        [LabelText("Hiển thị phần câu hỏi thảo luận")] public bool TurnOnDiscuss;
        [LabelText("Hiển thị đáp án phần thảo luận")] public bool TurnOnDiscussAnswer;
        [LabelText("Bật màn hình menu chính")] public bool TurnOnMainMenu;
        [LabelText("Bật màn hình chọn xe người chơi")][LabelWidth(200)] public bool TurnOnChooseCar;

        [Header("Cài đặt tuỳ chỉnh")]
        [LabelText("Bật bảng tuỳ chỉnh")] public bool TurnOnSetting;
        [LabelText("Đảo vị trí câu hỏi")] public bool TurnOnSufferQuestion;
        [LabelText("Đảo vị trí đáp án")] public bool TurnOnSufferAnswer;
        [LabelText("Hiển thị thanh chỉnh số lượng câu hỏi")][LabelWidth(220)] public bool TurnOnSliderAmount;
        [LabelText("Hiển thị thanh chỉnh tốc độ xe")][LabelWidth(220)] public bool TurnOnSliderSpeed;

        [Header("Cài đặt màn hình menu chính")]
        [LabelText("Ảnh nền menu chính")] public Sprite MainMenuBackGroundSprite;
        [LabelText("Tên trò chơi")] public Sprite GameTitleSprite;
        [LabelText("Nút bắt đầu chơi")] public Sprite PlaySprite;
        [LabelText("Nút tuỳ chỉnh")] public Sprite SettingSprite;

        [Header("Cài đặt màn hình chơi")]
        [LabelText("Hình nền động")] public Sprite GameplayBackgroundSprite;
        [LabelText("Hình nền động cho làn đường")] public Sprite GameplayForegroundSprite;
        [LabelText("Đáp án A")] public Sprite AAnswerSprite;
        [LabelText("Đáp án B")] public Sprite BAnswerSprite;

        [Header("Cài đặt tuỳ chọn xe")]
        [LabelText("Nền màn hình chọn xe")] public Sprite ChooseCarBackgroundSprite;
        [LabelText("Nút tiếp tục")] public Sprite ContinueSprite;
        [LabelText("Xe chọn mặc định")] public Sprite DefaultCarSelectSprite;

        [Space(5), Tooltip("Số thứ tự")] public List<Sprite> CarsSelectNumbers;
        [Space(5), Tooltip("Xe đang chọn")] public List<Sprite> CarsSelectSprites;
        [Space(5), Tooltip("Các xe cùng làn")] public List<Sprite> CarsRoadSprites;
        [Space(5), Tooltip("Các xe chắn đường")] public List<Sprite> CarObstacleSprites;

        #region Unity Funcion
        private new void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if (CarsSelectSprites.Count > 6) Debug.LogError($"Số lượng xe được chọn tối đa là 6 - để vừa với khung hình chọn xe - hiện tại đang có {CarsSelectSprites.Count}");
            if (CarsSelectNumbers.Count > 6) Debug.LogError($"Số lượng số thự tự xe được chọn tối đa là 6 - để vừa với khung hình chọn xe - hiện tại đang có {CarsSelectNumbers.Count}");
        }
        #endregion
    }
}