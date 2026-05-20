namespace Azoxia.AdaIsAkademi.Application.DependencyInjection
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Registers application-layer validators, command handlers, and query handlers.
    /// </summary>
    public class ServiceRegister :
        IServiceRegister
    {
        #region Methods

        /// <inheritdoc />
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAdaIsDomainEventHandling();
            services.AddScoped<IWorkerProfileCompletionEvaluator, WorkerProfileCompletionEvaluator>();
            services.AddScoped<IWorkerEmployerProfileAccess, WorkerEmployerProfileAccess>();
            services.AddScoped<IEmbeddingVectorizer, HashEmbeddingVectorizer>();
            RegisterCommandHandlers(services);
            RegisterQueryHandlers(services);
            RegisterValidators(services);
        }

        /// <summary>
        /// Registers <see cref="IRequestHandler{TRequest, TResult}"/> implementations for commands (result <see cref="Unit"/>).
        /// </summary>
        /// <param name="services">The application service collection.</param>
        private void RegisterCommandHandlers(IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<AcceptJobPostingApplicationCommand, Unit>, AcceptJobPostingApplicationCommandHandler>();
            services.AddScoped<IRequestHandler<ActivateEmployerCommand, Unit>, ActivateEmployerCommandHandler>();
            services.AddScoped<IRequestHandler<AddEmployerLocationCommand, int>, AddEmployerLocationCommandHandler>();
            services.AddScoped<IRequestHandler<AddEmployerSupervisorCommand, int>, AddEmployerSupervisorCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteEmployerLocationCommand, Unit>, DeleteEmployerLocationCommandHandler>();
            services.AddScoped<IRequestHandler<AddWorkerAvailabilityCommand, int>, AddWorkerAvailabilityCommandHandler>();
            services.AddScoped<IRequestHandler<AddWorkerCertificateCommand, int>, AddWorkerCertificateCommandHandler>();
            services.AddScoped<IRequestHandler<AddWorkerEducationCommand, int>, AddWorkerEducationCommandHandler>();
            services.AddScoped<IRequestHandler<AddWorkerExperienceCommand, int>, AddWorkerExperienceCommandHandler>();
            services.AddScoped<IRequestHandler<AddWorkerLanguageCommand, int>, AddWorkerLanguageCommandHandler>();
            services.AddScoped<IRequestHandler<AddWorkerReferenceCommand, int>, AddWorkerReferenceCommandHandler>();
            services.AddScoped<IRequestHandler<AddJobPostingSkillCommand, int>, AddJobPostingSkillCommandHandler>();
            services.AddScoped<IRequestHandler<AddWorkerSkillCommand, int>, AddWorkerSkillCommandHandler>();
            services.AddScoped<IRequestHandler<BanEmployerCommand, Unit>, BanEmployerCommandHandler>();
            services.AddScoped<IRequestHandler<GenerateCommissionReceivableCommand, int>, GenerateCommissionReceivableCommandHandler>();
            services.AddScoped<IRequestHandler<InitEmployerLogoUploadCommand, ObjectStorageUploadInitModel>, InitEmployerLogoUploadCommandHandler>();
            services.AddScoped<IRequestHandler<InitWorkerCvUploadCommand, ObjectStorageUploadInitModel>, InitWorkerCvUploadCommandHandler>();
            services.AddScoped<IRequestHandler<InitWorkerProfilePhotoUploadCommand, ObjectStorageUploadInitModel>, InitWorkerProfilePhotoUploadCommandHandler>();
            services.AddScoped<IRequestHandler<CreateWorkerPayoutCommand, WorkerPayoutSnapshotModel>, CreateWorkerPayoutCommandHandler>();
            services.AddScoped<IRequestHandler<MarkWorkerPayoutAsProcessingCommand, WorkerPayoutSnapshotModel>, MarkWorkerPayoutAsProcessingCommandHandler>();
            services.AddScoped<IRequestHandler<FailWorkerPayoutCommand, WorkerPayoutSnapshotModel>, FailWorkerPayoutCommandHandler>();
            services.AddScoped<IRequestHandler<RetryWorkerPayoutCommand, WorkerPayoutSnapshotModel>, RetryWorkerPayoutCommandHandler>();
            services.AddScoped<IRequestHandler<ConfirmEmployerLogoUploadCommand, Unit>, ConfirmEmployerLogoUploadCommandHandler>();
            services.AddScoped<IRequestHandler<ConfirmWorkerCvUploadCommand, int>, ConfirmWorkerCvUploadCommandHandler>();
            services.AddScoped<IRequestHandler<ConfirmWorkerProfilePhotoUploadCommand, Unit>, ConfirmWorkerProfilePhotoUploadCommandHandler>();
            services.AddScoped<IRequestHandler<ConfirmWorkerPayoutCommand, WorkerPayoutSnapshotModel>, ConfirmWorkerPayoutCommandHandler>();
            services.AddScoped<IRequestHandler<SetEmployerCommissionRateCommand, Unit>, SetEmployerCommissionRateCommandHandler>();
            services.AddScoped<IRequestHandler<BanSystemUserCommand, Unit>, BanSystemUserCommandHandler>();
            services.AddScoped<IRequestHandler<RunCvExtractionSweepCommand, int>, RunCvExtractionSweepCommandHandler>();
            services.AddScoped<IRequestHandler<RunOverdueAlarmSweepCommand, int>, RunOverdueAlarmSweepCommandHandler>();
            services.AddScoped<IRequestHandler<RunEmbeddingRefreshSweepCommand, int>, RunEmbeddingRefreshSweepCommandHandler>();
            services.AddScoped<IRequestHandler<RetryFailedSystemUserNotificationsCommand, int>, RetryFailedSystemUserNotificationsCommandHandler>();
            services.AddScoped<IRequestHandler<CancelJobPostingCommand, Unit>, CancelJobPostingCommandHandler>();
            services.AddScoped<IRequestHandler<ChangeSystemUserPasswordCommand, Unit>, ChangeSystemUserPasswordCommandHandler>();
            services.AddScoped<IRequestHandler<CheckInShiftAssignmentCommand, Unit>, CheckInShiftAssignmentCommandHandler>();
            services.AddScoped<IRequestHandler<SupervisorCheckInShiftAssignmentCommand, Unit>, SupervisorCheckInShiftAssignmentCommandHandler>();
            services.AddScoped<IRequestHandler<CheckOutShiftAssignmentCommand, Unit>, CheckOutShiftAssignmentCommandHandler>();
            services.AddScoped<IRequestHandler<ClearEmployerLogoCommand, Unit>, ClearEmployerLogoCommandHandler>();
            services.AddScoped<IRequestHandler<ClearWorkerProfilePhotoCommand, Unit>, ClearWorkerProfilePhotoCommandHandler>();
            services.AddScoped<IRequestHandler<CompleteJobPostingCommand, Unit>, CompleteJobPostingCommandHandler>();
            services.AddScoped<IRequestHandler<CreateShiftAssignmentCommand, int>, CreateShiftAssignmentCommandHandler>();
            services.AddScoped<IRequestHandler<CreateJobPostingCommand, int>, CreateJobPostingCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteEmployerCommand, Unit>, DeleteEmployerCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteWorkerCommand, Unit>, DeleteWorkerCommandHandler>();
            services.AddScoped<IRequestHandler<PublishJobPostingCommand, Unit>, PublishJobPostingCommandHandler>();
            services.AddScoped<IRequestHandler<ReactivateSystemUserCommand, Unit>, ReactivateSystemUserCommandHandler>();
            services.AddScoped<IRequestHandler<RegisterAdminCommand, int>, RegisterAdminCommandHandler>();
            services.AddScoped<IRequestHandler<RegisterEmployerCommand, int>, RegisterEmployerCommandHandler>();
            services.AddScoped<IRequestHandler<RegisterWorkerCommand, int>, RegisterWorkerCommandHandler>();
            services.AddScoped<IRequestHandler<RefreshSystemUserTokenCommand, SystemUserTokenModel>, RefreshSystemUserTokenCommandHandler>();
            services.AddScoped<IRequestHandler<RejectJobPostingApplicationCommand, Unit>, RejectJobPostingApplicationCommandHandler>();
            services.AddScoped<IRequestHandler<RecordEmployerWorkerProfileViewCommand, RecordEmployerWorkerProfileViewResultModel>, RecordEmployerWorkerProfileViewCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveEmployerSupervisorCommand, Unit>, RemoveEmployerSupervisorCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveWorkerAvailabilityCommand, Unit>, RemoveWorkerAvailabilityCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveWorkerCertificateCommand, Unit>, RemoveWorkerCertificateCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveWorkerEducationCommand, Unit>, RemoveWorkerEducationCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveWorkerExperienceCommand, Unit>, RemoveWorkerExperienceCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveWorkerLanguageCommand, Unit>, RemoveWorkerLanguageCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveJobPostingSkillCommand, Unit>, RemoveJobPostingSkillCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveWorkerReferenceCommand, Unit>, RemoveWorkerReferenceCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveWorkerSkillCommand, Unit>, RemoveWorkerSkillCommandHandler>();
            services.AddScoped<IRequestHandler<ConfirmWorkerCvReviewCommand, Unit>, ConfirmWorkerCvReviewCommandHandler>();
            services.AddScoped<IRequestHandler<DiscardWorkerCvReviewCommand, Unit>, DiscardWorkerCvReviewCommandHandler>();
            services.AddScoped<IRequestHandler<RequestSystemUserEmailVerificationCommand, Unit>, RequestSystemUserEmailVerificationCommandHandler>();
            services.AddScoped<IRequestHandler<LoginSystemUserCommand, SystemUserTokenModel>, LoginSystemUserCommandHandler>();
            services.AddScoped<IRequestHandler<MarkAllNotificationsAsReadCommand, Unit>, MarkAllNotificationsAsReadCommandHandler>();
            services.AddScoped<IRequestHandler<LogoutSystemUserCommand, Unit>, LogoutSystemUserCommandHandler>();
            services.AddScoped<IRequestHandler<MarkNotificationAsReadCommand, Unit>, MarkNotificationAsReadCommandHandler>();
            services.AddScoped<IRequestHandler<SuspendEmployerCommand, Unit>, SuspendEmployerCommandHandler>();
            services.AddScoped<IRequestHandler<SuspendSystemUserCommand, Unit>, SuspendSystemUserCommandHandler>();
            services.AddScoped<IRequestHandler<SubmitJobPostingApplicationCommand, int>, SubmitJobPostingApplicationCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateWorkerMatchingPreferencesCommand, Unit>, UpdateWorkerMatchingPreferencesCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateWorkerBioCommand, Unit>, UpdateWorkerBioCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateEmployerSocialLinksCommand, Unit>, UpdateEmployerSocialLinksCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateEmployerProfileCommand, Unit>, UpdateEmployerProfileCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateEmployerLocationCommand, Unit>, UpdateEmployerLocationCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateWorkerSocialLinksCommand, Unit>, UpdateWorkerSocialLinksCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateWorkerProfileCommand, Unit>, UpdateWorkerProfileCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateWorkerCvTemplatePreferenceCommand, Unit>, UpdateWorkerCvTemplatePreferenceCommandHandler>();
            services.AddScoped<IRequestHandler<SendWorkerNotificationCommand, int>, SendWorkerNotificationCommandHandler>();
            services.AddScoped<IRequestHandler<SendSystemUserNotificationCommand, int>, SendSystemUserNotificationCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateJobPostingCommand, Unit>, UpdateJobPostingCommandHandler>();
            services.AddScoped<IRequestHandler<VerifySystemUserEmailCommand, Unit>, VerifySystemUserEmailCommandHandler>();
            services.AddScoped<IRequestHandler<WithdrawJobPostingApplicationCommand, Unit>, WithdrawJobPostingApplicationCommandHandler>();
        }

        /// <summary>
        /// Registers <see cref="IRequestHandler{TRequest, TResult}"/> implementations for queries.
        /// </summary>
        /// <param name="services">The application service collection.</param>
        private void RegisterQueryHandlers(IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<GetEmployerByIdQuery, EmployerDetailModel>, GetEmployerByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployerDetailQuery, EmployerFullDetailModel>, GetEmployerDetailQueryHandler>();
            services.AddScoped<IRequestHandler<GetSpotDashboardSummaryQuery, SpotDashboardSummaryModel>, GetSpotDashboardSummaryQueryHandler>();
            services.AddScoped<IRequestHandler<GetWorkerPortfolioQuery, IReadOnlyList<WorkerPortfolioListItemModel>>, GetWorkerPortfolioQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployerLogoViewUrlQuery, MediaBlobViewUrlModel>, GetEmployerLogoViewUrlQueryHandler>();
            services.AddScoped<IRequestHandler<ListEmployersQuery, PagedQueryResultModel<EmployerListItemModel>>, ListEmployersQueryHandler>();
            services.AddScoped<IRequestHandler<GetCommissionReceivableByPeriodQuery, CommissionReceivableDetailModel>, GetCommissionReceivableByPeriodQueryHandler>();
            services.AddScoped<IRequestHandler<ListCommissionReceivablesByEmployerQuery, PagedQueryResultModel<CommissionReceivableListItemModel>>, ListCommissionReceivablesByEmployerQueryHandler>();
            services.AddScoped<IRequestHandler<ListEmployerDisputesQuery, PagedQueryResultModel<EmployerDisputeListItemModel>>, ListEmployerDisputesQueryHandler>();
            services.AddScoped<IRequestHandler<ListEmployerLocationsQuery, PagedQueryResultModel<EmployerLocationListItemModel>>, ListEmployerLocationsQueryHandler>();
            services.AddScoped<IRequestHandler<ListEmployerSupervisorsQuery, IReadOnlyList<EmployerSupervisorListItemModel>>, ListEmployerSupervisorsQueryHandler>();
            services.AddScoped<IRequestHandler<ListWorkerPayoutsQuery, PagedQueryResultModel<WorkerPayoutListItemModel>>, ListWorkerPayoutsQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployerCommissionEstimateQuery, EmployerCommissionEstimateModel>, GetEmployerCommissionEstimateQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployerCommissionPolicyQuery, EmployerCommissionPolicyModel>, GetEmployerCommissionPolicyQueryHandler>();
            services.AddScoped<IRequestHandler<ListEmployerCommissionSummariesQuery, IReadOnlyList<EmployerCommissionListItemModel>>, ListEmployerCommissionSummariesQueryHandler>();
            services.AddScoped<IRequestHandler<ExportEmployerCommissionPoliciesCsvQuery, EmployerCommissionPolicyExportPackageModel>, ExportEmployerCommissionPoliciesCsvQueryHandler>();
            services.AddScoped<IRequestHandler<GetDashboardStatisticsQuery, DashboardStatisticsModel>, GetDashboardStatisticsQueryHandler>();
            services.AddScoped<IRequestHandler<ExportOverdueAlarmsCsvQuery, OverdueAlarmExportPackageModel>, ExportOverdueAlarmsCsvQueryHandler>();
            services.AddScoped<IRequestHandler<ExportSystemUserNotificationDispatchesCsvQuery, SystemUserNotificationDispatchExportPackageModel>, ExportSystemUserNotificationDispatchesCsvQueryHandler>();
            services.AddScoped<IRequestHandler<GetOverdueJobSummaryQuery, OverdueJobSummaryModel>, GetOverdueJobSummaryQueryHandler>();
            services.AddScoped<IRequestHandler<GetMonetizationSummaryQuery, MonetizationSummaryModel>, GetMonetizationSummaryQueryHandler>();
            services.AddScoped<IRequestHandler<GetFinancialReconciliationSummaryQuery, FinancialReconciliationSummaryModel>, GetFinancialReconciliationSummaryQueryHandler>();
            services.AddScoped<IRequestHandler<GetCommissionRevenueSeriesQuery, CommissionRevenueSeriesModel>, GetCommissionRevenueSeriesQueryHandler>();
            services.AddScoped<IRequestHandler<ListFinancialReconciliationRowsQuery, PagedQueryResultModel<FinancialReconciliationListItemModel>>, ListFinancialReconciliationRowsQueryHandler>();
            services.AddScoped<IRequestHandler<GetJobPostingByIdQuery, JobPostingDetailModel>, GetJobPostingByIdQueryHandler>();
            services.AddScoped<IRequestHandler<ListMyNotificationsQuery, PagedQueryResultModel<SystemUserNotificationListItemModel>>, ListMyNotificationsQueryHandler>();
            services.AddScoped<IRequestHandler<GetSystemUserMeQuery, SystemUserMeModel>, GetSystemUserMeQueryHandler>();
            services.AddScoped<IRequestHandler<GetSystemUserByIdQuery, SystemUserMeModel>, GetSystemUserByIdQueryHandler>();
            services.AddScoped<IRequestHandler<ListSystemUsersQuery, PagedQueryResultModel<SystemUserListItemModel>>, ListSystemUsersQueryHandler>();
            services.AddScoped<IRequestHandler<GetWorkerPersonalizedNotificationPreviewQuery, WorkerNotificationPreviewModel>, GetWorkerPersonalizedNotificationPreviewQueryHandler>();
            services.AddScoped<IRequestHandler<GetWorkerProfilePhotoViewUrlQuery, MediaBlobViewUrlModel>, GetWorkerProfilePhotoViewUrlQueryHandler>();
            services.AddScoped<IRequestHandler<GetWorkerLiveStatusFeedQuery, WorkerLiveStatusFeedModel>, GetWorkerLiveStatusFeedQueryHandler>();
            services.AddScoped<IRequestHandler<GetWorkerByIdQuery, WorkerEmployerSafeDetailModel>, GetWorkerByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetWorkerDetailQuery, WorkerEmployerSafeFullDetailModel>, GetWorkerDetailQueryHandler>();
            services.AddScoped<IRequestHandler<GetWorkerSelfDetailQuery, WorkerSelfDetailModel>, GetWorkerSelfDetailQueryHandler>();
            services.AddScoped<IRequestHandler<GetWorkerSelfFullDetailQuery, WorkerSelfFullDetailModel>, GetWorkerSelfFullDetailQueryHandler>();
            services.AddScoped<IRequestHandler<GetWorkerActiveCvUploadSessionQuery, WorkerActiveCvUploadSessionModel?>, GetWorkerActiveCvUploadSessionQueryHandler>();
            services.AddScoped<IRequestHandler<ListGlobalSkillsQuery, IReadOnlyList<string>>, ListGlobalSkillsQueryHandler>();
            services.AddScoped<IRequestHandler<ListWorkersQuery, PagedQueryResultModel<WorkerListItemModel>>, ListWorkersQueryHandler>();
            services.AddScoped<IRequestHandler<SemanticSearchWorkersQuery, PagedQueryResultModel<SemanticSearchedWorkerListItemModel>>, SemanticSearchWorkersQueryHandler>();
            services.AddScoped<IRequestHandler<ListJobApplicationsByJobPostingIdQuery, PagedQueryResultModel<JobApplicationListItemModel>>, ListJobApplicationsByJobPostingIdQueryHandler>();
            services.AddScoped<IRequestHandler<ListMyJobApplicationsQuery, PagedQueryResultModel<WorkerJobApplicationListItemModel>>, ListMyJobApplicationsQueryHandler>();
            services.AddScoped<IRequestHandler<ListMyShiftAssignmentsQuery, PagedQueryResultModel<WorkerShiftAssignmentListItemModel>>, ListMyShiftAssignmentsQueryHandler>();
            services.AddScoped<IRequestHandler<ListEmployerShiftAssignmentsQuery, PagedQueryResultModel<WorkerShiftAssignmentListItemModel>>, ListEmployerShiftAssignmentsQueryHandler>();
            services.AddScoped<IRequestHandler<ListShiftAssignmentsHistoryQuery, PagedQueryResultModel<ShiftAssignmentHistoryListItemModel>>, ListShiftAssignmentsHistoryQueryHandler>();
            services.AddScoped<IRequestHandler<ListJobPostingsByEmployerIdQuery, PagedQueryResultModel<JobPostingSummaryModel>>, ListJobPostingsByEmployerIdQueryHandler>();
            services.AddScoped<IRequestHandler<ListOpenJobPostingsQuery, PagedQueryResultModel<JobPostingSummaryModel>>, ListOpenJobPostingsQueryHandler>();
            services.AddScoped<IRequestHandler<ListSemanticMatchedJobPostingsQuery, IReadOnlyList<SemanticMatchedJobPostingModel>>, ListSemanticMatchedJobPostingsQueryHandler>();
        }

        /// <summary>
        /// Registers <see cref="IRequestValidator{TRequest}"/> implementations.
        /// </summary>
        /// <param name="services">The application service collection.</param>
        private void RegisterValidators(IServiceCollection services)
        {
            services.AddScoped<IRequestValidator<AcceptJobPostingApplicationCommand>, AcceptJobPostingApplicationCommandValidator>();
            services.AddScoped<IRequestValidator<ActivateEmployerCommand>, ActivateEmployerCommandValidator>();
            services.AddScoped<IRequestValidator<AddEmployerLocationCommand>, AddEmployerLocationCommandValidator>();
            services.AddScoped<IRequestValidator<AddEmployerSupervisorCommand>, AddEmployerSupervisorCommandValidator>();
            services.AddScoped<IRequestValidator<DeleteEmployerLocationCommand>, DeleteEmployerLocationCommandValidator>();
            services.AddScoped<IRequestValidator<AddWorkerAvailabilityCommand>, AddWorkerAvailabilityCommandValidator>();
            services.AddScoped<IRequestValidator<AddWorkerCertificateCommand>, AddWorkerCertificateCommandValidator>();
            services.AddScoped<IRequestValidator<AddWorkerEducationCommand>, AddWorkerEducationCommandValidator>();
            services.AddScoped<IRequestValidator<AddWorkerExperienceCommand>, AddWorkerExperienceCommandValidator>();
            services.AddScoped<IRequestValidator<AddWorkerLanguageCommand>, AddWorkerLanguageCommandValidator>();
            services.AddScoped<IRequestValidator<AddWorkerReferenceCommand>, AddWorkerReferenceCommandValidator>();
            services.AddScoped<IRequestValidator<AddJobPostingSkillCommand>, AddJobPostingSkillCommandValidator>();
            services.AddScoped<IRequestValidator<AddWorkerSkillCommand>, AddWorkerSkillCommandValidator>();
            services.AddScoped<IRequestValidator<BanEmployerCommand>, BanEmployerCommandValidator>();
            services.AddScoped<IRequestValidator<GenerateCommissionReceivableCommand>, GenerateCommissionReceivableCommandValidator>();
            services.AddScoped<IRequestValidator<InitEmployerLogoUploadCommand>, InitEmployerLogoUploadCommandValidator>();
            services.AddScoped<IRequestValidator<InitWorkerCvUploadCommand>, InitWorkerCvUploadCommandValidator>();
            services.AddScoped<IRequestValidator<InitWorkerProfilePhotoUploadCommand>, InitWorkerProfilePhotoUploadCommandValidator>();
            services.AddScoped<IRequestValidator<CreateWorkerPayoutCommand>, CreateWorkerPayoutCommandValidator>();
            services.AddScoped<IRequestValidator<MarkWorkerPayoutAsProcessingCommand>, MarkWorkerPayoutAsProcessingCommandValidator>();
            services.AddScoped<IRequestValidator<FailWorkerPayoutCommand>, FailWorkerPayoutCommandValidator>();
            services.AddScoped<IRequestValidator<RetryWorkerPayoutCommand>, RetryWorkerPayoutCommandValidator>();
            services.AddScoped<IRequestValidator<ConfirmWorkerPayoutCommand>, ConfirmWorkerPayoutCommandValidator>();
            services.AddScoped<IRequestValidator<SetEmployerCommissionRateCommand>, SetEmployerCommissionRateCommandValidator>();
            services.AddScoped<IRequestValidator<BanSystemUserCommand>, BanSystemUserCommandValidator>();
            services.AddScoped<IRequestValidator<RunCvExtractionSweepCommand>, RunCvExtractionSweepCommandValidator>();
            services.AddScoped<IRequestValidator<RunOverdueAlarmSweepCommand>, RunOverdueAlarmSweepCommandValidator>();
            services.AddScoped<IRequestValidator<RunEmbeddingRefreshSweepCommand>, RunEmbeddingRefreshSweepCommandValidator>();
            services.AddScoped<IRequestValidator<RetryFailedSystemUserNotificationsCommand>, RetryFailedSystemUserNotificationsCommandValidator>();
            services.AddScoped<IRequestValidator<CancelJobPostingCommand>, CancelJobPostingCommandValidator>();
            services.AddScoped<IRequestValidator<ChangeSystemUserPasswordCommand>, ChangeSystemUserPasswordCommandValidator>();
            services.AddScoped<IRequestValidator<CheckInShiftAssignmentCommand>, CheckInShiftAssignmentCommandValidator>();
            services.AddScoped<IRequestValidator<SupervisorCheckInShiftAssignmentCommand>, SupervisorCheckInShiftAssignmentCommandValidator>();
            services.AddScoped<IRequestValidator<CheckOutShiftAssignmentCommand>, CheckOutShiftAssignmentCommandValidator>();
            services.AddScoped<IRequestValidator<ClearEmployerLogoCommand>, ClearEmployerLogoCommandValidator>();
            services.AddScoped<IRequestValidator<ClearWorkerProfilePhotoCommand>, ClearWorkerProfilePhotoCommandValidator>();
            services.AddScoped<IRequestValidator<ConfirmEmployerLogoUploadCommand>, ConfirmEmployerLogoUploadCommandValidator>();
            services.AddScoped<IRequestValidator<ConfirmWorkerCvUploadCommand>, ConfirmWorkerCvUploadCommandValidator>();
            services.AddScoped<IRequestValidator<ConfirmWorkerCvReviewCommand>, ConfirmWorkerCvReviewCommandValidator>();
            services.AddScoped<IRequestValidator<ConfirmWorkerProfilePhotoUploadCommand>, ConfirmWorkerProfilePhotoUploadCommandValidator>();
            services.AddScoped<IRequestValidator<CompleteJobPostingCommand>, CompleteJobPostingCommandValidator>();
            services.AddScoped<IRequestValidator<CreateShiftAssignmentCommand>, CreateShiftAssignmentCommandValidator>();
            services.AddScoped<IRequestValidator<CreateJobPostingCommand>, CreateJobPostingCommandValidator>();
            services.AddScoped<IRequestValidator<DeleteEmployerCommand>, DeleteEmployerCommandValidator>();
            services.AddScoped<IRequestValidator<DeleteWorkerCommand>, DeleteWorkerCommandValidator>();
            services.AddScoped<IRequestValidator<GetEmployerByIdQuery>, GetEmployerByIdQueryValidator>();
            services.AddScoped<IRequestValidator<GetEmployerDetailQuery>, GetEmployerDetailQueryValidator>();
            services.AddScoped<IRequestValidator<GetSpotDashboardSummaryQuery>, GetSpotDashboardSummaryQueryValidator>();
            services.AddScoped<IRequestValidator<GetWorkerPortfolioQuery>, GetWorkerPortfolioQueryValidator>();
            services.AddScoped<IRequestValidator<GetEmployerLogoViewUrlQuery>, GetEmployerLogoViewUrlQueryValidator>();
            services.AddScoped<IRequestValidator<ListEmployersQuery>, ListEmployersQueryValidator>();
            services.AddScoped<IRequestValidator<GetCommissionReceivableByPeriodQuery>, GetCommissionReceivableByPeriodQueryValidator>();
            services.AddScoped<IRequestValidator<ListCommissionReceivablesByEmployerQuery>, ListCommissionReceivablesByEmployerQueryValidator>();
            services.AddScoped<IRequestValidator<ListEmployerDisputesQuery>, ListEmployerDisputesQueryValidator>();
            services.AddScoped<IRequestValidator<ListEmployerLocationsQuery>, ListEmployerLocationsQueryValidator>();
            services.AddScoped<IRequestValidator<ListEmployerSupervisorsQuery>, ListEmployerSupervisorsQueryValidator>();
            services.AddScoped<IRequestValidator<ListWorkerPayoutsQuery>, ListWorkerPayoutsQueryValidator>();
            services.AddScoped<IRequestValidator<GetEmployerCommissionEstimateQuery>, GetEmployerCommissionEstimateQueryValidator>();
            services.AddScoped<IRequestValidator<GetEmployerCommissionPolicyQuery>, GetEmployerCommissionPolicyQueryValidator>();
            services.AddScoped<IRequestValidator<ListEmployerCommissionSummariesQuery>, ListEmployerCommissionSummariesQueryValidator>();
            services.AddScoped<IRequestValidator<ExportEmployerCommissionPoliciesCsvQuery>, ExportEmployerCommissionPoliciesCsvQueryValidator>();
            services.AddScoped<IRequestValidator<GetDashboardStatisticsQuery>, GetDashboardStatisticsQueryValidator>();
            services.AddScoped<IRequestValidator<ExportOverdueAlarmsCsvQuery>, ExportOverdueAlarmsCsvQueryValidator>();
            services.AddScoped<IRequestValidator<ExportSystemUserNotificationDispatchesCsvQuery>, ExportSystemUserNotificationDispatchesCsvQueryValidator>();
            services.AddScoped<IRequestValidator<GetOverdueJobSummaryQuery>, GetOverdueJobSummaryQueryValidator>();
            services.AddScoped<IRequestValidator<GetMonetizationSummaryQuery>, GetMonetizationSummaryQueryValidator>();
            services.AddScoped<IRequestValidator<GetFinancialReconciliationSummaryQuery>, GetFinancialReconciliationSummaryQueryValidator>();
            services.AddScoped<IRequestValidator<GetCommissionRevenueSeriesQuery>, GetCommissionRevenueSeriesQueryValidator>();
            services.AddScoped<IRequestValidator<ListFinancialReconciliationRowsQuery>, ListFinancialReconciliationRowsQueryValidator>();
            services.AddScoped<IRequestValidator<GetJobPostingByIdQuery>, GetJobPostingByIdQueryValidator>();
            services.AddScoped<IRequestValidator<ListMyNotificationsQuery>, ListMyNotificationsQueryValidator>();
            services.AddScoped<IRequestValidator<GetSystemUserMeQuery>, GetSystemUserMeQueryValidator>();
            services.AddScoped<IRequestValidator<GetSystemUserByIdQuery>, GetSystemUserByIdQueryValidator>();
            services.AddScoped<IRequestValidator<ListSystemUsersQuery>, ListSystemUsersQueryValidator>();
            services.AddScoped<IRequestValidator<GetWorkerPersonalizedNotificationPreviewQuery>, GetWorkerPersonalizedNotificationPreviewQueryValidator>();
            services.AddScoped<IRequestValidator<GetWorkerProfilePhotoViewUrlQuery>, GetWorkerProfilePhotoViewUrlQueryValidator>();
            services.AddScoped<IRequestValidator<GetWorkerLiveStatusFeedQuery>, GetWorkerLiveStatusFeedQueryValidator>();
            services.AddScoped<IRequestValidator<GetWorkerByIdQuery>, GetWorkerByIdQueryValidator>();
            services.AddScoped<IRequestValidator<ListGlobalSkillsQuery>, ListGlobalSkillsQueryValidator>();
            services.AddScoped<IRequestValidator<GetWorkerSelfDetailQuery>, GetWorkerSelfDetailQueryValidator>();
            services.AddScoped<IRequestValidator<GetWorkerSelfFullDetailQuery>, GetWorkerSelfFullDetailQueryValidator>();
            services.AddScoped<IRequestValidator<GetWorkerActiveCvUploadSessionQuery>, GetWorkerActiveCvUploadSessionQueryValidator>();
            services.AddScoped<IRequestValidator<GetWorkerDetailQuery>, GetWorkerDetailQueryValidator>();
            services.AddScoped<IRequestValidator<ListWorkersQuery>, ListWorkersQueryValidator>();
            services.AddScoped<IRequestValidator<SemanticSearchWorkersQuery>, SemanticSearchWorkersQueryValidator>();
            services.AddScoped<IRequestValidator<ListJobApplicationsByJobPostingIdQuery>, ListJobApplicationsByJobPostingIdQueryValidator>();
            services.AddScoped<IRequestValidator<ListMyJobApplicationsQuery>, ListMyJobApplicationsQueryValidator>();
            services.AddScoped<IRequestValidator<ListMyShiftAssignmentsQuery>, ListMyShiftAssignmentsQueryValidator>();
            services.AddScoped<IRequestValidator<ListEmployerShiftAssignmentsQuery>, ListEmployerShiftAssignmentsQueryValidator>();
            services.AddScoped<IRequestValidator<ListShiftAssignmentsHistoryQuery>, ListShiftAssignmentsHistoryQueryValidator>();
            services.AddScoped<IRequestValidator<ListJobPostingsByEmployerIdQuery>, ListJobPostingsByEmployerIdQueryValidator>();
            services.AddScoped<IRequestValidator<ListOpenJobPostingsQuery>, ListOpenJobPostingsQueryValidator>();
            services.AddScoped<IRequestValidator<ListSemanticMatchedJobPostingsQuery>, ListSemanticMatchedJobPostingsQueryValidator>();
            services.AddScoped<IRequestValidator<LoginSystemUserCommand>, LoginSystemUserCommandValidator>();
            services.AddScoped<IRequestValidator<MarkAllNotificationsAsReadCommand>, MarkAllNotificationsAsReadCommandValidator>();
            services.AddScoped<IRequestValidator<LogoutSystemUserCommand>, LogoutSystemUserCommandValidator>();
            services.AddScoped<IRequestValidator<MarkNotificationAsReadCommand>, MarkNotificationAsReadCommandValidator>();
            services.AddScoped<IRequestValidator<PublishJobPostingCommand>, PublishJobPostingCommandValidator>();
            services.AddScoped<IRequestValidator<ReactivateSystemUserCommand>, ReactivateSystemUserCommandValidator>();
            services.AddScoped<IRequestValidator<RegisterAdminCommand>, RegisterAdminCommandValidator>();
            services.AddScoped<IRequestValidator<RegisterEmployerCommand>, RegisterEmployerCommandValidator>();
            services.AddScoped<IRequestValidator<RegisterWorkerCommand>, RegisterWorkerCommandValidator>();
            services.AddScoped<IRequestValidator<RefreshSystemUserTokenCommand>, RefreshSystemUserTokenCommandValidator>();
            services.AddScoped<IRequestValidator<RejectJobPostingApplicationCommand>, RejectJobPostingApplicationCommandValidator>();
            services.AddScoped<IRequestValidator<RecordEmployerWorkerProfileViewCommand>, RecordEmployerWorkerProfileViewCommandValidator>();
            services.AddScoped<IRequestValidator<RemoveEmployerSupervisorCommand>, RemoveEmployerSupervisorCommandValidator>();
            services.AddScoped<IRequestValidator<RemoveWorkerAvailabilityCommand>, RemoveWorkerAvailabilityCommandValidator>();
            services.AddScoped<IRequestValidator<RemoveWorkerCertificateCommand>, RemoveWorkerCertificateCommandValidator>();
            services.AddScoped<IRequestValidator<RemoveWorkerEducationCommand>, RemoveWorkerEducationCommandValidator>();
            services.AddScoped<IRequestValidator<RemoveWorkerExperienceCommand>, RemoveWorkerExperienceCommandValidator>();
            services.AddScoped<IRequestValidator<RemoveWorkerLanguageCommand>, RemoveWorkerLanguageCommandValidator>();
            services.AddScoped<IRequestValidator<RemoveJobPostingSkillCommand>, RemoveJobPostingSkillCommandValidator>();
            services.AddScoped<IRequestValidator<RemoveWorkerReferenceCommand>, RemoveWorkerReferenceCommandValidator>();
            services.AddScoped<IRequestValidator<RemoveWorkerSkillCommand>, RemoveWorkerSkillCommandValidator>();
            services.AddScoped<IRequestValidator<DiscardWorkerCvReviewCommand>, DiscardWorkerCvReviewCommandValidator>();
            services.AddScoped<IRequestValidator<RequestSystemUserEmailVerificationCommand>, RequestSystemUserEmailVerificationCommandValidator>();
            services.AddScoped<IRequestValidator<SuspendEmployerCommand>, SuspendEmployerCommandValidator>();
            services.AddScoped<IRequestValidator<SuspendSystemUserCommand>, SuspendSystemUserCommandValidator>();
            services.AddScoped<IRequestValidator<SubmitJobPostingApplicationCommand>, SubmitJobPostingApplicationCommandValidator>();
            services.AddScoped<IRequestValidator<UpdateWorkerMatchingPreferencesCommand>, UpdateWorkerMatchingPreferencesCommandValidator>();
            services.AddScoped<IRequestValidator<UpdateWorkerBioCommand>, UpdateWorkerBioCommandValidator>();
            services.AddScoped<IRequestValidator<UpdateEmployerSocialLinksCommand>, UpdateEmployerSocialLinksCommandValidator>();
            services.AddScoped<IRequestValidator<UpdateEmployerProfileCommand>, UpdateEmployerProfileCommandValidator>();
            services.AddScoped<IRequestValidator<UpdateEmployerLocationCommand>, UpdateEmployerLocationCommandValidator>();
            services.AddScoped<IRequestValidator<UpdateWorkerSocialLinksCommand>, UpdateWorkerSocialLinksCommandValidator>();
            services.AddScoped<IRequestValidator<UpdateWorkerProfileCommand>, UpdateWorkerProfileCommandValidator>();
            services.AddScoped<IRequestValidator<UpdateWorkerCvTemplatePreferenceCommand>, UpdateWorkerCvTemplatePreferenceCommandValidator>();
            services.AddScoped<IRequestValidator<SendWorkerNotificationCommand>, SendWorkerNotificationCommandValidator>();
            services.AddScoped<IRequestValidator<SendSystemUserNotificationCommand>, SendSystemUserNotificationCommandValidator>();
            services.AddScoped<IRequestValidator<UpdateJobPostingCommand>, UpdateJobPostingCommandValidator>();
            services.AddScoped<IRequestValidator<VerifySystemUserEmailCommand>, VerifySystemUserEmailCommandValidator>();
            services.AddScoped<IRequestValidator<WithdrawJobPostingApplicationCommand>, WithdrawJobPostingApplicationCommandValidator>();
        }

        #endregion Methods
    }
}
