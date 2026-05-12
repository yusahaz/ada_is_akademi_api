namespace Azoxia.AdaIsAkademi.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Azoxia.Core.Domain;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;

    /// <summary>
    /// Represents an employer organization and its operational profile.
    /// </summary>
    public class Employer :
        CodedNamedEntityBase
    {
        #region Fields

        private readonly List<JobPosting> _jobPostings = new();
        private readonly List<EmployerLocation> _locations = new();
        private readonly List<EmployerSocialLink> _socialLinks = new();
        private readonly List<Supervisor> _supervisors = new();

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected Employer() { }

        /// <summary>
        /// Creates a new employer with tax identifier and initial lifecycle state.
        /// </summary>
        /// <param name="name">Display name.</param>
        /// <param name="description">Optional description.</param>
        /// <param name="taxNumber">Tax number text.</param>
        protected internal Employer(
            string name,
            string? description,
            string taxNumber) :
            base(name, description)
        {
            CommissionRate = 0.10m;
            Status = EmployerStatus.Pending;
            TaxNumber = new TaxNumber(taxNumber);
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Creates a new draft job posting owned by this employer at the given location.
        /// </summary>
        /// <param name="employerLocationId">Identifier of an <see cref="EmployerLocation"/> under this employer.</param>
        /// <param name="jobCategoryId">Job category identifier.</param>
        /// <param name="title">Posting title.</param>
        /// <param name="description">Posting description.</param>
        /// <param name="shiftDate">Calendar date of the shift.</param>
        /// <param name="shiftStartTime">Shift start time.</param>
        /// <param name="shiftEndTime">Shift end time.</param>
        /// <param name="wage">Wage offered for the shift.</param>
        /// <param name="headCount">Number of workers requested.</param>
        /// <returns>The new draft posting.</returns>
        protected internal JobPosting AddJobPosting(
            int employerLocationId,
            int jobCategoryId,
            string title,
            string description,
            DateOnly shiftDate,
            TimeOnly shiftStartTime,
            TimeOnly shiftEndTime,
            Money wage,
            int headCount)
        {
            (Status == EmployerStatus.Active)
                .ThrowIfFalse(DomainErrorCodes.EmployerCannotCreateJobPosting);

            EmployerLocation? location = Locations
                .FirstOrDefault(x => x.Id == employerLocationId);
            location = location.ThrowIfNull(DomainErrorCodes.EmployerLocationNotFound);

            (shiftEndTime > shiftStartTime)
                .ThrowIfFalse(DomainErrorCodes.JobPostingInvalidShiftTimes);

            (headCount > 0)
                .ThrowIfFalse(DomainErrorCodes.JobPostingHeadCountInvalid);

            JobPosting posting = new(
                Id,
                employerLocationId,
                jobCategoryId,
                title,
                description,
                shiftDate,
                shiftStartTime,
                shiftEndTime,
                wage,
                headCount);

            _jobPostings.Add(posting);
            return posting;
        }

        /// <summary>
        /// Adds or returns an existing location with the same display name.
        /// </summary>
        protected internal EmployerLocation AddLocation(string name, string? description = null)
        {
            EmployerLocation? location = Locations
                .FirstOrDefault(x => x.Name == name);

            if (location is null)
            {
                location = new(Id, name, description);
                _locations.Add(location);
            }

            return location;
        }

        /// <summary>
        /// Adds or returns an existing supervisor link for the user.
        /// </summary>
        protected internal Supervisor AddSupervisor(int systemUserId, int? locationId = null)
        {
            Supervisor? existing = Supervisors
                .FirstOrDefault(x => x.SystemUserId == systemUserId);

            if (existing is null)
            {
                Supervisor supervisor = new(Id, systemUserId, locationId);
                _supervisors.Add(supervisor);
                return supervisor;
            }

            return existing;
        }

        /// <summary>
        /// Soft-deletes this employer through the base lifecycle API.
        /// </summary>
        protected internal void DeleteEmployer()
            => base.Delete();

        /// <summary>
        /// Deactivates a supervisor assignment for the given user when present.
        /// </summary>
        protected internal void RemoveSupervisor(int systemUserId)
        {
            Supervisor? supervisor = Supervisors
                .FirstOrDefault(x => x.SystemUserId == systemUserId);

            if (supervisor is not null)
            {
                supervisor.Deactivate();
            }
        }

        /// <summary>
        /// Replaces outbound social links, keeping the last URL per platform.
        /// </summary>
        protected internal void ReplaceSocialLinks(IReadOnlyList<EmployerSocialLinkInput> links)
        {
            links = links.ThrowIfNull(AzoxiaErrorCodes.ArgumentNull);
            _socialLinks.Clear();
            IEnumerable<EmployerSocialLinkInput> distinctByPlatform =
                links
                    .GroupBy(x => x.Platform)
                    .Select(g => g.Last());
            foreach (EmployerSocialLinkInput row in distinctByPlatform)
            {
                _socialLinks.Add(new EmployerSocialLink(Id, row.Platform, row.Url));
            }
        }

        /// <summary>
        /// Replaces the embedded postal or legal address.
        /// </summary>
        protected internal void SetAddress(Address address)
        {
            Address = address;
        }

        /// <summary>
        /// Transitions the employer to active when not banned.
        /// </summary>
        protected internal void SetAsActive()
        {
            (Status != EmployerStatus.Banned)
                .ThrowIfFalse(DomainErrorCodes.EmployerInvalidStatusTransition);
            Status = EmployerStatus.Active;
        }

        /// <summary>
        /// Permanently bans the employer organization.
        /// </summary>
        protected internal void SetAsBanned()
            => Status = EmployerStatus.Banned;

        /// <summary>
        /// Suspends the employer when not banned.
        /// </summary>
        protected internal void SetAsSuspended()
        {
            (Status != EmployerStatus.Banned)
                .ThrowIfFalse(DomainErrorCodes.EmployerInvalidStatusTransition);
            Status = EmployerStatus.Suspended;
        }

        /// <summary>
        /// Updates commission rate within the inclusive 0..1 bounds.
        /// </summary>
        protected internal void SetCommissionRate(decimal commissionRate)
        {
            (commissionRate >= 0m && commissionRate <= 1m)
                .ThrowIfFalse(DomainErrorCodes.EmployerCommissionRateOutOfRange);
            CommissionRate = commissionRate;
        }

        /// <summary>
        /// Replaces primary employer contact details.
        /// </summary>
        protected internal void SetContact(Contact contact)
        {
            Contact = contact;
        }

        /// <summary>
        /// Sets or clears the logo object key after trimming.
        /// </summary>
        protected internal void SetLogoObjectKey(string? objectKey)
        {
            LogoObjectKey = objectKey.IsNullOrWhiteSpace()
                ? null
                : objectKey.Trim();
        }

        /// <summary>
        /// Updates display name, tax number, and optional description.
        /// </summary>
        protected internal void UpdateEmployer(string name, string taxNumber, string? description = null)
        {
            UpdateName(name, description);
            TaxNumber = new TaxNumber(taxNumber);
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Postal or legal address of the employer.
        /// </summary>
        public Address Address { get; private set; }

        /// <summary>
        /// Commission rate applied for monetization calculations (0..1).
        /// </summary>
        public decimal CommissionRate { get; private set; }

        /// <summary>
        /// Primary contact details for the employer.
        /// </summary>
        public Contact Contact { get; private set; }

        /// <summary>
        /// MinIO / S3 object key for employer logo, when configured.
        /// </summary>
        public string? LogoObjectKey { get; private set; }

        /// <summary>
        /// Current lifecycle status of the employer record.
        /// </summary>
        public EmployerStatus Status { get; private set; }

        /// <summary>
        /// Tax identifier associated with the employer.
        /// </summary>
        public TaxNumber TaxNumber { get; private set; }


        /// <summary>
        /// Job postings created by this employer.
        /// </summary>
        public virtual IReadOnlyList<JobPosting> JobPostings => _jobPostings.AsReadOnly();

        /// <summary>
        /// Registered locations under this employer.
        /// </summary>
        public virtual IReadOnlyList<EmployerLocation> Locations => _locations.AsReadOnly();

        /// <summary>
        /// Outbound social/profile links maintained by the employer on employer-facing read models.
        /// </summary>
        public virtual IReadOnlyList<EmployerSocialLink> SocialLinks =>
            _socialLinks.AsReadOnly();

        /// <summary>
        /// Shift supervisors assigned to this employer.
        /// </summary>
        public virtual IReadOnlyList<Supervisor> Supervisors => _supervisors.AsReadOnly();

        #endregion Properties
    }
}
