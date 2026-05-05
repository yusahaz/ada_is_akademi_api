namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;
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

        protected Worker() { }

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

        protected internal WorkerAvailability AddAvailability(DayOfWeek dayOfWeek, TimeOnly timeFrom, TimeOnly timeTo)
        {
            WorkerAvailability? existing = Availabilities.FirstOrDefault(x =>
                x.DayOfWeek == dayOfWeek &&
                x.TimeFrom == timeFrom &&
                x.TimeTo == timeTo);
            if (existing is not null)
            {
                return existing;
            }

            WorkerAvailability availability = new(Id, dayOfWeek, timeFrom, timeTo);
            _availabilities.Add(availability);
            return availability;
        }

        protected internal void RemoveAvailability(int availabilityId)
        {
            WorkerAvailability? availability = Availabilities.FirstOrDefault(x => x.Id == availabilityId);
            availability = availability.ThrowIfNull(DomainErrorCodes.WorkerProfileItemNotFound);
            _availabilities.Remove(availability);
        }

        protected internal WorkerCertificate AddCertificate(
            string name,
            string issuingOrganization,
            DateOnly issuedAt,
            DateOnly? expiresAt,
            string? documentUrl = null)
        {
            string normalizedName = name.Trim();
            string normalizedIssuer = issuingOrganization.Trim();

            WorkerCertificate? existing = Certificates.FirstOrDefault(x =>
                x.Name == normalizedName &&
                x.IssuingOrganization == normalizedIssuer &&
                x.IssuedAt == issuedAt);
            if (existing is not null)
            {
                return existing;
            }

            WorkerCertificate certificate = new(Id, name, issuingOrganization, issuedAt, expiresAt, documentUrl);
            _certificates.Add(certificate);
            return certificate;
        }

        protected internal void RemoveCertificate(int certificateId)
        {
            WorkerCertificate? certificate = Certificates.FirstOrDefault(x => x.Id == certificateId);
            certificate = certificate.ThrowIfNull(DomainErrorCodes.WorkerProfileItemNotFound);
            _certificates.Remove(certificate);
        }

        protected internal WorkerEducation AddEducation(
            string school,
            string department,
            EducationType educationType,
            int startYear,
            int? endYear,
            bool isOngoing)
        {
            string normalizedSchool = school.Trim();
            string normalizedDepartment = department.Trim();

            WorkerEducation? existing = Educations.FirstOrDefault(x =>
                x.School == normalizedSchool &&
                x.Department == normalizedDepartment &&
                x.EducationType == educationType &&
                x.StartYear == startYear &&
                x.EndYear == endYear &&
                x.IsOngoing == isOngoing);
            if (existing is not null)
            {
                return existing;
            }

            WorkerEducation education = new(Id, school, department, educationType, startYear, endYear, isOngoing);
            _educations.Add(education);
            return education;
        }

        protected internal void RemoveEducation(int educationId)
        {
            WorkerEducation? education = Educations.FirstOrDefault(x => x.Id == educationId);
            education = education.ThrowIfNull(DomainErrorCodes.WorkerProfileItemNotFound);
            _educations.Remove(education);
        }

        protected internal WorkerExperience AddExperience(
            string companyName,
            string position,
            DateOnly startDate,
            DateOnly? endDate,
            string? description = null)
        {
            string normalizedCompany = companyName.Trim();
            string normalizedPosition = position.Trim();

            WorkerExperience? existing = Experiences.FirstOrDefault(x =>
                x.CompanyName == normalizedCompany &&
                x.Position == normalizedPosition &&
                x.StartDate == startDate &&
                x.EndDate == endDate);
            if (existing is not null)
            {
                return existing;
            }

            WorkerExperience experience = new(Id, companyName, position, startDate, endDate, description);
            _experiences.Add(experience);
            return experience;
        }

        protected internal void RemoveExperience(int experienceId)
        {
            WorkerExperience? experience = Experiences.FirstOrDefault(x => x.Id == experienceId);
            experience = experience.ThrowIfNull(DomainErrorCodes.WorkerProfileItemNotFound);
            _experiences.Remove(experience);
        }

        protected internal WorkerLanguage AddLanguage(string language, LanguageLevel level)
        {
            string normalizedLanguage = language.Trim();

            WorkerLanguage? existing = Languages.FirstOrDefault(x =>
                x.Language == normalizedLanguage &&
                x.Level == level);
            if (existing is not null)
            {
                return existing;
            }

            WorkerLanguage workerLanguage = new(Id, language, level);
            _languages.Add(workerLanguage);
            return workerLanguage;
        }

        protected internal void RemoveLanguage(int languageId)
        {
            WorkerLanguage? language = Languages.FirstOrDefault(x => x.Id == languageId);
            language = language.ThrowIfNull(DomainErrorCodes.WorkerProfileItemNotFound);
            _languages.Remove(language);
        }

        protected internal WorkerReference AddReference(string company, string position, Contact contact)
        {
            string normalizedCompany = company.Trim();
            string normalizedPosition = position.Trim();

            WorkerReference? existing = References.FirstOrDefault(x =>
                x.Company == normalizedCompany &&
                x.Position == normalizedPosition &&
                x.Contact.Email == contact.Email &&
                x.Contact.FirstName == contact.FirstName &&
                x.Contact.LastName == contact.LastName &&
                x.Contact.Phone == contact.Phone);
            if (existing is not null)
            {
                return existing;
            }

            WorkerReference reference = new(Id, company, position, contact);
            _references.Add(reference);
            return reference;
        }

        protected internal void RemoveReference(int referenceId)
        {
            WorkerReference? reference = References.FirstOrDefault(x => x.Id == referenceId);
            reference = reference.ThrowIfNull(DomainErrorCodes.WorkerProfileItemNotFound);
            _references.Remove(reference);
        }

        protected internal void RemoveSkill(int skillId)
        {
            WorkerSkill? skill = Skills.FirstOrDefault(x => x.Id == skillId);
            skill = skill.ThrowIfNull(DomainErrorCodes.WorkerProfileItemNotFound);
            _skills.Remove(skill);
        }

        protected internal void UpdateProfile(string? nationality, string? university)
        {
            Nationality = nationality.IsNullOrWhiteSpace()
                ? null
                : nationality.Trim();
            University = university.IsNullOrWhiteSpace()
                ? null
                : university.Trim();
        }

        protected internal void UpdateSkillEmbedding(float[]? skillEmbedding)
        {
            if (skillEmbedding is null || skillEmbedding.Length == 0)
            {
                SkillEmbedding = null;
                EmbeddingUpdatedAt = null;
                return;
            }

            SkillEmbedding = skillEmbedding;
            EmbeddingUpdatedAt = DateTimeOffset.UtcNow;
        }

        protected internal void DeleteWorker()
            => base.Delete();

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
