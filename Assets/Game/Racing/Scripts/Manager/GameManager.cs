using Novastars.MiniGame.LatBai;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Novastars.MiniGame.DuaXe
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        [Header("Cài đặt tuỳ chỉnh")]
        [LabelText("Đảo vị trí câu hỏi")] public bool IsSufferQuestionOn;
        [LabelText("Đảo vị trí đáp án")] public bool IsSufferAnswerOn;
        [LabelText("Số lượng câu hỏi")] public int CurrentQuestionAmount;
        [ReadOnly][LabelText("Tuỳ chỉnh đã thực hiện")] public bool DoneSetupConfig = false;
        [ReadOnly][LabelText("Màn chơi kết thúc")] public bool IsGameOver;

        [LabelText("Câu hỏi đang sử dụng")] public List<DataQuestions.QuestionData> CurrentQuestionDatas = new List<DataQuestions.QuestionData>();
        [ReadOnly][LabelText("Số câu hỏi đã trả lời đúng")] public int CurrentQuestionIndex;
        [ReadOnly][LabelText("Xe đang sử dụng")] public Sprite CurrentCarSprite;
        [ReadOnly][LabelText("Xe cùng làn đang sử dụng")] public Sprite CurrentRoadSprite;
        [ReadOnly][LabelText("Xe chặn đường đang sử dụng")] public Sprite CurrentBlockSprite;

        #region Unity Funcion
        private new void Awake()
        {
            base.Awake();
        }
        #endregion

        #region Public Method
        public void SetupQuestionData()
        {
            var questionData = DataManager.Instance.dataQuestions.QuestionDatas;
            var isSufferQuestion = GameManager.Instance.IsSufferQuestionOn;
            if (isSufferQuestion)
            {
                questionData = isSufferQuestion ? questionData.OrderBy(i => Guid.NewGuid()).ToList() : questionData;
            }

            CurrentQuestionDatas = questionData.GetRange(0, CurrentQuestionAmount);
        }
        #endregion
    }
}