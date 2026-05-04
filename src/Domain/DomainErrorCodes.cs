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
        #endregion Properties
    }
}
