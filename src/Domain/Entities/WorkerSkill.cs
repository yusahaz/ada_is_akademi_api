namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// A skill tag attached to a worker profile with creation metadata.
    /// </summary>
    public class WorkerSkill :
        EntityBase
    {
        #region Ctors

        private WorkerSkill() { }

        protected internal WorkerSkill(
            int workerId,
            string tag)
        {
            WorkerId = workerId;
            Tag = new SkillTag(tag);
            CreatedAt = DateTimeOffset.UtcNow;
        }

        #endregion Ctors

        #region Properties
        /// <summary>
        /// UTC timestamp when this skill tag was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// Normalized skill label or keyword.
        /// </summary>
        public SkillTag Tag { get; private set; }

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
