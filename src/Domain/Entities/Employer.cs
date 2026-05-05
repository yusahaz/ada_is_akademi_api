namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;

    /// <summary>
    /// Represents an employer organization and its operational profile.
    /// </summary>
    public class Employer :
        CodedNamedEntityBase
    {
        #region Fields

        private readonly List<EmployerLocation> _locations = new();
        private readonly List<JobPosting> _jobPostings = new();
        private readonly List<ShiftSupervisor> _supervisors = new();

        #endregion Fields

        #region Ctors

        protected Employer() { }

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

        protected internal ShiftSupervisor AddShiftSupervisor(int systemUserId, int? locationId = null)
        {
            ShiftSupervisor? existing = Supervisors
                .FirstOrDefault(x => x.SystemUserId == systemUserId);

            if (existing is null)
            {
                ShiftSupervisor supervisor = new(Id, systemUserId, locationId);
                _supervisors.Add(supervisor);
                return supervisor;
            }

            return existing;
        }

        protected internal void DeleteEmployer()
            => base.Delete();

        protected internal void RemoveShiftSupervisor(int systemUserId)
        {
            ShiftSupervisor? supervisor = Supervisors
                .FirstOrDefault(x => x.SystemUserId == systemUserId);

            if (supervisor is not null)
            {
                supervisor.Deactivate();
            }
        }

        protected internal void SetAddress(Address address)
        {
            Address = address;
        }

        protected internal void SetAsActive()
        {
            (Status != EmployerStatus.Banned)
                .ThrowIfFalse(DomainErrorCodes.EmployerInvalidStatusTransition);
            Status = EmployerStatus.Active;
        }

        protected internal void SetAsBanned()
            => Status = EmployerStatus.Banned;

        protected internal void SetAsSuspended()
        {
            (Status != EmployerStatus.Banned)
                .ThrowIfFalse(DomainErrorCodes.EmployerInvalidStatusTransition);
            Status = EmployerStatus.Suspended;
        }

        protected internal void SetCommissionRate(decimal commissionRate)
        {
            (commissionRate >= 0m && commissionRate <= 1m)
                .ThrowIfFalse(DomainErrorCodes.EmployerCommissionRateOutOfRange);
            CommissionRate = commissionRate;
        }

        protected internal void SetContact(Contact contact)
        {
            Contact = contact;
        }

        protected internal void UpdateEmployer(string name, string taxNumber, string? description = null)
        {
            UpdateName(name, description);
            TaxNumber = new TaxNumber(taxNumber);
        }

        protected internal void SetLogoObjectKey(string? objectKey)
        {
            LogoObjectKey = objectKey.IsNullOrWhiteSpace()
                ? null
                : objectKey.Trim();
        }

        #endregion Utils

        #region Properties
        /// <summary>
        /// Postal or legal address of the employer.
        /// </summary>
        public Address Address { get; private set; }

        /// <summary>
        /// Primary contact details for the employer.
        /// </summary>
        public Contact Contact { get; private set; }

        /// <summary>
        /// Commission rate applied for monetization calculations (0..1).
        /// </summary>
        public decimal CommissionRate { get; private set; }

        /// <summary>
        /// Current lifecycle status of the employer record.
        /// </summary>
        public EmployerStatus Status { get; private set; }

        /// <summary>
        /// Tax identifier associated with the employer.
        /// </summary>
        public TaxNumber TaxNumber { get; private set; }

        /// <summary>
        /// MinIO / S3 object key for employer logo, when configured.
        /// </summary>
        public string? LogoObjectKey { get; private set; }

        /// <summary>
        /// Registered locations under this employer.
        /// </summary>
        public virtual IReadOnlyList<EmployerLocation> Locations => _locations.AsReadOnly();

        /// <summary>
        /// Job postings created by this employer.
        /// </summary>
        public virtual IReadOnlyList<JobPosting> JobPostings => _jobPostings.AsReadOnly();

        /// <summary>
        /// Shift supervisors assigned to this employer.
        /// </summary>
        public virtual IReadOnlyList<ShiftSupervisor> Supervisors => _supervisors.AsReadOnly();
        #endregion Properties
    }
}