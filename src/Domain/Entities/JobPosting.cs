namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.AdaIsAkademi.Domain.Events;
    using Azoxia.Core.Domain;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;

    /// <summary>
    /// Represents an open or draft job posting created by an employer.
    /// </summary>
    public class JobPosting :
        DeletableAggregateRoot
    {
        #region Fields

        private readonly List<JobApplication> _applications = new();
        private readonly List<JobPostingSkill> _skills = new();

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected JobPosting() { }

        /// <summary>
        /// Creates a draft posting with shift and wage details.
        /// </summary>
        protected internal JobPosting(
            int employerId,
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
            EmployerId = employerId;
            EmployerLocationId = employerLocationId;
            JobCategoryId = jobCategoryId;
            Title = title;
            Description = description;
            ShiftDate = shiftDate;
            ShiftStartTime = shiftStartTime;
            ShiftEndTime = shiftEndTime;
            Wage = wage;
            HeadCount = headCount;
            Status = JobPostingStatus.Draft;
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Accepts an application and may transition posting to filled when capacity is reached.
        /// </summary>
        protected internal void AcceptApplication(int applicationId)
        {
            (Status == JobPostingStatus.Open || Status == JobPostingStatus.Filled)
                .ThrowIfFalse(DomainErrorCodes.JobPostingInvalidStatusTransition);

            int acceptedCount = Applications.Count(x => x.Status == JobApplicationStatus.Accepted);
            (acceptedCount < HeadCount)
                .ThrowIfFalse(DomainErrorCodes.JobPostingCapacityReached);

            JobApplication application = Applications
                .FirstOrDefault(x => x.Id == applicationId);
            application = application.ThrowIfNull(DomainErrorCodes.JobApplicationNotFound);

            application.Accept();

            RaiseDomainEvent(() => new JobApplicationAcceptedEvent(Id, EmployerId, applicationId, application.WorkerId));

            if (Applications.Count(x => x.Status == JobApplicationStatus.Accepted) >= HeadCount)
            {
                if (Status != JobPostingStatus.Filled)
                {
                    Status = JobPostingStatus.Filled;
                    RaiseDomainEvent(() => new JobPostingFilledEvent(Id, EmployerId));
                }
            }
        }

        /// <summary>
        /// Creates or returns an existing application when posting is open and rules pass.
        /// </summary>
        protected internal JobApplication AddApplication(int workerId, bool hasConflictingShift, string? note = null)
        {
            (Status == JobPostingStatus.Open)
                .ThrowIfFalse(DomainErrorCodes.JobPostingInvalidStatusTransition);
            hasConflictingShift.ThrowIfTrue(DomainErrorCodes.WorkerHasConflictingShift);
            (ShiftDate >= DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date))
                .ThrowIfFalse(DomainErrorCodes.JobPostingShiftDatePassed);

            if (Applications.Any(x => x.WorkerId == workerId))
            {
                return Applications.First(x => x.WorkerId == workerId);
            }

            JobApplication application = new(Id, workerId, note);
            _applications.Add(application);
            RaiseDomainEvent(() => new JobApplicationSubmittedEvent(Id, EmployerId, workerId, application.AppliedAt));
            return application;
        }

        /// <summary>
        /// Adds or returns a skill requirement row for the posting.
        /// </summary>
        protected internal JobPostingSkill AddSkill(string tag, bool isRequired)
        {
            SkillTag normalizedTag = new(tag);

            JobPostingSkill? skill = Skills
                .FirstOrDefault(x => x.Tag == normalizedTag);

            if (skill is null)
            {
                skill = new(Id, normalizedTag, isRequired);
                _skills.Add(skill);
                return skill;
            }

            return skill;
        }

        /// <summary>
        /// Cancels the posting from draft, open, or filled states.
        /// </summary>
        protected internal void Cancel()
        {
            (Status == JobPostingStatus.Draft
                || Status == JobPostingStatus.Open
                || Status == JobPostingStatus.Filled)
                .ThrowIfFalse(DomainErrorCodes.JobPostingInvalidStatusTransition);
            Status = JobPostingStatus.Cancelled;
            RaiseDomainEvent(() => new JobPostingCancelledEvent(Id, EmployerId));
        }

        /// <summary>
        /// Completes the posting from open or filled states.
        /// </summary>
        protected internal void Complete()
        {
            (Status == JobPostingStatus.Open || Status == JobPostingStatus.Filled)
                .ThrowIfFalse(DomainErrorCodes.JobPostingInvalidStatusTransition);
            Status = JobPostingStatus.Completed;
            RaiseDomainEvent(() => new JobPostingCompletedEvent(Id, EmployerId));
        }

        /// <summary>
        /// Soft-deletes this posting through the base lifecycle API.
        /// </summary>
        protected internal void DeleteJobPosting()
        {
            base.Delete();
        }

        /// <summary>
        /// Publishes a draft posting to the open state.
        /// </summary>
        protected internal void Publish()
        {
            (Status == JobPostingStatus.Draft)
                .ThrowIfFalse(DomainErrorCodes.JobPostingInvalidStatusTransition);
            Status = JobPostingStatus.Open;
            RaiseDomainEvent(() => new JobPostingPublishedEvent(Id, EmployerId));
        }

        /// <summary>
        /// Rejects an application and may reopen a filled posting when appropriate.
        /// </summary>
        protected internal void RejectApplication(int applicationId, string? reason = null)
        {
            (Status == JobPostingStatus.Open || Status == JobPostingStatus.Filled)
                .ThrowIfFalse(DomainErrorCodes.JobPostingInvalidStatusTransition);

            JobApplication application = Applications
                .FirstOrDefault(x => x.Id == applicationId);
            application = application.ThrowIfNull(DomainErrorCodes.JobApplicationNotFound);

            bool wasAccepted = application.Status == JobApplicationStatus.Accepted;
            application.Reject(reason);

            RaiseDomainEvent(() => new JobApplicationRejectedEvent(Id, EmployerId, applicationId, application.WorkerId, reason));

            if (wasAccepted && Status == JobPostingStatus.Filled)
            {
                Status = JobPostingStatus.Open;
            }
        }

        /// <summary>
        /// Removes a skill requirement by identifier.
        /// </summary>
        protected internal void RemoveSkill(int skillId)
        {
            JobPostingSkill? skill = Skills
                .FirstOrDefault(x => x.Id == skillId);
            skill = skill.ThrowIfNull(DomainErrorCodes.SkillNotFound);
            _skills.Remove(skill);
        }

        /// <summary>
        /// Updates posting copy, shift schedule, wage, and head count in draft state.
        /// </summary>
        protected internal void Update(
            string title,
            string description,
            DateOnly shiftDate,
            TimeOnly shiftStartTime,
            TimeOnly shiftEndTime,
            Money wage,
            int headCount)
        {
            (Status == JobPostingStatus.Draft)
                .ThrowIfFalse(DomainErrorCodes.JobPostingInvalidStatusTransition);

            Title = title;
            Description = description;
            ShiftDate = shiftDate;
            ShiftStartTime = shiftStartTime;
            ShiftEndTime = shiftEndTime;
            Wage = wage;
            HeadCount = headCount;
        }

        /// <summary>
        /// Stores a non-empty description embedding vector when provided.
        /// </summary>
        protected internal void UpdateEmbedding(float[] descriptionEmbedding)
        {
            if (descriptionEmbedding is not null && descriptionEmbedding.Length > 0)
            {
                DescriptionEmbedding = descriptionEmbedding;
            }
        }

        /// <summary>
        /// Withdraws an application and may reopen a filled posting when appropriate.
        /// </summary>
        protected internal void WithdrawApplication(int applicationId)
        {
            (Status == JobPostingStatus.Open || Status == JobPostingStatus.Filled)
                .ThrowIfFalse(DomainErrorCodes.JobPostingInvalidStatusTransition);

            JobApplication application = Applications
                .FirstOrDefault(x => x.Id == applicationId);
            application = application.ThrowIfNull(DomainErrorCodes.JobApplicationNotFound);

            bool wasAccepted = application.Status == JobApplicationStatus.Accepted;
            application.Withdraw();

            if (wasAccepted && Status == JobPostingStatus.Filled)
            {
                Status = JobPostingStatus.Open;
            }
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Posting description text shown to applicants.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// OpenAI text-embedding-3-small vector (1536 dimensions) for semantic matching.
        /// </summary>
        public float[]? DescriptionEmbedding { get; private set; }

        /// <summary>
        /// Employer identifier that owns this posting.
        /// </summary>
        public int EmployerId { get; private set; }

        /// <summary>
        /// Location identifier where the shift takes place.
        /// </summary>
        public int EmployerLocationId { get; private set; }

        /// <summary>
        /// Number of positions requested in this posting.
        /// </summary>
        public int HeadCount { get; private set; }

        /// <summary>
        /// Job category identifier for classification.
        /// </summary>
        public int JobCategoryId { get; private set; }

        /// <summary>
        /// Calendar date for the shift.
        /// </summary>
        public DateOnly ShiftDate { get; private set; }

        /// <summary>
        /// End time for the shift.
        /// </summary>
        public TimeOnly ShiftEndTime { get; private set; }

        /// <summary>
        /// Start time for the shift.
        /// </summary>
        public TimeOnly ShiftStartTime { get; private set; }

        /// <summary>
        /// Current lifecycle status of the posting.
        /// </summary>
        public JobPostingStatus Status { get; private set; }

        /// <summary>
        /// Short posting title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Compensation value for the shift.
        /// </summary>
        public Money Wage { get; private set; }


        /// <summary>
        /// Employer that created the posting.
        /// </summary>
        public virtual Employer Employer { get; private set; }

        /// <summary>
        /// Employer location associated with the posting.
        /// </summary>
        public virtual EmployerLocation EmployerLocation { get; private set; }

        /// <summary>
        /// Category associated with the posting.
        /// </summary>
        public virtual JobCategory JobCategory { get; private set; }


        /// <summary>
        /// Worker applications submitted to this posting.
        /// </summary>
        public virtual IReadOnlyList<JobApplication> Applications => _applications.AsReadOnly();

        /// <summary>
        /// Skill requirements associated with this posting.
        /// </summary>
        public virtual IReadOnlyList<JobPostingSkill> Skills => _skills.AsReadOnly();

        #endregion Properties
    }
}
