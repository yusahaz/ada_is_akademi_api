namespace Azoxia.AdaIsAkademi.Domain.Events
{
    #region Identity

    public sealed record EmailVerificationRequestedEvent(int SystemUserId, string Email) : DomainEvent;

    public sealed record EmailVerifiedEvent(int SystemUserId, string Email) : DomainEvent;

    #endregion Identity

    #region WorkerProfile

    public sealed record WorkerRegisteredEvent(int WorkerId, int SystemUserId) : DomainEvent;

    public sealed record WorkerApprovedEvent(int WorkerId) : DomainEvent;

    public sealed record WorkerAccountSuspendedEvent(int WorkerId) : DomainEvent;

    public sealed record WorkerWorkPermitAttachedEvent(int WorkerId) : DomainEvent;

    public sealed record WorkerProfileUpdatedEvent(int WorkerId) : DomainEvent;

    public sealed record WorkerRatedEvent(int WorkerId, int EmployerId, decimal RatingValue) : DomainEvent;

    public sealed record CvUploadedEvent(int CvUploadSessionId, int WorkerId) : DomainEvent;

    public sealed record CvExtractionCompletedEvent(int CvUploadSessionId, int WorkerId) : DomainEvent;

    public sealed record CvExtractionFailedEvent(int CvUploadSessionId, int WorkerId, string? Reason) : DomainEvent;

    public sealed record CvImportConfirmedEvent(int CvUploadSessionId, int WorkerId) : DomainEvent;

    public sealed record CvImportDiscardedEvent(int CvUploadSessionId, int WorkerId) : DomainEvent;

    #endregion WorkerProfile

    #region JobPosting

    public sealed record JobCategoryCreatedEvent(int JobCategoryId, string Name) : DomainEvent;

    public sealed record JobPostingPublishedEvent(int JobPostingId, int EmployerId) : DomainEvent;

    public sealed record JobPostingFilledEvent(int JobPostingId, int EmployerId) : DomainEvent;

    public sealed record JobPostingCancelledEvent(int JobPostingId, int EmployerId) : DomainEvent;

    public sealed record JobPostingCompletedEvent(int JobPostingId, int EmployerId) : DomainEvent;

    public sealed record JobApplicationSubmittedEvent(
        int JobPostingId,
        int EmployerId,
        int WorkerId,
        DateTimeOffset AppliedAt) : DomainEvent;

    public sealed record JobApplicationAcceptedEvent(
        int JobPostingId,
        int EmployerId,
        int JobApplicationId,
        int WorkerId) : DomainEvent;

    public sealed record JobApplicationRejectedEvent(
        int JobPostingId,
        int EmployerId,
        int JobApplicationId,
        int WorkerId,
        string? Reason) : DomainEvent;

    #endregion JobPosting

    #region Assignment

    public sealed record AssignmentCheckedInEvent(int ShiftAssignmentId, int JobPostingId, int WorkerId) : DomainEvent;

    public sealed record AssignmentCheckedOutEvent(int ShiftAssignmentId, int JobPostingId, int WorkerId) : DomainEvent;

    public sealed record AssignmentCompletedEvent(int ShiftAssignmentId, int JobPostingId, int WorkerId) : DomainEvent;

    public sealed record AssignmentCancelledEvent(int ShiftAssignmentId, int JobPostingId, int WorkerId) : DomainEvent;

    public sealed record AssignmentDisputeRaisedEvent(int ShiftAssignmentId, int JobPostingId, string ReasonCode) : DomainEvent;

    public sealed record AssignmentDisputeResolvedEvent(int ShiftAssignmentId, int JobPostingId, string ResolutionCode) : DomainEvent;

    public sealed record AnomalyDetectedEvent(int ShiftAssignmentId, int JobPostingId, int WorkerId, string AnomalyCode) : DomainEvent;

    #endregion Assignment

    #region Commission / Billing / Payout

    public sealed record CommissionCalculatedEvent(int CommissionReceivableId, int EmployerId) : DomainEvent;

    public sealed record CommissionReceivableCreatedEvent(
        int CommissionReceivableId,
        int EmployerId,
        decimal Amount,
        string Currency) : DomainEvent;

    public sealed record InvoiceSentEvent(int CommissionReceivableId) : DomainEvent;

    public sealed record CommissionPaymentReceivedEvent(int CommissionReceivableId, decimal Amount) : DomainEvent;

    public sealed record CommissionReceivableOverdueEvent(int CommissionReceivableId) : DomainEvent;

    public sealed record WorkerPayoutPendingEvent(int WorkerPayoutId, int AssignmentId, int WorkerId, int EmployerId) : DomainEvent;

    public sealed record WorkerPayoutMarkedAsPaidEvent(int WorkerPayoutId, int AssignmentId, int WorkerId, int EmployerId) : DomainEvent;

    public sealed record WorkerPayoutConfirmedEvent(int WorkerPayoutId, int AssignmentId, int WorkerId, int EmployerId) : DomainEvent;

    public sealed record WorkerPayoutFailedEvent(int WorkerPayoutId, int AssignmentId, int WorkerId, int EmployerId, string? Reason) : DomainEvent;

    #endregion Commission / Billing / Payout
}
