namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Self-reported spoken-language proficiency scale for workers.
    /// </summary>
    public enum LanguageLevel
    {
        /// <summary>
        /// Near-native professional fluency.
        /// </summary>
        Advanced = 50,

        /// <summary>
        /// Foundational exposure; limited practical use.
        /// </summary>
        Beginner = 10,

        /// <summary>
        /// Simple conversations and routine phrases.
        /// </summary>
        Elementary = 20,

        /// <summary>
        /// Handles typical work and social situations with support.
        /// </summary>
        Intermediate = 30,

        /// <summary>
        /// Native or bilingual mastery.
        /// </summary>
        Native = 60,

        /// <summary>
        /// Fluent in most contexts; minor gaps remain.
        /// </summary>
        UpperIntermediate = 40,
    }
}
