namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Exceptions;

    /// <summary>
    /// Application-layer validation errors for commands and queries (stable codes and messages).
    /// </summary>
    internal static class ApplicationValidationCodes
    {
        #region Properties

        /// <summary>
        /// Application id must be positive when accepting a job posting application.
        /// </summary>
        internal static readonly ErrorCode AcceptJobPostingApplicationApplicationId = new(
            Code: "AZX_ADA_APP_VAL_114",
            ErrorMessage: "ApplicationId must be greater than zero.");

        /// <summary>
        /// Authenticated employer operations require a positive <c>employer_id</c> claim on the token.
        /// </summary>
        internal static readonly ErrorCode ActorEmployerIdClaimRequired = new(
            Code: "AZX_ADA_APP_VAL_900",
            ErrorMessage: "A valid employer_id claim is required.");

        /// <summary>
        /// Authenticated worker operations require a positive <c>worker_id</c> claim on the token.
        /// </summary>
        internal static readonly ErrorCode ActorWorkerIdClaimRequired = new(
            Code: "AZX_ADA_APP_VAL_901",
            ErrorMessage: "A valid worker_id claim is required.");

        /// <summary>
        /// Authenticated operations on current user profile require a positive <c>system_user_id</c> claim on the token.
        /// </summary>
        internal static readonly ErrorCode ActorSystemUserIdClaimRequired = new(
            Code: "AZX_ADA_APP_VAL_902",
            ErrorMessage: "A valid system_user_id claim is required.");

        /// <summary>
        /// Authenticated actor is not allowed to access or mutate the targeted resource.
        /// </summary>
        internal static readonly ErrorCode ActorResourceAccessDenied = new(
            Code: "AZX_ADA_APP_VAL_928",
            ErrorMessage: "You are not allowed to perform this operation on the requested resource.");

        /// <summary>
        /// Job posting id must be positive when accepting a job posting application.
        /// </summary>
        internal static readonly ErrorCode AcceptJobPostingApplicationJobPostingId = new(
            Code: "AZX_ADA_APP_VAL_113",
            ErrorMessage: "JobPostingId must be greater than zero.");

        /// <summary>
        /// Employer id must be positive when activating an employer.
        /// </summary>
        internal static readonly ErrorCode ActivateEmployerEmployerId = new(
            Code: "AZX_ADA_APP_VAL_201",
            ErrorMessage: "EmployerId must be greater than zero.");

        /// <summary>
        /// System user group id must be positive when activating a group.
        /// </summary>
        internal static readonly ErrorCode ActivateSystemUserGroupSystemUserGroupId = new(
            Code: "AZX_ADA_APP_VAL_401",
            ErrorMessage: "SystemUserGroupId must be greater than zero.");

        /// <summary>
        /// Job posting id must be positive when adding a required skill.
        /// </summary>
        internal static readonly ErrorCode AddJobPostingSkillJobPostingId = new(
            Code: "AZX_ADA_APP_VAL_104",
            ErrorMessage: "JobPostingId must be greater than zero.");

        /// <summary>
        /// Skill tag text is required when adding a posting skill.
        /// </summary>
        internal static readonly ErrorCode AddJobPostingSkillTag = new(
            Code: "AZX_ADA_APP_VAL_105",
            ErrorMessage: "Tag cannot be null, empty, or whitespace.");

        /// <summary>
        /// Permission id must be positive when adding a group permission row.
        /// </summary>
        internal static readonly ErrorCode AddSystemUserGroupPermissionPermissionId = new(
            Code: "AZX_ADA_APP_VAL_404",
            ErrorMessage: "PermissionId must be greater than zero.");

        /// <summary>
        /// System user group id must be positive when adding a group permission row.
        /// </summary>
        internal static readonly ErrorCode AddSystemUserGroupPermissionSystemUserGroupId = new(
            Code: "AZX_ADA_APP_VAL_403",
            ErrorMessage: "SystemUserGroupId must be greater than zero.");

        /// <summary>
        /// Tag text is required when adding a worker skill.
        /// </summary>
        internal static readonly ErrorCode AddWorkerSkillTagRequired = new(
            Code: "AZX_ADA_APP_VAL_002",
            ErrorMessage: "Tag cannot be null, empty, or whitespace.");

        /// <summary>
        /// Worker id must be positive when adding a worker skill.
        /// </summary>
        internal static readonly ErrorCode AddWorkerSkillWorkerId = new(
            Code: "AZX_ADA_APP_VAL_001",
            ErrorMessage: "WorkerId must be greater than zero.");

        /// <summary>
        /// Nationality length cannot exceed 128 characters for worker profile updates.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerProfileNationalityMaxLength = new(
            Code: "AZX_ADA_APP_VAL_942",
            ErrorMessage: "Nationality length cannot exceed 128 characters.");

        /// <summary>
        /// First name length cannot exceed 128 characters for worker profile updates.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerProfileFirstNameMaxLength = new(
            Code: "AZX_ADA_APP_VAL_1961",
            ErrorMessage: "FirstName length cannot exceed 128 characters.");

        /// <summary>
        /// Last name length cannot exceed 128 characters for worker profile updates.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerProfileLastNameMaxLength = new(
            Code: "AZX_ADA_APP_VAL_1962",
            ErrorMessage: "LastName length cannot exceed 128 characters.");

        /// <summary>
        /// University length cannot exceed 512 characters for worker profile updates.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerProfileUniversityMaxLength = new(
            Code: "AZX_ADA_APP_VAL_943",
            ErrorMessage: "University length cannot exceed 512 characters.");

        /// <summary>
        /// Worker bio exceeds the maximum length.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerBioMaxLength = new(
            Code: "AZX_ADA_APP_VAL_944",
            ErrorMessage: "Bio length cannot exceed 3000 characters.");

        /// <summary>
        /// Employer social links list exceeds supported count.
        /// </summary>
        internal static readonly ErrorCode UpdateEmployerSocialLinksCount = new(
            Code: "AZX_ADA_APP_VAL_953",
            ErrorMessage: "Too many social links.");

        /// <summary>
        /// Duplicate platforms are not supported when replacing employer social links.
        /// </summary>
        internal static readonly ErrorCode UpdateEmployerSocialLinksDuplicatePlatform = new(
            Code: "AZX_ADA_APP_VAL_954",
            ErrorMessage: "Each social platform must appear only once.");

        /// <summary>
        /// Social link URL is required for each employer link row.
        /// </summary>
        internal static readonly ErrorCode UpdateEmployerSocialLinksUrlRequired = new(
            Code: "AZX_ADA_APP_VAL_955",
            ErrorMessage: "Social link URL is required.");

        /// <summary>
        /// Social link URL must be an absolute HTTPS address within length limits for employer links.
        /// </summary>
        internal static readonly ErrorCode UpdateEmployerSocialLinksUrlInvalid = new(
            Code: "AZX_ADA_APP_VAL_956",
            ErrorMessage: "Social link URL must be a valid HTTPS address.");

        /// <summary>
        /// Worker social links list exceeds supported count.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerSocialLinksCount = new(
            Code: "AZX_ADA_APP_VAL_945",
            ErrorMessage: "Too many social links.");

        /// <summary>
        /// Duplicate platforms are not supported in one replace operation.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerSocialLinksDuplicatePlatform = new(
            Code: "AZX_ADA_APP_VAL_946",
            ErrorMessage: "Each social platform must appear only once.");

        /// <summary>
        /// Social link URL is required for each declared row.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerSocialLinksUrlRequired = new(
            Code: "AZX_ADA_APP_VAL_947",
            ErrorMessage: "Social link URL is required.");

        /// <summary>
        /// Social link URL must be an absolute HTTPS address within length limits.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerSocialLinksUrlInvalid = new(
            Code: "AZX_ADA_APP_VAL_948",
            ErrorMessage: "Social link URL must be a valid HTTPS address.");

        /// <summary>
        /// Object storage object key belongs to another actor aggregate or is malformed.
        /// </summary>
        internal static readonly ErrorCode MediaBlobObjectKeyOwnership = new(
            Code: "AZX_ADA_APP_VAL_949",
            ErrorMessage: "The uploaded object key is not scoped to your profile.");

        /// <summary>
        /// Object storage object key payload is missing.
        /// </summary>
        internal static readonly ErrorCode MediaBlobObjectKeyRequired = new(
            Code: "AZX_ADA_APP_VAL_950",
            ErrorMessage: "ObjectKey is required.");

        /// <summary>
        /// PUT content type hint is unexpectedly long when requesting a signed upload URL.
        /// </summary>
        internal static readonly ErrorCode InitMediaUploadContentTypeMaxLength = new(
            Code: "AZX_ADA_APP_VAL_951",
            ErrorMessage: "ContentType cannot exceed 128 characters.");

        /// <summary>
        /// CV content type is required at upload confirm stage.
        /// </summary>
        internal static readonly ErrorCode WorkerCvContentTypeRequired = new(
            Code: "AZX_ADA_APP_VAL_963",
            ErrorMessage: "ContentType is required for CV upload.");

        /// <summary>
        /// CV content type exceeds accepted length.
        /// </summary>
        internal static readonly ErrorCode WorkerCvContentTypeMaxLength = new(
            Code: "AZX_ADA_APP_VAL_964",
            ErrorMessage: "ContentType cannot exceed 128 characters for CV upload.");

        /// <summary>
        /// CV file format is not supported by extraction pipeline.
        /// </summary>
        internal static readonly ErrorCode WorkerCvFileFormatNotSupported = new(
            Code: "AZX_ADA_APP_VAL_965",
            ErrorMessage: "Only PDF and DOCX CV formats are supported.");

        /// <summary>
        /// CV file name exceeds accepted length.
        /// </summary>
        internal static readonly ErrorCode WorkerCvFileNameMaxLength = new(
            Code: "AZX_ADA_APP_VAL_966",
            ErrorMessage: "FileName cannot exceed 256 characters for CV upload.");

        /// <summary>
        /// CV file name is required to infer file format and keep auditability.
        /// </summary>
        internal static readonly ErrorCode WorkerCvFileNameRequired = new(
            Code: "AZX_ADA_APP_VAL_967",
            ErrorMessage: "FileName is required for CV upload.");

        /// <summary>
        /// CV file size is outside accepted range.
        /// </summary>
        internal static readonly ErrorCode WorkerCvFileSizeOutOfRange = new(
            Code: "AZX_ADA_APP_VAL_968",
            ErrorMessage: "FileSizeBytes must be between 1 and 10485760.");

        /// <summary>
        /// CV upload session identifier must be positive for review actions.
        /// </summary>
        internal static readonly ErrorCode WorkerCvUploadSessionIdRequired = new(
            Code: "AZX_ADA_APP_VAL_969",
            ErrorMessage: "CvUploadSessionId must be greater than zero.");

        /// <summary>
        /// Employer id must be positive when filtering financial reconciliation rows.
        /// </summary>
        internal static readonly ErrorCode ListFinancialReconciliationRowsEmployerId = new(
            Code: "AZX_ADA_APP_VAL_970",
            ErrorMessage: "EmployerId must be greater than zero when provided.");

        /// <summary>
        /// Limit must be within allowed bounds for financial reconciliation list query.
        /// </summary>
        internal static readonly ErrorCode ListFinancialReconciliationRowsLimit = new(
            Code: "AZX_ADA_APP_VAL_971",
            ErrorMessage: "Limit must be between 1 and 200.");

        /// <summary>
        /// Offset must be non-negative for financial reconciliation list query.
        /// </summary>
        internal static readonly ErrorCode ListFinancialReconciliationRowsOffset = new(
            Code: "AZX_ADA_APP_VAL_972",
            ErrorMessage: "Offset cannot be negative.");

        /// <summary>
        /// Date range filter is invalid when from is greater than to.
        /// </summary>
        internal static readonly ErrorCode ListFinancialReconciliationRowsDateRange = new(
            Code: "AZX_ADA_APP_VAL_973",
            ErrorMessage: "From cannot be later than To.");

        /// <summary>
        /// Expected salary currency must be a three-letter ISO code when provided.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerMatchingExpectedSalaryCurrencyLength = new(
            Code: "AZX_ADA_APP_VAL_960",
            ErrorMessage: "ExpectedSalaryCurrencyCode must be a three-letter ISO 4217 code when provided.");

        /// <summary>
        /// Expected salary currency must be supplied when declaring an amount.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerMatchingExpectedSalaryCurrencyRequired = new(
            Code: "AZX_ADA_APP_VAL_961",
            ErrorMessage: "ExpectedSalaryCurrencyCode is required when declaring min or max salary amounts.");

        /// <summary>
        /// Expected salary bounds must remain within the supported non-negative bounded range.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerMatchingExpectedSalaryAmountRange = new(
            Code: "AZX_ADA_APP_VAL_962",
            ErrorMessage: "Expected salary amounts must be between 0 and 999,999,999.99.");

        /// <summary>
        /// Expected salary minimum must not exceed declared maximum when both are present.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerMatchingExpectedSalaryMinMax = new(
            Code: "AZX_ADA_APP_VAL_963",
            ErrorMessage: "ExpectedSalaryMinAmount cannot be greater than ExpectedSalaryMaxAmount.");

        /// <summary>
        /// Salary currency must not appear without its paired amount.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerMatchingExpectedSalaryCurrencyWithoutAmount = new(
            Code: "AZX_ADA_APP_VAL_964",
            ErrorMessage: "Supply amounts for salary bounds when declaring their currencies.");

        /// <summary>
        /// Salary bounds declared together must share the same currency.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerMatchingExpectedSalaryCurrencyMismatch = new(
            Code: "AZX_ADA_APP_VAL_971",
            ErrorMessage: "Expected salary minimum and maximum must use the same currency.");

        /// <summary>
        /// Interested job categories list cannot include duplicates for the worker.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerMatchingInterestedCategoryDuplicates = new(
            Code: "AZX_ADA_APP_VAL_965",
            ErrorMessage: "InterestedJobCategoryIds must not contain duplicates.");

        /// <summary>
        /// Interested category id list violates count limits.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerMatchingInterestedCategoryCount = new(
            Code: "AZX_ADA_APP_VAL_966",
            ErrorMessage: "Too many interested job categories.");

        /// <summary>
        /// Interested category reference must be positive.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerMatchingInterestedCategoryId = new(
            Code: "AZX_ADA_APP_VAL_967",
            ErrorMessage: "InterestedJobCategoryIds must contain positive category ids.");

        /// <summary>
        /// Interested job categories must be explicitly provided when replacing the list.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerMatchingInterestedCategoryIdsRequired = new(
            Code: "AZX_ADA_APP_VAL_968",
            ErrorMessage: "InterestedJobCategoryIds is required when SetInterestedJobCategories is true.");

        /// <summary>
        /// Matching preferences mutation must toggle at least one section.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerMatchingNoOp = new(
            Code: "AZX_ADA_APP_VAL_969",
            ErrorMessage: "Enable SetExpectedSalary and/or SetInterestedJobCategories.");

        /// <summary>
        /// One or more job category identifiers are unknown or inactive for matching preferences updates.
        /// </summary>
        internal static readonly ErrorCode UpdateWorkerMatchingUnknownJobCategory = new(
            Code: "AZX_ADA_APP_VAL_970",
            ErrorMessage: "One or more job category ids are invalid.");

        /// <summary>
        /// Employer id must be positive when banning an employer.
        /// </summary>
        internal static readonly ErrorCode BanEmployerEmployerId = new(
            Code: "AZX_ADA_APP_VAL_203",
            ErrorMessage: "EmployerId must be greater than zero.");

        /// <summary>
        /// System user id must be positive when banning a system user.
        /// </summary>
        internal static readonly ErrorCode BanSystemUserSystemUserId = new(
            Code: "AZX_ADA_APP_VAL_301",
            ErrorMessage: "SystemUserId must be greater than zero.");

        /// <summary>
        /// Job posting id must be positive when cancelling a posting.
        /// </summary>
        internal static readonly ErrorCode CancelJobPostingJobPostingId = new(
            Code: "AZX_ADA_APP_VAL_102",
            ErrorMessage: "JobPostingId must be greater than zero.");

        /// <summary>
        /// Password is required when changing a system user password.
        /// </summary>
        internal static readonly ErrorCode ChangeSystemUserPasswordPasswordRequired = new(
            Code: "AZX_ADA_APP_VAL_309",
            ErrorMessage: "Password cannot be null, empty, or whitespace.");

        /// <summary>
        /// System user id must be positive when changing password.
        /// </summary>
        internal static readonly ErrorCode ChangeSystemUserPasswordSystemUserId = new(
            Code: "AZX_ADA_APP_VAL_308",
            ErrorMessage: "SystemUserId must be greater than zero.");

        /// <summary>
        /// Job posting id must be positive when completing a posting.
        /// </summary>
        internal static readonly ErrorCode CompleteJobPostingJobPostingId = new(
            Code: "AZX_ADA_APP_VAL_103",
            ErrorMessage: "JobPostingId must be greater than zero.");

        /// <summary>
        /// Description is required when creating a job posting.
        /// </summary>
        internal static readonly ErrorCode CreateJobPostingDescriptionRequired = new(
            Code: "AZX_ADA_APP_VAL_124",
            ErrorMessage: "Description cannot be null, empty, or whitespace.");

        /// <summary>
        /// Employer location id must be positive when creating a job posting.
        /// </summary>
        internal static readonly ErrorCode CreateJobPostingEmployerLocationId = new(
            Code: "AZX_ADA_APP_VAL_121",
            ErrorMessage: "EmployerLocationId must be greater than zero.");

        /// <summary>
        /// Head count must be positive when creating a job posting.
        /// </summary>
        internal static readonly ErrorCode CreateJobPostingHeadCountPositive = new(
            Code: "AZX_ADA_APP_VAL_125",
            ErrorMessage: "HeadCount must be greater than zero.");

        /// <summary>
        /// Job category id must be positive when creating a job posting.
        /// </summary>
        internal static readonly ErrorCode CreateJobPostingJobCategoryId = new(
            Code: "AZX_ADA_APP_VAL_122",
            ErrorMessage: "JobCategoryId must be greater than zero.");

        /// <summary>
        /// Shift end must be after shift start when creating a job posting.
        /// </summary>
        internal static readonly ErrorCode CreateJobPostingShiftEndAfterStart = new(
            Code: "AZX_ADA_APP_VAL_128",
            ErrorMessage: "ShiftEndTime must be after ShiftStartTime.");

        /// <summary>
        /// Title is required when creating a job posting.
        /// </summary>
        internal static readonly ErrorCode CreateJobPostingTitleRequired = new(
            Code: "AZX_ADA_APP_VAL_123",
            ErrorMessage: "Title cannot be null, empty, or whitespace.");

        /// <summary>
        /// Wage amount must be positive when creating a job posting.
        /// </summary>
        internal static readonly ErrorCode CreateJobPostingWageAmountPositive = new(
            Code: "AZX_ADA_APP_VAL_127",
            ErrorMessage: "WageAmount must be greater than zero.");

        /// <summary>
        /// Wage currency is required when creating a job posting.
        /// </summary>
        internal static readonly ErrorCode CreateJobPostingWageCurrencyRequired = new(
            Code: "AZX_ADA_APP_VAL_126",
            ErrorMessage: "WageCurrency cannot be null, empty, or whitespace.");

        /// <summary>
        /// Check-in token hash is required when creating a shift assignment.
        /// </summary>
        internal static readonly ErrorCode CreateShiftAssignmentCheckInTokenHashRequired = new(
            Code: "AZX_ADA_APP_VAL_331",
            ErrorMessage: "CheckInTokenHash cannot be null, empty, or whitespace.");

        /// <summary>
        /// Supervisor check-in token hash is required when creating a shift assignment.
        /// </summary>
        internal static readonly ErrorCode CreateShiftAssignmentSupervisorCheckInTokenHashRequired = new(
            Code: "AZX_ADA_APP_VAL_944",
            ErrorMessage: "SupervisorCheckInTokenHash cannot be null, empty, or whitespace.");

        /// <summary>
        /// Job application id must be positive when creating a shift assignment.
        /// </summary>
        internal static readonly ErrorCode CreateShiftAssignmentJobApplicationId = new(
            Code: "AZX_ADA_APP_VAL_332",
            ErrorMessage: "JobApplicationId must be greater than zero.");

        /// <summary>
        /// Shift assignment can only be created from an accepted job application.
        /// </summary>
        internal static readonly ErrorCode CreateShiftAssignmentApplicationNotAccepted = new(
            Code: "AZX_ADA_APP_VAL_929",
            ErrorMessage: "Shift assignment can only be created for accepted applications.");

        /// <summary>
        /// System user group id must be positive when deactivating a group.
        /// </summary>
        internal static readonly ErrorCode DeactivateSystemUserGroupSystemUserGroupId = new(
            Code: "AZX_ADA_APP_VAL_402",
            ErrorMessage: "SystemUserGroupId must be greater than zero.");

        /// <summary>
        /// Employer id must be positive on get employer by id query.
        /// </summary>
        internal static readonly ErrorCode GetEmployerByIdEmployerId = new(
            Code: "AZX_ADA_APP_VAL_204",
            ErrorMessage: "EmployerId must be greater than zero.");

        /// <summary>
        /// Job posting id must be positive on get job posting by id query.
        /// </summary>
        internal static readonly ErrorCode GetJobPostingByIdJobPostingId = new(
            Code: "AZX_ADA_APP_VAL_501",
            ErrorMessage: "JobPostingId must be greater than zero.");

        /// <summary>
        /// Worker id must be positive on get worker by id query.
        /// </summary>
        internal static readonly ErrorCode GetWorkerByIdWorkerId = new(
            Code: "AZX_ADA_APP_VAL_601",
            ErrorMessage: "WorkerId must be greater than zero.");

        /// <summary>
        /// Job posting id must be positive on worker notification preview query.
        /// </summary>
        internal static readonly ErrorCode GetWorkerPersonalizedNotificationPreviewJobPostingId = new(
            Code: "AZX_ADA_APP_VAL_602",
            ErrorMessage: "JobPostingId must be greater than zero.");

        /// <summary>
        /// Limit must be between 1 and 50 for worker live status feed query.
        /// </summary>
        internal static readonly ErrorCode GetWorkerLiveStatusFeedLimit = new(
            Code: "AZX_ADA_APP_VAL_953",
            ErrorMessage: "Limit must be between 1 and 50.");

        /// <summary>
        /// Worker id must be positive when sending worker notification.
        /// </summary>
        internal static readonly ErrorCode SendWorkerNotificationWorkerId = new(
            Code: "AZX_ADA_APP_VAL_954",
            ErrorMessage: "WorkerId must be greater than zero.");

        /// <summary>
        /// System user id must be positive when sending generic user notification.
        /// </summary>
        internal static readonly ErrorCode SendSystemUserNotificationSystemUserId = new(
            Code: "AZX_ADA_APP_VAL_959",
            ErrorMessage: "SystemUserId must be greater than zero.");

        /// <summary>
        /// Notification template code is required.
        /// </summary>
        internal static readonly ErrorCode SendWorkerNotificationTemplateCode = new(
            Code: "AZX_ADA_APP_VAL_955",
            ErrorMessage: "TemplateCode cannot be null, empty, or whitespace.");

        /// <summary>
        /// Notification title is required.
        /// </summary>
        internal static readonly ErrorCode SendWorkerNotificationTitle = new(
            Code: "AZX_ADA_APP_VAL_956",
            ErrorMessage: "Title cannot be null, empty, or whitespace.");

        /// <summary>
        /// Notification body is required.
        /// </summary>
        internal static readonly ErrorCode SendWorkerNotificationBody = new(
            Code: "AZX_ADA_APP_VAL_957",
            ErrorMessage: "Body cannot be null, empty, or whitespace.");

        /// <summary>
        /// Retry batch size must be between 1 and 500 for failed notification retries.
        /// </summary>
        internal static readonly ErrorCode RetryFailedSystemUserNotificationsBatchSize = new(
            Code: "AZX_ADA_APP_VAL_958",
            ErrorMessage: "BatchSize must be between 1 and 500.");

        /// <summary>
        /// Job posting id must be positive when listing applications by posting.
        /// </summary>
        internal static readonly ErrorCode ListJobApplicationsByJobPostingIdJobPostingId = new(
            Code: "AZX_ADA_APP_VAL_701",
            ErrorMessage: "JobPostingId must be greater than zero.");

        /// <summary>
        /// Limit must be between 1 and 200 when listing job applications by posting.
        /// </summary>
        internal static readonly ErrorCode ListJobApplicationsByJobPostingIdLimit = new(
            Code: "AZX_ADA_APP_VAL_930",
            ErrorMessage: "Limit must be between 1 and 200.");

        /// <summary>
        /// Offset must be zero or positive when listing job applications by posting.
        /// </summary>
        internal static readonly ErrorCode ListJobApplicationsByJobPostingIdOffset = new(
            Code: "AZX_ADA_APP_VAL_931",
            ErrorMessage: "Offset must be greater than or equal to zero.");

        /// <summary>
        /// Limit must be between 1 and 200 for employer-owned job posting list query.
        /// </summary>
        internal static readonly ErrorCode ListJobPostingsByEmployerIdLimit = new(
            Code: "AZX_ADA_APP_VAL_932",
            ErrorMessage: "Limit must be between 1 and 200.");

        /// <summary>
        /// Offset must be zero or positive for employer-owned job posting list query.
        /// </summary>
        internal static readonly ErrorCode ListJobPostingsByEmployerIdOffset = new(
            Code: "AZX_ADA_APP_VAL_933",
            ErrorMessage: "Offset must be greater than or equal to zero.");

        /// <summary>
        /// Limit must be between 1 and 200 for open job posting list query.
        /// </summary>
        internal static readonly ErrorCode ListOpenJobPostingsLimit = new(
            Code: "AZX_ADA_APP_VAL_934",
            ErrorMessage: "Limit must be between 1 and 200.");

        /// <summary>
        /// Offset must be zero or positive for open job posting list query.
        /// </summary>
        internal static readonly ErrorCode ListOpenJobPostingsOffset = new(
            Code: "AZX_ADA_APP_VAL_935",
            ErrorMessage: "Offset must be greater than or equal to zero.");

        /// <summary>
        /// Limit must be between 1 and 50 for semantic posting list query.
        /// </summary>
        internal static readonly ErrorCode ListSemanticMatchedJobPostingsLimitRange = new(
            Code: "AZX_ADA_APP_VAL_335",
            ErrorMessage: "Limit must be between 1 and 50.");

        /// <summary>
        /// Worker id must be positive for semantic posting list query.
        /// </summary>
        internal static readonly ErrorCode ListSemanticMatchedJobPostingsWorkerId = new(
            Code: "AZX_ADA_APP_VAL_336",
            ErrorMessage: "WorkerId must be greater than zero.");

        /// <summary>
        /// Limit must be between 1 and 100 for employer commission summary list query.
        /// </summary>
        internal static readonly ErrorCode ListEmployerCommissionSummariesLimitRange = new(
            Code: "AZX_ADA_APP_VAL_907",
            ErrorMessage: "Limit must be between 1 and 100.");

        /// <summary>
        /// Employer id must be positive for list commission receivables query.
        /// </summary>
        internal static readonly ErrorCode ListCommissionReceivablesByEmployerEmployerId = new(
            Code: "AZX_ADA_APP_VAL_914",
            ErrorMessage: "EmployerId must be greater than zero.");

        /// <summary>
        /// Limit must be between 1 and 100 for list commission receivables query.
        /// </summary>
        internal static readonly ErrorCode ListCommissionReceivablesByEmployerLimit = new(
            Code: "AZX_ADA_APP_VAL_915",
            ErrorMessage: "Limit must be between 1 and 100.");

        /// <summary>
        /// Offset must be zero or positive for list commission receivables query.
        /// </summary>
        internal static readonly ErrorCode ListCommissionReceivablesByEmployerOffset = new(
            Code: "AZX_ADA_APP_VAL_936",
            ErrorMessage: "Offset must be greater than or equal to zero.");

        /// <summary>
        /// Limit must be between 1 and 200 for worker-self job application list query.
        /// </summary>
        internal static readonly ErrorCode ListMyJobApplicationsLimit = new(
            Code: "AZX_ADA_APP_VAL_938",
            ErrorMessage: "Limit must be between 1 and 200.");

        /// <summary>
        /// Offset must be zero or positive for worker-self job application list query.
        /// </summary>
        internal static readonly ErrorCode ListMyJobApplicationsOffset = new(
            Code: "AZX_ADA_APP_VAL_939",
            ErrorMessage: "Offset must be greater than or equal to zero.");

        /// <summary>
        /// Limit must be between 1 and 200 for worker-self shift assignment list query.
        /// </summary>
        internal static readonly ErrorCode ListMyShiftAssignmentsLimit = new(
            Code: "AZX_ADA_APP_VAL_940",
            ErrorMessage: "Limit must be between 1 and 200.");

        /// <summary>
        /// Offset must be zero or positive for worker-self shift assignment list query.
        /// </summary>
        internal static readonly ErrorCode ListMyShiftAssignmentsOffset = new(
            Code: "AZX_ADA_APP_VAL_941",
            ErrorMessage: "Offset must be greater than or equal to zero.");

        /// <summary>
        /// Limit must be between 1 and 200 for notification inbox list query.
        /// </summary>
        internal static readonly ErrorCode ListMyNotificationsLimit = new(
            Code: "AZX_ADA_APP_VAL_974",
            ErrorMessage: "Limit must be between 1 and 200.");

        /// <summary>
        /// Offset must be zero or positive for notification inbox list query.
        /// </summary>
        internal static readonly ErrorCode ListMyNotificationsOffset = new(
            Code: "AZX_ADA_APP_VAL_975",
            ErrorMessage: "Offset must be greater than or equal to zero.");

        /// <summary>
        /// Notification dispatch id must be greater than zero when marking as read.
        /// </summary>
        internal static readonly ErrorCode MarkNotificationAsReadNotificationId = new(
            Code: "AZX_ADA_APP_VAL_976",
            ErrorMessage: "NotificationId must be greater than zero.");

        /// <summary>
        /// Limit must be between 1 and 200 for list employers query.
        /// </summary>
        internal static readonly ErrorCode ListEmployersLimit = new(
            Code: "AZX_ADA_APP_VAL_916",
            ErrorMessage: "Limit must be between 1 and 200.");

        /// <summary>
        /// Offset must be zero or positive for list employers query.
        /// </summary>
        internal static readonly ErrorCode ListEmployersOffset = new(
            Code: "AZX_ADA_APP_VAL_917",
            ErrorMessage: "Offset must be greater than or equal to zero.");

        /// <summary>
        /// Commission range filters must remain between 0 and 1 and max must be greater than or equal to min.
        /// </summary>
        internal static readonly ErrorCode ListEmployersCommissionRange = new(
            Code: "AZX_ADA_APP_VAL_918",
            ErrorMessage: "Commission range is invalid.");

        /// <summary>
        /// Limit must be between 1 and 200 for list workers query.
        /// </summary>
        internal static readonly ErrorCode ListWorkersLimit = new(
            Code: "AZX_ADA_APP_VAL_919",
            ErrorMessage: "Limit must be between 1 and 200.");

        /// <summary>
        /// Offset must be zero or positive for list workers query.
        /// </summary>
        internal static readonly ErrorCode ListWorkersOffset = new(
            Code: "AZX_ADA_APP_VAL_920",
            ErrorMessage: "Offset must be greater than or equal to zero.");

        /// <summary>
        /// Limit must be between 1 and 200 for list system users query.
        /// </summary>
        internal static readonly ErrorCode ListSystemUsersLimit = new(
            Code: "AZX_ADA_APP_VAL_921",
            ErrorMessage: "Limit must be between 1 and 200.");

        /// <summary>
        /// Offset must be zero or positive for list system users query.
        /// </summary>
        internal static readonly ErrorCode ListSystemUsersOffset = new(
            Code: "AZX_ADA_APP_VAL_922",
            ErrorMessage: "Offset must be greater than or equal to zero.");

        /// <summary>
        /// Limit must be between 1 and 200 for list system user groups query.
        /// </summary>
        internal static readonly ErrorCode ListSystemUserGroupsLimit = new(
            Code: "AZX_ADA_APP_VAL_923",
            ErrorMessage: "Limit must be between 1 and 200.");

        /// <summary>
        /// Offset must be zero or positive for list system user groups query.
        /// </summary>
        internal static readonly ErrorCode ListSystemUserGroupsOffset = new(
            Code: "AZX_ADA_APP_VAL_924",
            ErrorMessage: "Offset must be greater than or equal to zero.");

        /// <summary>
        /// Amount must be non-negative when generating commission receivable.
        /// </summary>
        internal static readonly ErrorCode GenerateCommissionReceivableAmount = new(
            Code: "AZX_ADA_APP_VAL_908",
            ErrorMessage: "Amount must be greater than or equal to zero.");

        /// <summary>
        /// Currency is required when generating commission receivable.
        /// </summary>
        internal static readonly ErrorCode GenerateCommissionReceivableCurrency = new(
            Code: "AZX_ADA_APP_VAL_909",
            ErrorMessage: "Currency cannot be null, empty, or whitespace.");

        /// <summary>
        /// Employer id must be positive when generating commission receivable.
        /// </summary>
        internal static readonly ErrorCode GenerateCommissionReceivableEmployerId = new(
            Code: "AZX_ADA_APP_VAL_910",
            ErrorMessage: "EmployerId must be greater than zero.");

        /// <summary>
        /// Period end must be on or after period start for commission receivable generation.
        /// </summary>
        internal static readonly ErrorCode GenerateCommissionReceivablePeriod = new(
            Code: "AZX_ADA_APP_VAL_911",
            ErrorMessage: "PeriodEnd must be on or after PeriodStart.");

        /// <summary>
        /// Assignment id must be positive when creating worker payout.
        /// </summary>
        internal static readonly ErrorCode CreateWorkerPayoutAssignmentId = new(
            Code: "AZX_ADA_APP_VAL_947",
            ErrorMessage: "AssignmentId must be greater than zero.");

        /// <summary>
        /// Worker payout id must be positive when marking payout as processing.
        /// </summary>
        internal static readonly ErrorCode MarkWorkerPayoutAsProcessingWorkerPayoutId = new(
            Code: "AZX_ADA_APP_VAL_948",
            ErrorMessage: "WorkerPayoutId must be greater than zero.");

        /// <summary>
        /// Worker payout id must be positive when confirming payout.
        /// </summary>
        internal static readonly ErrorCode ConfirmWorkerPayoutWorkerPayoutId = new(
            Code: "AZX_ADA_APP_VAL_949",
            ErrorMessage: "WorkerPayoutId must be greater than zero.");

        /// <summary>
        /// Worker payout id must be positive when failing payout.
        /// </summary>
        internal static readonly ErrorCode FailWorkerPayoutWorkerPayoutId = new(
            Code: "AZX_ADA_APP_VAL_950",
            ErrorMessage: "WorkerPayoutId must be greater than zero.");

        /// <summary>
        /// Worker payout id must be positive when retrying payout.
        /// </summary>
        internal static readonly ErrorCode RetryWorkerPayoutWorkerPayoutId = new(
            Code: "AZX_ADA_APP_VAL_951",
            ErrorMessage: "WorkerPayoutId must be greater than zero.");

        /// <summary>
        /// Employer id must be positive for commission receivable by period query.
        /// </summary>
        internal static readonly ErrorCode GetCommissionReceivableByPeriodEmployerId = new(
            Code: "AZX_ADA_APP_VAL_912",
            ErrorMessage: "EmployerId must be greater than zero.");

        /// <summary>
        /// Period end must be on or after period start for commission receivable by period query.
        /// </summary>
        internal static readonly ErrorCode GetCommissionReceivableByPeriodPeriod = new(
            Code: "AZX_ADA_APP_VAL_913",
            ErrorMessage: "PeriodEnd must be on or after PeriodStart.");

        /// <summary>
        /// Device identifier is required when logging in a system user.
        /// </summary>
        internal static readonly ErrorCode LoginSystemUserDeviceIdentifierRequired = new(
            Code: "AZX_ADA_APP_VAL_311",
            ErrorMessage: "DeviceIdentifier cannot be null, empty, or whitespace.");

        /// <summary>
        /// Email is required when logging in a system user.
        /// </summary>
        internal static readonly ErrorCode LoginSystemUserEmailRequired = new(
            Code: "AZX_ADA_APP_VAL_310",
            ErrorMessage: "Email cannot be null, empty, or whitespace.");

        /// <summary>
        /// Password is required when logging in a system user.
        /// </summary>
        internal static readonly ErrorCode LoginSystemUserPasswordRequired = new(
            Code: "AZX_ADA_APP_VAL_312",
            ErrorMessage: "Password cannot be null, empty, or whitespace.");

        /// <summary>
        /// System user type is required and must be one of Admin, Employer, or Worker for login.
        /// </summary>
        internal static readonly ErrorCode LoginSystemUserTypeRequired = new(
            Code: "AZX_ADA_APP_VAL_977",
            ErrorMessage: "SystemUserType must be Admin, Employer, or Worker.");

        /// <summary>
        /// Credentials are invalid or the account is not eligible for login.
        /// </summary>
        internal static readonly ErrorCode LoginSystemUserAuthenticationFailed = new(
            Code: "AZX_ADA_APP_VAL_925",
            ErrorMessage: "Email/password combination is invalid or the account cannot sign in.");

        /// <summary>
        /// Device identifier is required when logging out a system user.
        /// </summary>
        internal static readonly ErrorCode LogoutSystemUserDeviceIdentifierRequired = new(
            Code: "AZX_ADA_APP_VAL_328",
            ErrorMessage: "DeviceIdentifier cannot be null, empty, or whitespace.");

        /// <summary>
        /// Refresh token is required when logging out a system user.
        /// </summary>
        internal static readonly ErrorCode LogoutSystemUserRefreshTokenRequired = new(
            Code: "AZX_ADA_APP_VAL_329",
            ErrorMessage: "RefreshToken cannot be null, empty, or whitespace.");

        /// <summary>
        /// System user id must be positive when logging out a system user.
        /// </summary>
        internal static readonly ErrorCode LogoutSystemUserSystemUserId = new(
            Code: "AZX_ADA_APP_VAL_330",
            ErrorMessage: "SystemUserId must be greater than zero.");

        /// <summary>
        /// Logout request failed because the provided user-device-token combination is invalid.
        /// </summary>
        internal static readonly ErrorCode LogoutSystemUserSessionNotFound = new(
            Code: "AZX_ADA_APP_VAL_927",
            ErrorMessage: "Active session was not found for the provided user, device, and refresh token.");

        /// <summary>
        /// Assignment id must be positive when checking in a shift assignment.
        /// </summary>
        internal static readonly ErrorCode CheckInShiftAssignmentAssignmentId = new(
            Code: "AZX_ADA_APP_VAL_333",
            ErrorMessage: "AssignmentId must be greater than zero.");

        /// <summary>
        /// Check-in token hash is required when checking in a shift assignment.
        /// </summary>
        internal static readonly ErrorCode CheckInShiftAssignmentTokenHashRequired = new(
            Code: "AZX_ADA_APP_VAL_334",
            ErrorMessage: "CheckInTokenHash cannot be null, empty, or whitespace.");

        /// <summary>
        /// Assignment id must be positive when checking out a shift assignment.
        /// </summary>
        internal static readonly ErrorCode CheckOutShiftAssignmentAssignmentId = new(
            Code: "AZX_ADA_APP_VAL_937",
            ErrorMessage: "AssignmentId must be greater than zero.");

        /// <summary>
        /// Assignment id must be positive when supervisor confirms shift assignment check-in.
        /// </summary>
        internal static readonly ErrorCode SupervisorCheckInShiftAssignmentAssignmentId = new(
            Code: "AZX_ADA_APP_VAL_945",
            ErrorMessage: "AssignmentId must be greater than zero.");

        /// <summary>
        /// Supervisor check-in token hash is required when supervisor confirms shift assignment check-in.
        /// </summary>
        internal static readonly ErrorCode SupervisorCheckInShiftAssignmentTokenHashRequired = new(
            Code: "AZX_ADA_APP_VAL_946",
            ErrorMessage: "SupervisorCheckInTokenHash cannot be null, empty, or whitespace.");

        /// <summary>
        /// Job posting id must be positive when publishing a posting.
        /// </summary>
        internal static readonly ErrorCode PublishJobPostingJobPostingId = new(
            Code: "AZX_ADA_APP_VAL_101",
            ErrorMessage: "JobPostingId must be greater than zero.");

        /// <summary>
        /// System user id must be positive when reactivating a system user.
        /// </summary>
        internal static readonly ErrorCode ReactivateSystemUserSystemUserId = new(
            Code: "AZX_ADA_APP_VAL_303",
            ErrorMessage: "SystemUserId must be greater than zero.");

        /// <summary>
        /// Device identifier is required when refreshing a system user token.
        /// </summary>
        internal static readonly ErrorCode RefreshSystemUserTokenDeviceIdentifierRequired = new(
            Code: "AZX_ADA_APP_VAL_314",
            ErrorMessage: "DeviceIdentifier cannot be null, empty, or whitespace.");

        /// <summary>
        /// Refresh token is required when refreshing a system user token.
        /// </summary>
        internal static readonly ErrorCode RefreshSystemUserTokenRefreshTokenRequired = new(
            Code: "AZX_ADA_APP_VAL_315",
            ErrorMessage: "RefreshToken cannot be null, empty, or whitespace.");

        /// <summary>
        /// System user id must be positive when refreshing a system user token.
        /// </summary>
        internal static readonly ErrorCode RefreshSystemUserTokenSystemUserId = new(
            Code: "AZX_ADA_APP_VAL_313",
            ErrorMessage: "SystemUserId must be greater than zero.");

        /// <summary>
        /// Refresh token rotation request failed due to invalid or inactive auth session state.
        /// </summary>
        internal static readonly ErrorCode RefreshSystemUserTokenAuthenticationFailed = new(
            Code: "AZX_ADA_APP_VAL_926",
            ErrorMessage: "Refresh session is invalid or account cannot refresh token.");

        /// <summary>
        /// Employer organization name is required when registering an employer account.
        /// </summary>
        internal static readonly ErrorCode RegisterEmployerEmployerNameRequired = new(
            Code: "AZX_ADA_APP_VAL_316",
            ErrorMessage: "EmployerName cannot be null, empty, or whitespace.");

        /// <summary>
        /// Employer address city is required when registering an employer account.
        /// </summary>
        internal static readonly ErrorCode RegisterEmployerEmployerAddressCityRequired = new(
            Code: "AZX_ADA_APP_VAL_321",
            ErrorMessage: "EmployerAddressCity cannot be null, empty, or whitespace.");

        /// <summary>
        /// Employer address country is required when registering an employer account.
        /// </summary>
        internal static readonly ErrorCode RegisterEmployerEmployerAddressCountryRequired = new(
            Code: "AZX_ADA_APP_VAL_322",
            ErrorMessage: "EmployerAddressCountry cannot be null, empty, or whitespace.");

        /// <summary>
        /// Employer address line is required when registering an employer account.
        /// </summary>
        internal static readonly ErrorCode RegisterEmployerEmployerAddressLine1Required = new(
            Code: "AZX_ADA_APP_VAL_323",
            ErrorMessage: "EmployerAddressLine1 cannot be null, empty, or whitespace.");

        /// <summary>
        /// Contact first name is required when registering an employer account.
        /// </summary>
        internal static readonly ErrorCode RegisterEmployerContactFirstNameRequired = new(
            Code: "AZX_ADA_APP_VAL_324",
            ErrorMessage: "FirstName cannot be null, empty, or whitespace.");

        /// <summary>
        /// Contact last name is required when registering an employer account.
        /// </summary>
        internal static readonly ErrorCode RegisterEmployerContactLastNameRequired = new(
            Code: "AZX_ADA_APP_VAL_325",
            ErrorMessage: "LastName cannot be null, empty, or whitespace.");

        /// <summary>
        /// Contact phone is required when registering an employer account.
        /// </summary>
        internal static readonly ErrorCode RegisterEmployerContactPhoneRequired = new(
            Code: "AZX_ADA_APP_VAL_326",
            ErrorMessage: "Phone cannot be null, empty, or whitespace.");

        /// <summary>
        /// Employer tax number is required when registering an employer account.
        /// </summary>
        internal static readonly ErrorCode RegisterEmployerEmployerTaxNumberRequired = new(
            Code: "AZX_ADA_APP_VAL_317",
            ErrorMessage: "EmployerTaxNumber cannot be null, empty, or whitespace.");

        /// <summary>
        /// Email is required when registering a system user.
        /// </summary>
        internal static readonly ErrorCode RegisterSystemUserEmailRequired = new(
            Code: "AZX_ADA_APP_VAL_318",
            ErrorMessage: "Email cannot be null, empty, or whitespace.");

        /// <summary>
        /// Email must be unique across all system users.
        /// </summary>
        internal static readonly ErrorCode RegisterSystemUserEmailAlreadyExists = new(
            Code: "AZX_ADA_APP_VAL_319",
            ErrorMessage: "A system user with the same email already exists.");

        /// <summary>
        /// Password is required when registering a system user.
        /// </summary>
        internal static readonly ErrorCode RegisterSystemUserPasswordRequired = new(
            Code: "AZX_ADA_APP_VAL_320",
            ErrorMessage: "Password cannot be null, empty, or whitespace.");

        /// <summary>
        /// Application id must be positive when rejecting a job posting application.
        /// </summary>
        internal static readonly ErrorCode RejectJobPostingApplicationApplicationId = new(
            Code: "AZX_ADA_APP_VAL_116",
            ErrorMessage: "ApplicationId must be greater than zero.");

        /// <summary>
        /// Job posting id must be positive when rejecting a job posting application.
        /// </summary>
        internal static readonly ErrorCode RejectJobPostingApplicationJobPostingId = new(
            Code: "AZX_ADA_APP_VAL_115",
            ErrorMessage: "JobPostingId must be greater than zero.");

        /// <summary>
        /// Job posting id must be positive when removing a posting skill.
        /// </summary>
        internal static readonly ErrorCode RemoveJobPostingSkillJobPostingId = new(
            Code: "AZX_ADA_APP_VAL_129",
            ErrorMessage: "JobPostingId must be greater than zero.");

        /// <summary>
        /// Skill id must be positive when removing a posting skill.
        /// </summary>
        internal static readonly ErrorCode RemoveJobPostingSkillSkillId = new(
            Code: "AZX_ADA_APP_VAL_130",
            ErrorMessage: "SkillId must be greater than zero.");

        /// <summary>
        /// System user id must be positive when requesting email verification.
        /// </summary>
        internal static readonly ErrorCode RequestSystemUserEmailVerificationSystemUserId = new(
            Code: "AZX_ADA_APP_VAL_304",
            ErrorMessage: "SystemUserId must be greater than zero.");

        /// <summary>
        /// Token hash is required when requesting email verification.
        /// </summary>
        internal static readonly ErrorCode RequestSystemUserEmailVerificationTokenHash = new(
            Code: "AZX_ADA_APP_VAL_305",
            ErrorMessage: "TokenHash cannot be null, empty, or whitespace.");

        /// <summary>
        /// Expiration must be in the future when requesting email verification.
        /// </summary>
        internal static readonly ErrorCode RequestSystemUserEmailVerificationExpiresAtFuture = new(
            Code: "AZX_ADA_APP_VAL_327",
            ErrorMessage: "ExpiresAt must be a future date/time.");

        /// <summary>
        /// Job posting id must be positive when submitting an application.
        /// </summary>
        internal static readonly ErrorCode SubmitJobPostingApplicationJobPostingId = new(
            Code: "AZX_ADA_APP_VAL_801",
            ErrorMessage: "JobPostingId must be greater than zero.");

        /// <summary>
        /// Employer id must be positive when suspending an employer.
        /// </summary>
        internal static readonly ErrorCode SuspendEmployerEmployerId = new(
            Code: "AZX_ADA_APP_VAL_202",
            ErrorMessage: "EmployerId must be greater than zero.");

        /// <summary>
        /// Commission rate must be between 0 and 1.
        /// </summary>
        internal static readonly ErrorCode SetEmployerCommissionRateCommissionRate = new(
            Code: "AZX_ADA_APP_VAL_903",
            ErrorMessage: "CommissionRate must be between 0 and 1.");

        /// <summary>
        /// Employer id must be positive when setting employer commission rate.
        /// </summary>
        internal static readonly ErrorCode SetEmployerCommissionRateEmployerId = new(
            Code: "AZX_ADA_APP_VAL_904",
            ErrorMessage: "EmployerId must be greater than zero.");

        /// <summary>
        /// Employer id must be positive for employer commission policy query.
        /// </summary>
        internal static readonly ErrorCode GetEmployerCommissionPolicyEmployerId = new(
            Code: "AZX_ADA_APP_VAL_905",
            ErrorMessage: "EmployerId must be greater than zero.");

        /// <summary>
        /// Employer id must be positive for employer commission estimate query.
        /// </summary>
        internal static readonly ErrorCode GetEmployerCommissionEstimateEmployerId = new(
            Code: "AZX_ADA_APP_VAL_906",
            ErrorMessage: "EmployerId must be greater than zero.");

        /// <summary>
        /// System user id must be positive when suspending a system user.
        /// </summary>
        internal static readonly ErrorCode SuspendSystemUserSystemUserId = new(
            Code: "AZX_ADA_APP_VAL_302",
            ErrorMessage: "SystemUserId must be greater than zero.");

        /// <summary>
        /// Description is required when updating a job posting.
        /// </summary>
        internal static readonly ErrorCode UpdateJobPostingDescriptionRequired = new(
            Code: "AZX_ADA_APP_VAL_108",
            ErrorMessage: "Description cannot be null, empty, or whitespace.");

        /// <summary>
        /// Head count must be positive when updating a job posting.
        /// </summary>
        internal static readonly ErrorCode UpdateJobPostingHeadCountPositive = new(
            Code: "AZX_ADA_APP_VAL_109",
            ErrorMessage: "HeadCount must be greater than zero.");

        /// <summary>
        /// Job posting id must be positive when updating a job posting.
        /// </summary>
        internal static readonly ErrorCode UpdateJobPostingJobPostingId = new(
            Code: "AZX_ADA_APP_VAL_106",
            ErrorMessage: "JobPostingId must be greater than zero.");

        /// <summary>
        /// Shift end must be after shift start when updating a job posting.
        /// </summary>
        internal static readonly ErrorCode UpdateJobPostingShiftEndAfterStart = new(
            Code: "AZX_ADA_APP_VAL_112",
            ErrorMessage: "ShiftEndTime must be after ShiftStartTime.");

        /// <summary>
        /// Title is required when updating a job posting.
        /// </summary>
        internal static readonly ErrorCode UpdateJobPostingTitleRequired = new(
            Code: "AZX_ADA_APP_VAL_107",
            ErrorMessage: "Title cannot be null, empty, or whitespace.");

        /// <summary>
        /// Wage amount must be positive when updating a job posting.
        /// </summary>
        internal static readonly ErrorCode UpdateJobPostingWageAmountPositive = new(
            Code: "AZX_ADA_APP_VAL_111",
            ErrorMessage: "WageAmount must be greater than zero.");

        /// <summary>
        /// Wage currency is required when updating a job posting.
        /// </summary>
        internal static readonly ErrorCode UpdateJobPostingWageCurrencyRequired = new(
            Code: "AZX_ADA_APP_VAL_110",
            ErrorMessage: "WageCurrency cannot be null, empty, or whitespace.");

        /// <summary>
        /// System user id must be positive when verifying email.
        /// </summary>
        internal static readonly ErrorCode VerifySystemUserEmailSystemUserId = new(
            Code: "AZX_ADA_APP_VAL_306",
            ErrorMessage: "SystemUserId must be greater than zero.");

        /// <summary>
        /// Token hash is required when verifying email.
        /// </summary>
        internal static readonly ErrorCode VerifySystemUserEmailTokenHash = new(
            Code: "AZX_ADA_APP_VAL_307",
            ErrorMessage: "TokenHash cannot be null, empty, or whitespace.");

        /// <summary>
        /// Application id must be positive when withdrawing an application.
        /// </summary>
        internal static readonly ErrorCode WithdrawJobPostingApplicationApplicationId = new(
            Code: "AZX_ADA_APP_VAL_803",
            ErrorMessage: "ApplicationId must be greater than zero.");

        /// <summary>
        /// Job posting id must be positive when withdrawing an application.
        /// </summary>
        internal static readonly ErrorCode WithdrawJobPostingApplicationJobPostingId = new(
            Code: "AZX_ADA_APP_VAL_804",
            ErrorMessage: "JobPostingId must be greater than zero.");

        #endregion Properties
    }
}
