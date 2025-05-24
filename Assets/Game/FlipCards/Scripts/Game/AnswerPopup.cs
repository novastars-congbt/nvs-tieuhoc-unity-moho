using System.Collections;
using TMPro;
using UnityEngine;

namespace Novastars.MiniGame.LatBai
{
    public class AnswerPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _answerTMP;

        #region Public Method
        public void ShowAnswer(string answer)
        {
            _answerTMP.text = answer;
        }
        #endregion
    }
}