namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;
    using Azoxia.Core.Extensions;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Aggregate root representing a worker profile and related employment metadata.
    /// </summary>
    public class Worker :
        DeletableEntityBase
    {
        #region Fields

        private readonly List<WorkerAvailability> _availabilities = new();
        private readonly List<WorkerCertificate> _certificates = new();
        private readonly List<WorkerEducation> _educations = new();
        private readonly List<WorkerExperience> _experiences = new();
        private readonly List<WorkerLanguage> _languages = new();
        private readonly List<WorkerReference> _references = new();
        private readonly List<WorkerSkill> _skills = new();

        #endregion Fields

        #region Ctors

        private Worker() { }

        protected internal Worker(int systemUserId)
        {
            SystemUserId = systemUserId;
        }

        #endregion Ctors

        #region Utils

        protected internal WorkerSkill AddSkill(string tag)
        {
            SkillTag normalizedTag = new(tag);

            WorkerSkill? skill = Skills
                .FirstOrDefault(x => x.Tag == normalizedTag);

            if (skill is null)
            {
                skill = new(Id, normalizedTag);
                _skills.Add(skill);
                return skill;
            }

            return skill;
        }

        #endregion Utils

        #region Properties
        /// <summary>
        /// UTC timestamp of the last skill-embedding refresh, if any.
        /// </summary>
        public DateTimeOffset? EmbeddingUpdatedAt { get; private set; }

        /// <summary>
        /// Declared nationality of the worker, if provided.
        /// </summary>
        public string? Nationality { get; private set; }

        /// <summary>
        /// Optional embedding vector derived from declared skills.
        /// </summary>
        public float[]? SkillEmbedding { get; private set; }

        /// <summary>
        /// Identifier of the linked application user.
        /// </summary>
        public int SystemUserId { get; private set; }

        /// <summary>
        /// University name associated with the worker, if applicable.
        /// </summary>
        public string? University { get; private set; }

        /// <summary>
        /// Linked application user account for this worker profile.
        /// </summary>
        public virtual SystemUser SystemUser { get; private set; }

        /// <summary>
        /// Weekly availability slots linked to this worker.
        /// </summary>
        public virtual IReadOnlyList<WorkerAvailability> Availabilities => _availabilities.AsReadOnly();

        /// <summary>
        /// Professional certifications declared by this worker.
        /// </summary>
        public virtual IReadOnlyList<WorkerCertificate> Certificates => _certificates.AsReadOnly();

        /// <summary>
        /// Education history entries for this worker.
        /// </summary>
        public virtual IReadOnlyList<WorkerEducation> Educations => _educations.AsReadOnly();

        /// <summary>
        /// Prior work experience records for this worker.
        /// </summary>
        public virtual IReadOnlyList<WorkerExperience> Experiences => _experiences.AsReadOnly();

        /// <summary>
        /// Languages spoken by this worker and proficiency levels.
        /// </summary>
        public virtual IReadOnlyList<WorkerLanguage> Languages => _languages.AsReadOnly();

        /// <summary>
        /// External references (contacts) supplied for this worker.
        /// </summary>
        public virtual IReadOnlyList<WorkerReference> References => _references.AsReadOnly();

        /// <summary>
        /// Skill tags associated with this worker.
        /// </summary>
        public virtual IReadOnlyList<WorkerSkill> Skills => _skills.AsReadOnly();
        #endregion Properties
    }
}
