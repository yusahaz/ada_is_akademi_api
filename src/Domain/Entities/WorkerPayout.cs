namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;

    /// <summary>
    /// Tracks employer-to-worker payout lifecycle for a completed assignment.
    /// </summary>
    public class WorkerPayout :
        EntityBase
    {
        #region Fields

        private const int MaxRetryCount = 3;
        private const int WorkerConfirmationWindowHours = 48;

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected WorkerPayout() { }

        /// <summary>
        /// Creates a payout row in pending status with computed net amount.
        /// </summary>
        protected internal WorkerPayout(
            int assignmentId,
            int employerId,
            int workerId,
            Money grossAmount,
            Money commissionAmount)
        {
            AssignmentId = assignmentId;
            EmployerId = employerId;
            WorkerId = workerId;
            GrossAmount = grossAmount;
            CommissionAmount = commissionAmount;
            decimal netAmount = Math.Max(0m, grossAmount.Amount - commissionAmount.Amount);
            NetAmount = new Money(netAmount, grossAmount.Currency);
            Status = WorkerPayoutStatus.Pending;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Marks payout as paid after worker confirmation when status allows.
        /// </summary>
        protected internal void ConfirmPaid()
        {
            (Status == WorkerPayoutStatus.Processing)
                .ThrowIfFalse(DomainErrorCodes.WorkerPayoutInvalidStatusTransition);

            Status = WorkerPayoutStatus.Paid;
            PaidAt = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Records a failed processing attempt with optional reason and retry counter update.
        /// </summary>
        protected internal void Fail(string? reason = null)
        {
            (Status == WorkerPayoutStatus.Processing)
                .ThrowIfFalse(DomainErrorCodes.WorkerPayoutInvalidStatusTransition);

            Status = WorkerPayoutStatus.Failed;
            FailedAt = DateTimeOffset.UtcNow;
            RetryCount += 1;
            LastFailureReason = reason;
        }

        /// <summary>
        /// Moves payout into processing when assignment is not disputed and retry rules pass.
        /// </summary>
        protected internal void MarkAsProcessing(bool assignmentIsDisputed)
        {
            (!assignmentIsDisputed)
                .ThrowIfFalse(DomainErrorCodes.WorkerPayoutAssignmentDisputed);
            (Status == WorkerPayoutStatus.Pending || Status == WorkerPayoutStatus.Failed)
                .ThrowIfFalse(DomainErrorCodes.WorkerPayoutInvalidStatusTransition);
            (RetryCount < MaxRetryCount)
                .ThrowIfFalse(DomainErrorCodes.WorkerPayoutRetryLimitExceeded);

            Status = WorkerPayoutStatus.Processing;
            ProcessingMarkedAt = DateTimeOffset.UtcNow;
            ConfirmationDueAt = ProcessingMarkedAt.Value.AddHours(WorkerConfirmationWindowHours);
        }

        /// <summary>
        /// Resets a failed payout back to pending when retries remain.
        /// </summary>
        protected internal void Retry()
        {
            (Status == WorkerPayoutStatus.Failed)
                .ThrowIfFalse(DomainErrorCodes.WorkerPayoutInvalidStatusTransition);
            (RetryCount < MaxRetryCount)
                .ThrowIfFalse(DomainErrorCodes.WorkerPayoutRetryLimitExceeded);

            Status = WorkerPayoutStatus.Pending;
            FailedAt = null;
            LastFailureReason = null;
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Assignment identifier bound to this payout.
        /// </summary>
        public int AssignmentId { get; private set; }

        /// <summary>
        /// Commission portion subtracted from gross amount.
        /// </summary>
        public Money CommissionAmount { get; private set; }

        /// <summary>
        /// UTC window end for worker confirmation in processing state.
        /// </summary>
        public DateTimeOffset? ConfirmationDueAt { get; private set; }

        /// <summary>
        /// UTC timestamp when payout row was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// Employer identifier that pays out this row.
        /// </summary>
        public int EmployerId { get; private set; }

        /// <summary>
        /// UTC timestamp for last failed transition.
        /// </summary>
        public DateTimeOffset? FailedAt { get; private set; }

        /// <summary>
        /// Gross wage amount before commission deduction.
        /// </summary>
        public Money GrossAmount { get; private set; }

        /// <summary>
        /// Last failure reason emitted during processing lifecycle.
        /// </summary>
        public string? LastFailureReason { get; private set; }

        /// <summary>
        /// Net amount expected to be transferred to worker.
        /// </summary>
        public Money NetAmount { get; private set; }

        /// <summary>
        /// UTC timestamp when worker confirmation is completed.
        /// </summary>
        public DateTimeOffset? PaidAt { get; private set; }

        /// <summary>
        /// UTC timestamp when employer marks payout as paid.
        /// </summary>
        public DateTimeOffset? ProcessingMarkedAt { get; private set; }

        /// <summary>
        /// Retry counter for failed processing transitions.
        /// </summary>
        public int RetryCount { get; private set; }

        /// <summary>
        /// Current payout status.
        /// </summary>
        public WorkerPayoutStatus Status { get; private set; }

        /// <summary>
        /// Worker identifier receiving the payout.
        /// </summary>
        public int WorkerId { get; private set; }

        /// <summary>
        /// Linked employer navigation.
        /// </summary>
        public virtual Employer Employer { get; private set; }

        /// <summary>
        /// Linked assignment navigation.
        /// </summary>
        public virtual ShiftAssignment ShiftAssignment { get; private set; }

        /// <summary>
        /// Linked worker navigation.
        /// </summary>
        public virtual Worker Worker { get; private set; }

        #endregion Properties
    }
}
