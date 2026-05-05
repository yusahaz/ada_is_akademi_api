namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;
    using Azoxia.Core.ValueTypes;

    /// <summary>
    /// Immutable audit row for commission and payout transitions.
    /// </summary>
    public class CommissionAuditLog :
        EntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected CommissionAuditLog() { }

        /// <summary>
        /// Creates an audit log entry for commission or payout lifecycle events.
        /// </summary>
        /// <param name="employerId">Owning employer key.</param>
        /// <param name="eventType">Recorded event type.</param>
        /// <param name="amount">Monetary snapshot.</param>
        /// <param name="assignmentId">Optional assignment key.</param>
        /// <param name="commissionReceivableId">Optional receivable key.</param>
        /// <param name="workerPayoutId">Optional payout key.</param>
        /// <param name="note">Optional note.</param>
        protected internal CommissionAuditLog(
            int employerId,
            CommissionAuditEventType eventType,
            Money amount,
            int? assignmentId = null,
            int? commissionReceivableId = null,
            int? workerPayoutId = null,
            string? note = null)
        {
            EmployerId = employerId;
            EventType = eventType;
            Amount = amount;
            AssignmentId = assignmentId;
            CommissionReceivableId = commissionReceivableId;
            WorkerPayoutId = workerPayoutId;
            Note = note;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Monetary snapshot for this audit event.
        /// </summary>
        public Money Amount { get; private set; }

        /// <summary>
        /// Source assignment id for payout/commission snapshot.
        /// </summary>
        public int? AssignmentId { get; private set; }

        /// <summary>
        /// Receivable id for receivable lifecycle events.
        /// </summary>
        public int? CommissionReceivableId { get; private set; }

        /// <summary>
        /// UTC timestamp for immutable append-only row creation.
        /// </summary>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// Employer that owns this audit transition.
        /// </summary>
        public int EmployerId { get; private set; }

        /// <summary>
        /// Logged event type for commission or payout transition.
        /// </summary>
        public CommissionAuditEventType EventType { get; private set; }

        /// <summary>
        /// Optional explanatory note for failed/retried transitions.
        /// </summary>
        public string? Note { get; private set; }

        /// <summary>
        /// Worker payout id for payout lifecycle events.
        /// </summary>
        public int? WorkerPayoutId { get; private set; }

        /// <summary>
        /// Linked receivable navigation.
        /// </summary>
        public virtual CommissionReceivable? CommissionReceivable { get; private set; }

        /// <summary>
        /// Employer navigation.
        /// </summary>
        public virtual Employer Employer { get; private set; }

        /// <summary>
        /// Linked assignment navigation.
        /// </summary>
        public virtual ShiftAssignment? ShiftAssignment { get; private set; }

        /// <summary>
        /// Linked worker payout navigation.
        /// </summary>
        public virtual WorkerPayout? WorkerPayout { get; private set; }

        #endregion Properties
    }
}
