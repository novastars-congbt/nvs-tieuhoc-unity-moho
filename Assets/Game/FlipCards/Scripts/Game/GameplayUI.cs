using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.LatBai
{
    public class GameplayUI : MonoBehaviour
    {
        [Header("UI Element")]
        [SerializeField] private Button _foldMissionButton;
        [SerializeField] private Button _showTutorialButton;
        [SerializeField] private Image _guideImgButton;

        [Header("UI Script Reference")]
        [SerializeField] private MissionRegion _missionReigon;
        [SerializeField] private DeskRegion _deskReigon;

        [Header("Animation")]
        [SerializeField] private Animator _tutorialBtnAnim;
        [SerializeField] private bool _isButtonFold = true;

        #region Unity Funcion
        private void OnEnable()
        {
            _guideImgButton.sprite = UIManager.Instance.GuideButtonSprite;
            _missionReigon.gameObject.SetActive(UIManager.Instance.TurnOnMissonInGame);
            _deskReigon.gameObject.SetActive(true);
            if (!UIManager.Instance.TurnOnMissonInGame)
            {
                FoldMissionRegion();
            }

            _showTutorialButton.gameObject.SetActive(UIManager.Instance.TurnOnGuideButton);
            _foldMissionButton.onClick.AddListener(FoldMissionRegion);
            _showTutorialButton.onClick.AddListener(ShowTutorial);
        }

        private void OnDisable()
        {
            _foldMissionButton.onClick.RemoveAllListeners();
            _showTutorialButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Public Method
        public MissionRegion GetMissionRegion() => _missionReigon;
        public DeskRegion GetDeskRegion() => _deskReigon;
        #endregion

        #region Private Method
        private void FoldMissionRegion()
        {
            if (_missionReigon.gameObject.activeInHierarchy) _missionReigon.FoldMissionRegion();
            if (_deskReigon.gameObject.activeInHierarchy) _deskReigon.FoldDeskRegion();
            if (_showTutorialButton.gameObject.activeInHierarchy) FoldTutorialButton();
        }

        private void FoldTutorialButton()
        {
            _isButtonFold = !_isButtonFold;

            if (_isButtonFold) _tutorialBtnAnim.Play("Open", 0);
            else _tutorialBtnAnim.Play("Close", 0);
        }

        private void ShowTutorial()
        {
            var spawnClickVFX = FeedbackManager.Instance.SpawnClickButtonVFX();
            if (spawnClickVFX)
            {
                spawnClickVFX.transform.position = new Vector2(_showTutorialButton.transform.position.x, _showTutorialButton.transform.position.y);
            }

            PopupManager.Instance.GetGuidePopupObject().SetActive(true);
            PopupManager.Instance.GetGuidePopupScript().LoadGuide();
        }
        #endregion
    }
}