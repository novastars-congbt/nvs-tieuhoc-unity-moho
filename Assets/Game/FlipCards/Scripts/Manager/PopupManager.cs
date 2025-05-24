using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Novastars.MiniGame.LatBai
{
    public class PopupManager : SingletonBehaviour<PopupManager>
    {
        [Header("Cài đặt bảng hiển thị")]
        [LabelText("Vị trí bảng hiển thị")][SerializeField] private Transform _popupTransformParrent;

        [Header("Prefab các bảng hiển thị")]
        [LabelText("Bảng tuỳ chỉnh")][SerializeField] private GameObject _settingPopupPrefab;
        [LabelText("Bảng hướng dẫn")][SerializeField] private GameObject _guidePopupPrefab;
        [LabelText("Bảng kết quả")][SerializeField] private GameObject _resultPopupPrefab;
        [LabelText("Bảng đáp án")][SerializeField] private GameObject _answerPopupPrefab;

        private GameObject _settingPopupObject;
        private GameObject _guidePopupObject;
        private GameObject _resultPopupObject;
        private GameObject _answerPopupObject;

        [Header("Cài đặt các khung hiển thị")]
        [LabelText("Khung câu hỏi thử thách")] [SerializeField] private GameObject _missionPannel;
        [LabelText("Khung tương tác với thẻ bài")] [SerializeField] private GameObject _deskCardPannel;
        [LabelText("Khung hướng dẫn")] [SerializeField] private GameObject _guideTextPannel;
        [LabelText("Nút hiển thị khung hướng dẫn")] [SerializeField] private GameObject _guideButtonPannel;

        #region Unity Funcion
        private new void Awake()
        {
            base.Awake();

            InitilizePopup();
        }
        #endregion

        #region Public Method
        public GameObject GetSettingPopupObject() => _settingPopupObject;
        public GameObject GetGuidePopupObject() => _guidePopupObject;
        public GameObject GetResultPopupObject() => _resultPopupObject;
        public GameObject GetAnswerPopupObject() => _answerPopupObject;
        public SettingPopup GetSettingPopupScript() => _settingPopupObject.GetComponent<SettingPopup>();
        public GuidePopup GetGuidePopupScript() => _guidePopupObject.GetComponent<GuidePopup>();
        public ResultPopup GetResultPopupScript() => _resultPopupObject.GetComponent<ResultPopup>();
        public AnswerPopup GetAnswerPopupScript() => _answerPopupObject.GetComponent<AnswerPopup>();

        public void ShowResult()
        {
            _resultPopupObject.SetActive(true);

            _missionPannel.SetActive(false);
            _deskCardPannel.SetActive(false);
            _guideTextPannel.SetActive(false);
            _guideButtonPannel.SetActive(false);

            _resultPopupObject.GetComponent<Animator>().Play("Show");
            
            if (FeatureManager.Instance.IsShowStarResultPopup)
            {
                GetResultPopupScript().ShowStar();
            }
            else
            {
                GetResultPopupScript().ShowNormal();
            }
        }
        #endregion

        #region Private Method
        private void InitilizePopup()
        {
            _settingPopupObject = Instantiate(_settingPopupPrefab, _popupTransformParrent);
            _guidePopupObject = Instantiate(_guidePopupPrefab, _popupTransformParrent);
            _resultPopupObject = Instantiate(_resultPopupPrefab, _popupTransformParrent);
            _answerPopupObject = Instantiate(_answerPopupPrefab, _popupTransformParrent);
            
            _settingPopupObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _guidePopupObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _resultPopupObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _answerPopupObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        #endregion
    }
}