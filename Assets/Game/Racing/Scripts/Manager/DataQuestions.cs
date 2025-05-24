using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Novastars.MiniGame.DuaXe
{
    [CreateAssetMenu(menuName = "Novastars/Mini Game/Dua xe/Question", fileName = "Question")]
    public class DataQuestions : ScriptableObject
    {
        [LabelText("Số lượng câu hỏi mặc định")]
        public int DefaultQuesitonAmount;
        public List<QuestionData> QuestionDatas;

        [Serializable]
        public class QuestionData
        {
            [LabelText("Câu hỏi")]
            public string QuestionString;
            [LabelText("Đáp án A")]
            public string AnswerAString;
            [LabelText("Đáp án A đúng")]
            public bool IsAnswerA;
            [LabelText("Đáp án B")]
            public string AnswerBString;
            [LabelText("Đáp án B đúng")]
            public bool IsAnswerB;

            public (bool, string) GetAnswer(int index)
            {
                if (index == 0)
                {
                    return (IsAnswerA, AnswerAString);
                }
                else
                {
                    return (IsAnswerB, AnswerBString);
                }
            }
        }
    }
}
