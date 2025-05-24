using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Novastars.MiniGame.LatBai
{
    public class MissionRegion : MonoBehaviour
    {
        [Header("Mission Prefab")]
        [SerializeField] private GameObject _missionPrefab;
        [SerializeField] private Transform _missionContentParrent;

        [Header("Animation")]
        [SerializeField] private Animator _missionRegionAnim;
        [SerializeField] private bool _isRegionOpen = true;

        [Header("UI Element")]
        [SerializeField] private Image _foldMissionImg;
        [SerializeField] private Image _pannelMissionImg;

        #region Unity Funcion
        private void Awake() => _missionRegionAnim = GetComponent<Animator>();
        private void Start()
        {
            gameObject.SetActive(UIManager.Instance.TurnOnMissonInGame);
            _pannelMissionImg.sprite = UIManager.Instance.MissionPannelSprite;

            SetupAllMission();
        }
        #endregion

        #region Public Method
        public void FoldMissionRegion()
        {
            _isRegionOpen = !_isRegionOpen;
            _foldMissionImg.sprite = _isRegionOpen ? UIManager.Instance.ArrowLeft : UIManager.Instance.ArrowRight;

            if (_isRegionOpen) _missionRegionAnim.Play("Open", 0);
            else _missionRegionAnim.Play("Close", 0);
        }
        #endregion

        #region Private Method
        private void SetupAllMission()
        {
            var missionDatas = DataManager.Instance.MissionDatas;

            for (int index = 0; index < missionDatas.Count; index++)
            {
                var missionScript = Instantiate(_missionPrefab, _missionContentParrent).GetComponent<Mission>();
                missionScript.Settup(missionDatas[index].MissionQuestion, missionDatas[index].MissionAnswer, index + 1);
            }
        }
        #endregion
    }
}