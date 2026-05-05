namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Language competency declared by a worker.
    /// </summary>
    public class WorkerLanguage :
        EntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected WorkerLanguage() { }

        /// <summary>
        /// Creates a language proficiency row.
        /// </summary>
        /// <param name="workerId">Owning worker key.</param>
        /// <param name="language">Language name.</param>
        /// <param name="level">Proficiency level.</param>
        protected internal WorkerLanguage(
            int workerId,
            string language,
            LanguageLevel level)
        {
            WorkerId = workerId;
            Language = language;
            Level = level;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// ISO-style or display name of the language.
        /// </summary>
        public string Language { get; private set; }

        /// <summary>
        /// Self-reported proficiency band for the language.
        /// </summary>
        public LanguageLevel Level { get; private set; }

        /// <summary>
        /// Foreign key to the owning worker.
        /// </summary>
        public int WorkerId { get; private set; }


        /// <summary>
        /// Owning worker aggregate.
        /// </summary>
        public virtual Worker Worker { get; private set; }

        #endregion Properties
    }
}
