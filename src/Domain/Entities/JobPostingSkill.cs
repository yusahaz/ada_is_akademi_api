namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Represents a required or optional skill tag for a job posting.
    /// </summary>
    public class JobPostingSkill :
        EntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected JobPostingSkill() { }

        /// <summary>
        /// Creates a skill requirement row for a posting.
        /// </summary>
        /// <param name="jobPostingId">Owning posting key.</param>
        /// <param name="tag">Skill tag text.</param>
        /// <param name="isRequired">Whether the skill is mandatory.</param>
        protected internal JobPostingSkill(
            int jobPostingId,
            string tag,
            bool isRequired)
        {
            JobPostingId = jobPostingId;
            Tag = new SkillTag(tag);
            IsRequired = isRequired;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Indicates whether this skill is mandatory for the posting.
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// Owning job posting identifier.
        /// </summary>
        public int JobPostingId { get; private set; }

        /// <summary>
        /// Skill label used for matching and filtering.
        /// </summary>
        public SkillTag Tag { get; private set; }


        /// <summary>
        /// Job posting that owns this skill requirement.
        /// </summary>
        public virtual JobPosting JobPosting { get; private set; }

        #endregion Properties
    }
}
