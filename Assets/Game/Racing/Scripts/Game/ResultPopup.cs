using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Novastars.MiniGame.DuaXe
{
    public class ResultPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _resultTimeTMP;
        [SerializeField] private GameObject game;

        #region Public Method
        public void Replay()
        {
            BaseGameUI.Instance.GetGameplayUILogic().RestartGame();
        }

        public void Continue()
        {
            game.SetActive(false);
        }

        public void ShowResult(string text)
        {
            _resultTimeTMP.text = text;
        }
        #endregion
    }
}