using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Novastars.MiniGame.KeoTha
{
    [CreateAssetMenu(menuName = "Novastars/Mini Game/Keo tha/Level", fileName = "Level")]
    public class LevelManager : ScriptableObject
    {
        [LabelText("Ảnh chung khu vực thả")]
        public Sprite sprDropBound;
        [LabelText("Ảnh chung đối tượng cần kéo")]
        public Sprite sprDragObj;
        [LabelText ("Danh sách khu vực thả")]
        public DropBound[] dropBounds;
        [LabelText ("Danh sách đối tượng cần kéo")]
        public DragObj[] dragObjs;
        [LabelText ("Danh sách hiệu ứng kéo đúng")]
        public EffectTrue[] effectTrues;

        [Serializable]
        public struct DropBound {
            [LabelText("Tên khu vực thả")]
            public string name;
        }

        [Serializable]
        public struct DragObj
        {
            [LabelText("Id hoàn thành")]
            public int idComplete;
            [LabelText("Tên đối tượng cần kéo")]
            public string name;
            [LabelText("Ảnh đối tượng cần kéo")]
            public Sprite spr;
        }

        [Serializable]
        public struct EffectTrue {
            [LabelText("Danh sách Id hoàn thành")]
            public int[] idCompletes;
            [LabelText("Âm thanh đúng")]
            public AudioClip audioTrue;
        }
    }
}
