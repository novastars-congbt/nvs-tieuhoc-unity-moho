using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Novastars.MiniGame.LatBai
{
    [CreateAssetMenu(menuName = "Novastars/Mini Game/Lat bai/Data", fileName = "Data")]
    public class DataCard : ScriptableObject
    {
        [LabelText("Số lượng thẻ bài định trước")] public int defaultCardAmount;
        [LabelText("Sử dụng thẻ bài cỡ lớn")] public bool isFullCard;
        [LabelText("Hình ảnh và nội dung thẻ bài")] public Card[] Cards;

        [Serializable]
        public struct Card{
            [LabelText("Ảnh thẻ bài")]
            public Sprite CardSprite;
            [LabelText("Chữ thẻ bài")]
            public string CardLabel;
            [LabelText("Âm thanh thẻ bài")]
            public AudioClip CardAudio;
        }
    }
}
