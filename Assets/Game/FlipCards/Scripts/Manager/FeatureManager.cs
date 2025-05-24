using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Novastars.MiniGame.LatBai
{
    //[ExecuteAlways]
    public class FeatureManager : SingletonBehaviour<FeatureManager>
    {
        [LabelText("Bỏ qua màn hình menu chính")] public bool PassMainMenu;

        [LabelText("Sử dụng bảng kết quả đánh sao")][Tooltip("Chỉ bật 1 bảng")] public bool IsShowStarResultPopup;
        [LabelText("Sử dụng bảng kết quả thẻ bài")][Tooltip("Chỉ bật 1 bảng")] public bool IsShowDeskResultPopup;

        #region Unity Funcion
        private new void Awake()
        {
            base.Awake();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying) ConfigGameFeature();
        }
#endif
        #endregion

        #region Private Method
        private void ConfigGameFeature()
        {
            if (IsShowStarResultPopup) IsShowDeskResultPopup = false;
            else IsShowDeskResultPopup = true;
        }
        #endregion
    }
}