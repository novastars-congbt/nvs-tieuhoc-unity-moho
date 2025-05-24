using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.LatBai
{
    public class Mission : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _missionDetailTMP;
        [SerializeField] private Button _missonHideButton;
        [SerializeField] private Button _missionOpenAnwerBtn;

        private string _missionQuestionString;
        private string _missionAnwString;
        private bool _isShowMission = true;

        #region Unity Functions
        private void Awake()
        {
            _missionDetailTMP = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            _missonHideButton.onClick.AddListener(ShowHideMission);
            _missionOpenAnwerBtn.onClick.AddListener(OpenAnswerMission);
        }
        #endregion

        public void Settup(string missionQues, string missionAns, int numberOrder)
        {
            _missionQuestionString = $"{numberOrder}. {missionQues}";
            _missionAnwString = missionAns;

            _missionDetailTMP.text = _missionQuestionString;
            _missionOpenAnwerBtn.gameObject.SetActive(_missionAnwString.Trim().Length > 0);
        }

        #region Private Method
        private void ShowHideMission()
        {
            _isShowMission = !_isShowMission;

            if (_isShowMission) _missionDetailTMP.color = new Color(_missionDetailTMP.color.r, _missionDetailTMP.color.g, _missionDetailTMP.color.b, 1f);
            else _missionDetailTMP.color = new Color(_missionDetailTMP.color.r, _missionDetailTMP.color.g, _missionDetailTMP.color.b, 0.35f);
        }

        private void OpenAnswerMission()
        {
            PopupManager.Instance.GetAnswerPopupObject().SetActive(true);
            PopupManager.Instance.GetAnswerPopupScript().ShowAnswer(_missionAnwString);
        }
        #endregion
    }
}