namespace DTT.Hangman
{
    /// <summary>
    /// Represents a behaviour controller a phrase section with a given type of phrase displayer.
    /// </summary>
    /// <typeparam name="T">The type of phrase displayer.</typeparam>
    public abstract class PhraseSectionController<T> : PhraseSectionController where T : PhraseDisplayer
    {
        /// <summary>
        /// The displayer of the phrase.
        /// </summary>
        protected T p_displayer;

        /// <summary>
        /// Returns whether the phrase is completed or not.
        /// </summary>
        /// <returns>Whether the phrase is completed or not.</returns>
        protected override bool GetIsPhraseCompleted() => p_displayer.IsFullPhraseDisplayed();

        /// <summary>
        /// Called when the word should be generated using multiple arguments.
        /// </summary>
        /// <param name="service">The hangman game service.</param>
        /// <param name="list">The list the phrase was chosen from.</param>
        /// <param name="phrase">The phrase to be generated.</param>
        protected internal override void OnGeneratePhrase(HangmanService service, HangmanSettings.PhraseList list, Phrase phrase)
        {
            HangmanSettings settings = service.Settings;
            p_displayer = CreatePhraseDisplay(settings, phrase);
            
            if(settings.ShowTheme)
                CreateThemeDisplay(list.Theme);
            
            p_displayer.Generate(service, list, phrase);
        }

        /// <summary>
        /// Creates the user interface displaying the phrase.
        /// </summary>
        /// <param name="settings">The game settings.</param>
        /// <param name="phrase">The phrase to display.</param>
        /// <returns>The display instance.</returns>
        protected abstract T CreatePhraseDisplay(HangmanSettings settings, Phrase phrase);

        /// <summary>
        /// Creates the user interface display the theme.
        /// </summary>
        /// <param name="theme">The theme to display.</param>
        protected abstract void CreateThemeDisplay(string theme);
    }
}
