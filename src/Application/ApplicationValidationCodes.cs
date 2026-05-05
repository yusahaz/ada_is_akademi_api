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
        /// Job application id must be positive when creating a shift assignment.
        /// </summary>
        internal static readonly ErrorCode CreateShiftAssignmentJobApplicationId = new(
            Code: "AZX_ADA_APP_VAL_332",
            ErrorMessage: "JobApplicationId must be greater than zero.");

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
        /// Job posting id must be positive when listing applications by posting.
        /// </summary>
        internal static readonly ErrorCode ListJobApplicationsByJobPostingIdJobPostingId = new(
            Code: "AZX_ADA_APP_VAL_701",
            ErrorMessage: "JobPostingId must be greater than zero.");

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
