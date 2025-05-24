using System;
using UnityEngine;
using DTT.MinigameBase.UI;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// Controls the scenario for the sunflower demo scene.
    /// </summary>
    public class SunflowerScenarioController : ScenarioController
    {
        /// <summary>
        /// The sprite renderer of the leaves.
        /// </summary>
        [SerializeField]
        private SpriteRenderer _leavesRenderer;

        /// <summary>
        /// The prefab of the petals.
        /// </summary>
        [SerializeField]
        private GameObject _petalPrefab;

        /// <summary>
        /// The transform of the crown of the sunflower.
        /// </summary>
        [SerializeField]
        private Transform _crownTransform;

        /// <summary>
        /// The sprite renderer of the leaves.
        /// </summary>
        public SpriteRenderer LeavesRenderer => _leavesRenderer;

        /// <summary>
        /// Generates the petal behaviours.
        /// </summary>
        /// <param name="index">The index in the ordered array.</param>
        /// <param name="settings">The hangman settings.</param>
        /// <returns>The scenario part behaviour.</returns>
        /// 
        public static SunflowerScenarioController instance;

        [SerializeField]
        GameUI gameUI;
        [SerializeField]
        OrbitChildren orbitChildren;

        private void Awake()
        {
            if (instance == null) instance = this;
        }

        public void EndGame()
        {
            //// Do nothing if the scenario is already completed.
            if (_completed)
                return;

            //// Do nothing if no ordered parts are part of the scenario.
            if (_orderedParts.Length == 0)
                return;

            //// Check whether all ordered parts have been unlocked and return if any is not.
            for (int i = 0; i < _orderedParts.Length; i++)
                if (!_orderedParts[i].Unlocked)
                    return;

            _completed = true;
            gameUI.isWin = false;
            OnCompletion();
            Completed?.Invoke();
        }
        protected override ScenarioPartBehaviour GeneratePart(int index, HangmanSettings settings)
            => Instantiate(_petalPrefab, _crownTransform).GetComponent<PetalBehaviour>();

        protected override void GeneratePartsBasedOnLives(HangmanSettings settings)
        {
            base.GeneratePartsBasedOnLives(settings);
            Debug.LogError("=============== orbit");
            orbitChildren.InvokeSetOrbitRotations();
        }

        /// <summary>
        /// Clears the petals by destroying them and resetting the ordered parts to an empty array.
        /// </summary>
        public override void Clear()
        {
            for (int i = _crownTransform.childCount - 1; i >= 0; i--)
                Destroy(_crownTransform.GetChild(i).gameObject);
            //foreach (Transform tran in _crownTransform)
            //{
            //    DestroyImmediate(tran.gameObject);
            //}
            _orderedParts = Array.Empty<ScenarioPartBehaviour>();
            Debug.LogError("======================== crownChildCount = " + _crownTransform.childCount);
            Debug.LogError("======================== PartChildCount = " +  _orderedParts.Length);
        }
    }
}
