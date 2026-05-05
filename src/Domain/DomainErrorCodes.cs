namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Exceptions;

    /// <summary>
    /// Domain-specific error catalog for AdaIsAkademi bounded context.
    /// </summary>
    public static class DomainErrorCodes
    {
        #region Properties
        /// <summary>
        /// Employer status does not allow the requested transition.
        /// </summary>
        public static readonly ErrorCode EmployerInvalidStatusTransition = new(
            Code: "AZX_ADA_DOMAIN_010",
            ErrorMessage: "This operation is not allowed for the current employer status.");

        /// <summary>
        /// Only active employers may create job postings.
        /// </summary>
        public static readonly ErrorCode EmployerCannotCreateJobPosting = new(
            Code: "AZX_ADA_DOMAIN_016",
            ErrorMessage: "Only active employers can create job postings.");

        /// <summary>
        /// The referenced location is not registered for this employer.
        /// </summary>
        public static readonly ErrorCode EmployerLocationNotFound = new(
            Code: "AZX_ADA_DOMAIN_017",
            ErrorMessage: "The specified location does not exist for this employer.");

        /// <summary>
        /// Commission rate must be between 0 and 1 inclusive.
        /// </summary>
        public static readonly ErrorCode EmployerCommissionRateOutOfRange = new(
            Code: "AZX_ADA_DOMAIN_022",
            ErrorMessage: "Commission rate must be between 0 and 1.");

        /// <summary>
        /// Commission receivable period end must be on or after period start.
        /// </summary>
        public static readonly ErrorCode CommissionReceivablePeriodInvalid = new(
            Code: "AZX_ADA_DOMAIN_023",
            ErrorMessage: "Commission receivable period is invalid.");

        /// <summary>
        /// Commission receivable generation requires an active employer.
        /// </summary>
        public static readonly ErrorCode CommissionReceivableEmployerNotActive = new(
            Code: "AZX_ADA_DOMAIN_026",
            ErrorMessage: "Commission receivable can only be generated for active employers.");

        /// <summary>
        /// Geofence radius is outside the permitted range.
        /// </summary>
        public static readonly ErrorCode GeofenceRadiusOutOfRange = new(
            Code: "AZX_ADA_DOMAIN_008",
            ErrorMessage: "Geofence radius must be between 1 and 100000 metres.");

        /// <summary>
        /// The target job application could not be found.
        /// </summary>
        public static readonly ErrorCode JobApplicationNotFound = new(
            Code: "AZX_ADA_DOMAIN_003",
            ErrorMessage: "The target job application could not be found.");

        /// <summary>
        /// The requested application state transition is not allowed.
        /// </summary>
        public static readonly ErrorCode JobApplicationInvalidStatusTransition = new(
            Code: "AZX_ADA_DOMAIN_024",
            ErrorMessage: "This operation is not allowed for the current job application status.");

        /// <summary>
        /// The job posting has reached its accepted application capacity.
        /// </summary>
        public static readonly ErrorCode JobPostingCapacityReached = new(
            Code: "AZX_ADA_DOMAIN_002",
            ErrorMessage: "The job posting has reached its accepted application capacity.");

        /// <summary>
        /// The operation is not valid for the current job posting status.
        /// </summary>
        public static readonly ErrorCode JobPostingInvalidStatusTransition = new(
            Code: "AZX_ADA_DOMAIN_001",
            ErrorMessage: "This operation is not allowed for the current job posting status.");

        /// <summary>
        /// Head count for a job posting must be positive.
        /// </summary>
        public static readonly ErrorCode JobPostingHeadCountInvalid = new(
            Code: "AZX_ADA_DOMAIN_018",
            ErrorMessage: "Head count must be greater than zero.");

        /// <summary>
        /// Shift end time must be after shift start time.
        /// </summary>
        public static readonly ErrorCode JobPostingInvalidShiftTimes = new(
            Code: "AZX_ADA_DOMAIN_019",
            ErrorMessage: "Shift end time must be after shift start time.");

        /// <summary>
        /// Applications cannot be created for shifts in the past.
        /// </summary>
        public static readonly ErrorCode JobPostingShiftDatePassed = new(
            Code: "AZX_ADA_DOMAIN_004",
            ErrorMessage: "Applications cannot be created for shifts in the past.");

        /// <summary>
        /// The requested skill record could not be found.
        /// </summary>
        public static readonly ErrorCode SkillNotFound = new(
            Code: "AZX_ADA_DOMAIN_007",
            ErrorMessage: "The requested skill record could not be found.");

        /// <summary>
        /// Skill tag text is missing or whitespace.
        /// </summary>
        public static readonly ErrorCode SkillTagInvalid = new(
            Code: "AZX_ADA_DOMAIN_006",
            ErrorMessage: "Skill tag cannot be null, empty, or whitespace.");

        /// <summary>
        /// The provided check-in token is invalid for the assignment.
        /// </summary>
        public static readonly ErrorCode ShiftAssignmentCheckInTokenInvalid = new(
            Code: "AZX_ADA_DOMAIN_020",
            ErrorMessage: "The provided check-in token is invalid.");

        /// <summary>
        /// The requested shift assignment state transition is not allowed.
        /// </summary>
        public static readonly ErrorCode ShiftAssignmentInvalidStatusTransition = new(
            Code: "AZX_ADA_DOMAIN_021",
            ErrorMessage: "This operation is not allowed for the current shift assignment status.");

        /// <summary>
        /// Mutual QR confirmation window has expired.
        /// </summary>
        public static readonly ErrorCode ShiftAssignmentMutualQrWindowExpired = new(
            Code: "AZX_ADA_DOMAIN_027",
            ErrorMessage: "Mutual QR confirmation window has expired.");

        /// <summary>
        /// Email is already verified for the account.
        /// </summary>
        public static readonly ErrorCode SystemUserEmailAlreadyVerified = new(
            Code: "AZX_ADA_DOMAIN_012",
            ErrorMessage: "Email is already verified.");

        /// <summary>
        /// Email verification expiration must be in the future.
        /// </summary>
        public static readonly ErrorCode SystemUserEmailVerificationExpiresAtInvalid = new(
            Code: "AZX_ADA_DOMAIN_014",
            ErrorMessage: "Email verification expiration must be in the future.");

        /// <summary>
        /// Email verification token is invalid or expired.
        /// </summary>
        public static readonly ErrorCode SystemUserEmailVerificationInvalid = new(
            Code: "AZX_ADA_DOMAIN_013",
            ErrorMessage: "Email verification token is invalid or expired.");

        /// <summary>
        /// System user status does not allow the requested transition.
        /// </summary>
        public static readonly ErrorCode SystemUserInvalidStatusTransition = new(
            Code: "AZX_ADA_DOMAIN_011",
            ErrorMessage: "This operation is not allowed for the current user account status.");

        /// <summary>
        /// The target refresh token could not be found.
        /// </summary>
        public static readonly ErrorCode SystemUserRefreshTokenNotFound = new(
            Code: "AZX_ADA_DOMAIN_015",
            ErrorMessage: "The target refresh token could not be found.");

        /// <summary>
        /// Tax number text is missing or whitespace.
        /// </summary>
        public static readonly ErrorCode TaxNumberInvalid = new(
            Code: "AZX_ADA_DOMAIN_009",
            ErrorMessage: "Tax number cannot be null, empty, or whitespace.");

        /// <summary>
        /// Worker has a conflicting shift for the target date and time.
        /// </summary>
        public static readonly ErrorCode WorkerHasConflictingShift = new(
            Code: "AZX_ADA_DOMAIN_005",
            ErrorMessage: "Worker has a conflicting shift for the target date and time.");

        /// <summary>
        /// The requested worker profile sub-item could not be found.
        /// </summary>
        public static readonly ErrorCode WorkerProfileItemNotFound = new(
            Code: "AZX_ADA_DOMAIN_025",
            ErrorMessage: "The requested worker profile item could not be found.");

        /// <summary>
        /// Worker payout cannot transition from its current status.
        /// </summary>
        public static readonly ErrorCode WorkerPayoutInvalidStatusTransition = new(
            Code: "AZX_ADA_DOMAIN_028",
            ErrorMessage: "This operation is not allowed for the current worker payout status.");

        /// <summary>
        /// Worker payout processing is blocked because related assignment is disputed.
        /// </summary>
        public static readonly ErrorCode WorkerPayoutAssignmentDisputed = new(
            Code: "AZX_ADA_DOMAIN_029",
            ErrorMessage: "Worker payout cannot proceed while assignment is disputed.");

        /// <summary>
        /// Worker payout retry threshold has been reached.
        /// </summary>
        public static readonly ErrorCode WorkerPayoutRetryLimitExceeded = new(
            Code: "AZX_ADA_DOMAIN_030",
            ErrorMessage: "Worker payout retry limit has been exceeded.");

        /// <summary>
        /// Notification dispatch cannot transition from current status.
        /// </summary>
        public static readonly ErrorCode NotificationDispatchInvalidStatusTransition = new(
            Code: "AZX_ADA_DOMAIN_031",
            ErrorMessage: "This operation is not allowed for the current notification dispatch status.");

        /// <summary>
        /// Notification dispatch retry threshold has been reached.
        /// </summary>
        public static readonly ErrorCode NotificationDispatchRetryLimitExceeded = new(
            Code: "AZX_ADA_DOMAIN_032",
            ErrorMessage: "Notification dispatch retry limit has been exceeded.");
        #endregion Properties
    }
}
