using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.Hangman
{
    /// <summary>
    /// Represents a behaviour that controls a scenario.
    /// </summary>
    public class ScenarioController : MonoBehaviour, IGameplaySectionController
    {
        /// <summary>
        /// The letter controller.
        /// </summary>
        [SerializeField]
        private LetterSectionController _letterController;

        /// <summary>
        /// The behaviours for the scenario parts in order.
        /// </summary>
        //[SerializeField]
        public ScenarioPartBehaviour[] _orderedParts;

        /// <summary>
        /// The behaviours for the scenario parts in order.
        /// </summary>
        public ScenarioPartBehaviour[] OrderedParts => _orderedParts;

        /// <summary>
        /// The amount of unlocked parts.
        /// </summary>
        public int UnlockedCount => _orderedParts.Count(part => part.Unlocked);

        /// <summary>
        /// The letter controller.
        /// </summary>
        public LetterSectionController LetterController => _letterController;

        /// <summary>
        /// Fired when the scenario has been completed.
        /// </summary>
        public Action Completed;

        /// <summary>
        /// The phrase used for the game.
        /// </summary>
        protected Phrase p_phrase;

        /// <summary>
        /// The settings used for the game.
        /// </summary>
        protected HangmanSettings p_settings;

        /// <summary>
        /// The service used for the game.
        /// </summary>
        protected HangmanService p_service;

        /// <summary>
        /// Whether the scenario has been completed.
        /// </summary>
        public bool _completed;

        /// <summary>
        /// Starts listening to letter click events.
        /// </summary>
        private void OnEnable() => _letterController.LetterClicked += OnLetterClicked;

        /// <summary>
        /// Stops listening to letter click events.
        /// </summary>
        private void OnDisable() => _letterController.LetterClicked -= OnLetterClicked;

        /// <summary>
        /// Checks whether all ordered parts have been completed to
        /// call on completion operations.
        /// </summary>
        /// 

        private void Update()
        {
            //// Do nothing if the scenario is already completed.
            //if (_completed)
            //    return;

            //// Do nothing if no ordered parts are part of the scenario.
            //if (_orderedParts.Length == 0)
            //    return;

            //// Check whether all ordered parts have been unlocked and return if any is not.
            //for (int i = 0; i < _orderedParts.Length; i++)
            //    if (!_orderedParts[i].Unlocked)
            //        return;

            //_completed = true;
            //OnCompletion();
            //Completed?.Invoke();
        }

        /// <summary>
        /// Regenerates the scenario.
        /// </summary>
        /// <param name="service">The game service.</param>
        /// <param name="list">The list the phrase is chosen from.</param>
        /// <param name="phrase">The phrase used for the game.</param>
        public void Generate(HangmanService service, HangmanSettings.PhraseList list, Phrase phrase)
        {
            _completed = false;

            p_service = service;
            p_settings = service.Settings;
            p_phrase = phrase;

            if (p_settings.BaseLivesOnScenarioParts)
                p_settings.Lives = _orderedParts.Length;
            else
                GeneratePartsBasedOnLives(p_settings);
        }

        /// <summary>
        /// Locks all scenario parts.
        /// </summary>
        public virtual void Clear()
        {
            for (int i = 0; i < _orderedParts.Length; i++)
                _orderedParts[i].Lock();
        }

        /// <summary>
        /// Should return a generated scenario part.
        /// </summary>
        /// <param name="index">The index of the scenario part in the array.</param>
        /// <param name="settings">The game settings.</param>
        /// <returns>The generated part.</returns>
        protected virtual ScenarioPartBehaviour GeneratePart(int index, HangmanSettings settings) => null;

        /// <summary>
        /// Called when the scenario has been completed.
        /// </summary>
        protected virtual void OnCompletion() { }

        /// <summary>
        /// Called when a letter has been clicked by the user
        /// to unlock a part of the scenario if necessary.
        /// </summary>
        /// <param name="letter">The clicked letter.</param>
        private void OnLetterClicked(char letter)
        {
            // Do nothing if the word contains the letter.
            if (p_phrase.Contains(letter))
                return;

            // Make up a list of behaviours unlocking and find the next behaviour to unlock.
            List<ScenarioPartBehaviour> behavioursUnlocking = new List<ScenarioPartBehaviour>();
            ScenarioPartBehaviour behaviourToUnlock = FindBehaviourUnlockStates(behavioursUnlocking);

            // If no unlock-able behavior can be found, return.
            if (behaviourToUnlock == null)
                return;

            // If there are behaviours unlocking, we wait for them to finish before adding the behaviour to unlock to the scenario.
            // If no other behaviours are unlocking, add the behaviour to unlock to the scenario.
            if (behavioursUnlocking.Count != 0)
                StartCoroutine(WaitForUnlockingBehaviours(behavioursUnlocking, behaviourToUnlock.Unlock));
            else
                behaviourToUnlock.Unlock(p_service);
        }

        /// <summary>
        /// Generates scenario parts based on lives amount set in the settings.
        /// </summary>
        /// <param name="settings">The game settings.</param>
        protected virtual void GeneratePartsBasedOnLives(HangmanSettings settings)
        {
            _orderedParts = new ScenarioPartBehaviour[settings.Lives];

            for (int i = 0; i < _orderedParts.Length; i++)
                _orderedParts[i] = GeneratePart(i, settings);
        }

        /// <summary>
        /// Looks for and returns a behaviour to unlock and fills a list of currently unlocking behaviours while doing so.
        /// </summary>
        /// <param name="behavioursUnlocking">The unlocking behaviours found.</param>
        /// <returns>The behaviour to unlock.</returns>
        private ScenarioPartBehaviour FindBehaviourUnlockStates(List<ScenarioPartBehaviour> behavioursUnlocking)
        {
            ScenarioPartBehaviour behaviourToUnlock = null;

            for (int i = 0; i < OrderedParts.Length; i++)
            {
                ScenarioPartBehaviour partBehaviour = _orderedParts[i];
                if (partBehaviour.Unlocking)
                {
                    behavioursUnlocking.Add(partBehaviour);
                    continue;
                }

                if (!partBehaviour.Unlocked)
                {
                    behaviourToUnlock = partBehaviour;
                    break;
                }
            }

            return behaviourToUnlock;
        }

        /// <summary>
        /// Waits for a given list of scenario part behaviours to be unlocked before
        /// calling a given action.
        /// </summary>
        /// <param name="unlockingBehaviours">The list of scenario part behaviours to wait for.</param>
        /// <param name="action">The action to invoke after waiting.</param>
        /// <returns>Waits until all scenario parts are unlocked.</returns>
        private IEnumerator WaitForUnlockingBehaviours(List<ScenarioPartBehaviour> unlockingBehaviours, Action<HangmanService> action)
        {
            while (unlockingBehaviours.All(behaviour => behaviour != null && !behaviour.Unlocked))
                yield return null;

            action.Invoke(p_service);
        }
    }
}