using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DTT.MinigameBase;
using DTT.MinigameBase.UI;
using Sirenix.OdinInspector;
using UnityEngine;
//using static DG.DemiEditor.DeEditorUtils;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace DTT.Hangman
{
   /// <summary>
   /// Provides the core gameplay services usable for a hangman game.
   /// </summary>
   public class HangmanService : MonoBehaviour, IMinigame<HangmanSettings, HangmanResult>, IFinishedable, IRestartable
   {
        /// <summary>
        /// The default settings used for the game.
        /// </summary>
        [SerializeField]
        [LabelText("Data")]
        private HangmanSettings _settings;

        /// <summary>
        /// The controller managing the letter section.
        /// </summary>
        [SerializeField]
    private LetterSectionController _letterSection;

      /// <summary>
      /// The controller managing the phrase section.
      /// </summary>
    [SerializeField]
    private PhraseSectionController _phraseSection;

      /// <summary>
      /// The controller managing the scenario.
      /// </summary>
    [SerializeField]
    private ScenarioController _scenario;

    private Phrase phrase;

      /// <summary>
      /// Whether the game is currently paused.
      /// </summary>
    public bool IsPaused => _isPaused;

      /// <summary>
      /// Whether the game is currently active.
      /// </summary>
    public bool IsGameActive => !_isPaused && _isActive;

      /// <summary>
      /// The elapsed amount of seconds since the game has started.
      /// 0 if no time constrained has been set.
      /// </summary>
    public double ElapsedSeconds => _stopwatch.Elapsed.TotalSeconds;

      /// <summary>
      /// The settings currently used for the game.
      /// </summary>
    public HangmanSettings Settings => _activeSettings != null ? _activeSettings : _settings;
      
      /// <summary>
      /// Called when the game has finished returning the result of the game.
      /// </summary>
    public event Action<HangmanResult> Finish;
    
      /// <summary>
      /// Called when the game has started.
      /// </summary>
    public event Action Started;

      /// <summary>
      /// Called when the game is paused.
      /// </summary>
    public event Action<bool> Paused;
      
      /// <summary>
      /// Called when the game is finished.
      /// </summary>
    public event Action Finished;

      /// <summary>
      /// The stopwatch used when a time constrained has been set.
      /// </summary>
    private readonly Stopwatch _stopwatch = new Stopwatch();

      /// <summary>
      /// The sections to be controlled for the game.
      /// </summary>
    private readonly List<IGameplaySectionController> _sections = new List<IGameplaySectionController>();

      /// <summary>
      /// Whether the game is currently paused.
      /// </summary>
    private bool _isPaused = false;

      /// <summary>
      /// Whether the game is active.
      /// </summary>
    private bool _isActive = false;

      /// <summary>
      /// The settings currently used for the game.
      /// </summary>
    private HangmanSettings _activeSettings;

      /// <summary>
      /// The current phrase used for the game.
      /// </summary>
    public Phrase CurrentPhrase { get; private set; }
    public int index = 0;

        [SerializeField]
        GameUI gameUI;

      /// <summary>
      /// Adds required sections.
      /// </summary>
    protected virtual void Awake()
    {
        //AddSection(_letterSection);
        //AddSection(_phraseSection);
        //AddSection(_scenario);
         
        //_phraseSection.PhraseCompleted += ForceFinish;
        //_scenario.Completed += ForceFinish;
    }

      /// <summary>
      /// Starts the game using given settings.
      /// </summary>
      /// <param name="settings">The settings to use for the game.</param>
    public void StartGame(HangmanSettings settings){
        if (IsGameActive){
            Debug.LogWarning("Can't start if a game is already active.");
            return;
        } 
        if (settings == null){
            Debug.LogWarning("Can't start when when the game settings are null.");
            return;
        }
        
        HangmanSettings.PhraseList list = RetrieveList(settings.GetPhraseList);
        phrase = default;
        bool phraseFound = false;

        if (settings.Random){
            // In random mode, keep selecting phrases until we find one that hasn't been passed
            int attempts = 0;
            int maxAttempts = list.PhraseCount; // Prevent infinite loop
            do{
                int randomIndex = Random.Range(0, list.PhraseCount);
                phrase = RetrieveNextWord(list, randomIndex);
                phraseFound = !phrase.pass;
                attempts++;
                index = randomIndex;
            } while (!phraseFound && attempts < maxAttempts);
        }
        else{
            // In sequential mode, find the next non-passed phrase
            for (int i = 0; i < list.PhraseCount; i++){
                phrase = RetrieveNextWord(list, i);
                if (!phrase.pass){
                    index = i;
                    phraseFound = true;
                    break;
                }
            }
        }

        // If no valid phrase was found, reset all passes and start from the beginning
        if (!phraseFound){
            Debug.Log("All phrases have been used. Resetting the list.");
            for (int i = 0; i < list.PhraseCount; i++)
                list.PassPhrase(i, false);
            phrase = RetrieveNextWord(list, 0);
            index = 0;
        }

        if (settings.ShowVowels)
            phrase = phrase.Expose(HangmanSettings.VOWELS.ToCharArray());
         
         // Generate sections.
        for(int i = 0; i < _sections.Count; i++)
            _sections[i].Generate(this, list, phrase);

        if(settings.UsesTimeConstrained)
            _stopwatch.Restart();

        CurrentPhrase = phrase;
                  
        _isActive = true;
        _activeSettings = settings;

        Started?.Invoke();
    }

        public bool IsBigWin(HangmanSettings settings)
        {
            HangmanSettings.PhraseList list = RetrieveList(settings.GetPhraseList);
            phrase = default;
            bool phraseFound = false;

            if (settings.Random)
            {
                // In random mode, keep selecting phrases until we find one that hasn't been passed
                int attempts = 0;
                int maxAttempts = list.PhraseCount; // Prevent infinite loop
                do
                {
                    int randomIndex = Random.Range(0, list.PhraseCount);
                    phrase = RetrieveNextWord(list, randomIndex);
                    phraseFound = !phrase.pass;
                    attempts++;
                } while (!phraseFound && attempts < maxAttempts);
            }
            else
            {
                // In sequential mode, find the next non-passed phrase
                for (int i = 0; i < list.PhraseCount; i++)
                {
                    phrase = RetrieveNextWord(list, i);
                    if (!phrase.pass)
                    {
                        phraseFound = true;
                        break;
                    }
                }
            }
            return !phraseFound;
        }

        public void SetAnswer(HangmanSettings settings)
        {
            HangmanSettings.PhraseList list = RetrieveList(settings.GetPhraseList);
            phrase = default;

            MiniGameEndController.instance.totalAnswer = list.PhraseCount;
            for (int i = 0; i < list.PhraseCount; i++)
            {
                MiniGameEndController.instance.SetTextForAnswer(i, list[i].value);
            }
        }

    /// <summary>
    /// Adds a section to the hangman service to receive callbacks.
    /// </summary>
    /// <param name="controller">The gameplay part</param>
    public void AddSection(IGameplaySectionController controller) => _sections.Add(controller);

    /// <summary>
    /// Starts the game using the default settings.
    /// </summary>
      public void StartGame() => StartGame(_settings);

      /// <summary>
      /// Restarts the game using the default settings.
      /// </summary>
      public void Restart() => Restart(_activeSettings);

        public bool IsBigWin() => IsBigWin(_settings);

        public void SetAnswer() => SetAnswer(_settings);

      /// <summary>
      /// Restarts the game using given settings.
      /// </summary>
      /// <param name="settings">The settings to use for the game.</param>
      public void Restart(HangmanSettings settings)
      {
            if (settings == null)
            {
                Debug.LogError("=================== setting == null");
                settings = _settings;
                AddSection(_letterSection);
                AddSection(_phraseSection);
                AddSection(_scenario);

                _phraseSection.PhraseCompleted += ForceFinish;
                _scenario.Completed += ForceFinish;
            }
        Cleanup();
        Continue();
        StartGame(settings);
      }

      /// <summary>
      /// Clears the game sections.
      /// </summary>
      public void Cleanup()
      {

        _isActive = false;
         
        for(int i = 0; i < _sections.Count; i++)
            _sections[i].Clear();

        Debug.LogError("================ clean up");
      }

      /// <summary>
      /// Does the stopwatch update.
      /// </summary>
    private void Update() => OnStopWatchUpdate();

      /// <summary>
      /// Forces a finish if the stopwatch is running and the total amount of seconds
      /// has become greater than the time constrained set.
      /// </summary>
    private void OnStopWatchUpdate()
    {
         if (!_stopwatch.IsRunning)
            return;
         
         if(_stopwatch.Elapsed.TotalSeconds >= _settings.TimeConstrained)
            {
                if (gameUI != null) gameUI.isWin = false;
                ForceFinish();
            }
            
    }

      /// <summary>
      /// Pauses the game.
      /// </summary>
    public void Pause()
    {
         if (_isPaused)
            return;
         
         if(_settings.UsesTimeConstrained)
            _stopwatch.Stop();

         _isPaused = true;
         Paused?.Invoke(true);
      }

      /// <summary>
      /// Continues the game.
      /// </summary>
      public void Continue()
      {
         if (!_isPaused)
            return;
         
         if(_settings.UsesTimeConstrained)
            _stopwatch.Start();

         _isPaused = false;
         Paused?.Invoke(false);
      }
      
      /// <summary>
      /// Forces the game to finish invoking all relevant events.
      /// </summary>
      public void ForceFinish()
      {
         if(_settings.UsesTimeConstrained)
            _stopwatch.Stop();
         
         _isActive = false;

            if (gameUI.isWin) StartCoroutine(DelayFinished());
            else
            {
                Finished?.Invoke();
                Finish?.Invoke(new HangmanResult
                {
                    finishedPhrase = _phraseSection.IsPhraseCompleted,
                    wrongGuesses = _scenario.UnlockedCount,
                    timeTaken = ElapsedSeconds
                });
            }
      }

        IEnumerator DelayFinished()
        {
            if (RetrieveList(_settings.GetPhraseList)._phrases[index].audible.Length > 0 && RetrieveList(_settings.GetPhraseList)._phrases[index].audible[0] != null)
            {
                if (MiniGameEndController.instance != null)
                {
                    MiniGameEndController.instance.PlayAudio(RetrieveList(_settings.GetPhraseList)._phrases[index].audible[0]);
                    yield return new WaitForSeconds(RetrieveList(_settings.GetPhraseList)._phrases[index].audible[0].length);
                }
            }
            else yield return null;
            Finished?.Invoke();
            Finish?.Invoke(new HangmanResult
            {
                finishedPhrase = _phraseSection.IsPhraseCompleted,
                wrongGuesses = _scenario.UnlockedCount,
                timeTaken = ElapsedSeconds
            });
        }

        /// <summary>
        /// Returns a phrase list to play the game with out of a given array of lists.
        /// By default this will return a random list from the array.
        /// </summary>
        /// <param name="lists">The lists to choose from.</param>
        /// <returns>The list to play the game with.</returns>
        protected virtual HangmanSettings.PhraseList RetrieveList(HangmanSettings.PhraseList phraseList) => phraseList;

      /// <summary>
      /// Returns a phrase to play the game with out of a given list of phrases.
      /// By default this will return a random phrase from the list.
      /// </summary>
      /// <param name="list">The list to choose a phrase from.</param>
      /// <returns>The phrase to play the game with.</returns>
      protected virtual Phrase RetrieveWord(HangmanSettings.PhraseList list) => list[Random.Range(0, list.PhraseCount)];
      protected virtual Phrase RetrieveNextWord(HangmanSettings.PhraseList list, int i) => list[i];
   }
}
