namespace Azoxia.AdaIsAkademi.Application.DependencyInjection
{
    using Azoxia.AdaIsAkademi.Application;
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
            services.AddScoped<IRequestHandler<ActivateSystemUserGroupCommand, Unit>, ActivateSystemUserGroupCommandHandler>();
            services.AddScoped<IRequestHandler<AddJobPostingSkillCommand, int>, AddJobPostingSkillCommandHandler>();
            services.AddScoped<IRequestHandler<AddSystemUserGroupPermissionCommand, int>, AddSystemUserGroupPermissionCommandHandler>();
            services.AddScoped<IRequestHandler<AddWorkerSkillCommand, int>, AddWorkerSkillCommandHandler>();
            services.AddScoped<IRequestHandler<BanEmployerCommand, Unit>, BanEmployerCommandHandler>();
            services.AddScoped<IRequestHandler<GenerateCommissionReceivableCommand, int>, GenerateCommissionReceivableCommandHandler>();
            services.AddScoped<IRequestHandler<SetEmployerCommissionRateCommand, Unit>, SetEmployerCommissionRateCommandHandler>();
            services.AddScoped<IRequestHandler<BanSystemUserCommand, Unit>, BanSystemUserCommandHandler>();
            services.AddScoped<IRequestHandler<RunOverdueAlarmSweepCommand, int>, RunOverdueAlarmSweepCommandHandler>();
            services.AddScoped<IRequestHandler<CancelJobPostingCommand, Unit>, CancelJobPostingCommandHandler>();
            services.AddScoped<IRequestHandler<ChangeSystemUserPasswordCommand, Unit>, ChangeSystemUserPasswordCommandHandler>();
            services.AddScoped<IRequestHandler<CheckInShiftAssignmentCommand, Unit>, CheckInShiftAssignmentCommandHandler>();
            services.AddScoped<IRequestHandler<CompleteJobPostingCommand, Unit>, CompleteJobPostingCommandHandler>();
            services.AddScoped<IRequestHandler<CreateShiftAssignmentCommand, int>, CreateShiftAssignmentCommandHandler>();
            services.AddScoped<IRequestHandler<CreateJobPostingCommand, int>, CreateJobPostingCommandHandler>();
            services.AddScoped<IRequestHandler<DeactivateSystemUserGroupCommand, Unit>, DeactivateSystemUserGroupCommandHandler>();
            services.AddScoped<IRequestHandler<PublishJobPostingCommand, Unit>, PublishJobPostingCommandHandler>();
            services.AddScoped<IRequestHandler<ReactivateSystemUserCommand, Unit>, ReactivateSystemUserCommandHandler>();
            services.AddScoped<IRequestHandler<RegisterAdminCommand, int>, RegisterAdminCommandHandler>();
            services.AddScoped<IRequestHandler<RegisterEmployerCommand, int>, RegisterEmployerCommandHandler>();
            services.AddScoped<IRequestHandler<RegisterWorkerCommand, int>, RegisterWorkerCommandHandler>();
            services.AddScoped<IRequestHandler<RefreshSystemUserTokenCommand, SystemUserTokenModel>, RefreshSystemUserTokenCommandHandler>();
            services.AddScoped<IRequestHandler<RejectJobPostingApplicationCommand, Unit>, RejectJobPostingApplicationCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveJobPostingSkillCommand, Unit>, RemoveJobPostingSkillCommandHandler>();
            services.AddScoped<IRequestHandler<RequestSystemUserEmailVerificationCommand, Unit>, RequestSystemUserEmailVerificationCommandHandler>();
            services.AddScoped<IRequestHandler<LoginSystemUserCommand, SystemUserTokenModel>, LoginSystemUserCommandHandler>();
            services.AddScoped<IRequestHandler<LogoutSystemUserCommand, Unit>, LogoutSystemUserCommandHandler>();
            services.AddScoped<IRequestHandler<SuspendEmployerCommand, Unit>, SuspendEmployerCommandHandler>();
            services.AddScoped<IRequestHandler<SuspendSystemUserCommand, Unit>, SuspendSystemUserCommandHandler>();
            services.AddScoped<IRequestHandler<SubmitJobPostingApplicationCommand, int>, SubmitJobPostingApplicationCommandHandler>();
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
            services.AddScoped<IRequestHandler<GetCommissionReceivableByPeriodQuery, CommissionReceivableDetailModel>, GetCommissionReceivableByPeriodQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployerCommissionEstimateQuery, EmployerCommissionEstimateModel>, GetEmployerCommissionEstimateQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployerCommissionPolicyQuery, EmployerCommissionPolicyModel>, GetEmployerCommissionPolicyQueryHandler>();
            services.AddScoped<IRequestHandler<ListEmployerCommissionSummariesQuery, IReadOnlyList<EmployerCommissionListItemModel>>, ListEmployerCommissionSummariesQueryHandler>();
            services.AddScoped<IRequestHandler<ExportEmployerCommissionPoliciesCsvQuery, EmployerCommissionPolicyExportPackageModel>, ExportEmployerCommissionPoliciesCsvQueryHandler>();
            services.AddScoped<IRequestHandler<GetDashboardStatisticsQuery, DashboardStatisticsModel>, GetDashboardStatisticsQueryHandler>();
            services.AddScoped<IRequestHandler<ExportOverdueAlarmsCsvQuery, OverdueAlarmExportPackageModel>, ExportOverdueAlarmsCsvQueryHandler>();
            services.AddScoped<IRequestHandler<GetOverdueJobSummaryQuery, OverdueJobSummaryModel>, GetOverdueJobSummaryQueryHandler>();
            services.AddScoped<IRequestHandler<GetMonetizationSummaryQuery, MonetizationSummaryModel>, GetMonetizationSummaryQueryHandler>();
            services.AddScoped<IRequestHandler<GetJobPostingByIdQuery, JobPostingDetailModel>, GetJobPostingByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetSystemUserMeQuery, SystemUserMeModel>, GetSystemUserMeQueryHandler>();
            services.AddScoped<IRequestHandler<GetWorkerPersonalizedNotificationPreviewQuery, WorkerNotificationPreviewModel>, GetWorkerPersonalizedNotificationPreviewQueryHandler>();
            services.AddScoped<IRequestHandler<GetWorkerByIdQuery, WorkerDetailModel>, GetWorkerByIdQueryHandler>();
            services.AddScoped<IRequestHandler<ListJobApplicationsByJobPostingIdQuery, IReadOnlyList<JobApplicationListItemModel>>, ListJobApplicationsByJobPostingIdQueryHandler>();
            services.AddScoped<IRequestHandler<ListJobPostingsByEmployerIdQuery, IReadOnlyList<JobPostingSummaryModel>>, ListJobPostingsByEmployerIdQueryHandler>();
            services.AddScoped<IRequestHandler<ListOpenJobPostingsQuery, IReadOnlyList<JobPostingSummaryModel>>, ListOpenJobPostingsQueryHandler>();
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
            services.AddScoped<IRequestValidator<ActivateSystemUserGroupCommand>, ActivateSystemUserGroupCommandValidator>();
            services.AddScoped<IRequestValidator<AddJobPostingSkillCommand>, AddJobPostingSkillCommandValidator>();
            services.AddScoped<IRequestValidator<AddSystemUserGroupPermissionCommand>, AddSystemUserGroupPermissionCommandValidator>();
            services.AddScoped<IRequestValidator<AddWorkerSkillCommand>, AddWorkerSkillCommandValidator>();
            services.AddScoped<IRequestValidator<BanEmployerCommand>, BanEmployerCommandValidator>();
            services.AddScoped<IRequestValidator<GenerateCommissionReceivableCommand>, GenerateCommissionReceivableCommandValidator>();
            services.AddScoped<IRequestValidator<SetEmployerCommissionRateCommand>, SetEmployerCommissionRateCommandValidator>();
            services.AddScoped<IRequestValidator<BanSystemUserCommand>, BanSystemUserCommandValidator>();
            services.AddScoped<IRequestValidator<RunOverdueAlarmSweepCommand>, RunOverdueAlarmSweepCommandValidator>();
            services.AddScoped<IRequestValidator<CancelJobPostingCommand>, CancelJobPostingCommandValidator>();
            services.AddScoped<IRequestValidator<ChangeSystemUserPasswordCommand>, ChangeSystemUserPasswordCommandValidator>();
            services.AddScoped<IRequestValidator<CheckInShiftAssignmentCommand>, CheckInShiftAssignmentCommandValidator>();
            services.AddScoped<IRequestValidator<CompleteJobPostingCommand>, CompleteJobPostingCommandValidator>();
            services.AddScoped<IRequestValidator<CreateShiftAssignmentCommand>, CreateShiftAssignmentCommandValidator>();
            services.AddScoped<IRequestValidator<CreateJobPostingCommand>, CreateJobPostingCommandValidator>();
            services.AddScoped<IRequestValidator<DeactivateSystemUserGroupCommand>, DeactivateSystemUserGroupCommandValidator>();
            services.AddScoped<IRequestValidator<GetEmployerByIdQuery>, GetEmployerByIdQueryValidator>();
            services.AddScoped<IRequestValidator<GetCommissionReceivableByPeriodQuery>, GetCommissionReceivableByPeriodQueryValidator>();
            services.AddScoped<IRequestValidator<GetEmployerCommissionEstimateQuery>, GetEmployerCommissionEstimateQueryValidator>();
            services.AddScoped<IRequestValidator<GetEmployerCommissionPolicyQuery>, GetEmployerCommissionPolicyQueryValidator>();
            services.AddScoped<IRequestValidator<ListEmployerCommissionSummariesQuery>, ListEmployerCommissionSummariesQueryValidator>();
            services.AddScoped<IRequestValidator<ExportEmployerCommissionPoliciesCsvQuery>, ExportEmployerCommissionPoliciesCsvQueryValidator>();
            services.AddScoped<IRequestValidator<GetDashboardStatisticsQuery>, GetDashboardStatisticsQueryValidator>();
            services.AddScoped<IRequestValidator<ExportOverdueAlarmsCsvQuery>, ExportOverdueAlarmsCsvQueryValidator>();
            services.AddScoped<IRequestValidator<GetOverdueJobSummaryQuery>, GetOverdueJobSummaryQueryValidator>();
            services.AddScoped<IRequestValidator<GetMonetizationSummaryQuery>, GetMonetizationSummaryQueryValidator>();
            services.AddScoped<IRequestValidator<GetJobPostingByIdQuery>, GetJobPostingByIdQueryValidator>();
            services.AddScoped<IRequestValidator<GetSystemUserMeQuery>, GetSystemUserMeQueryValidator>();
            services.AddScoped<IRequestValidator<GetWorkerPersonalizedNotificationPreviewQuery>, GetWorkerPersonalizedNotificationPreviewQueryValidator>();
            services.AddScoped<IRequestValidator<GetWorkerByIdQuery>, GetWorkerByIdQueryValidator>();
            services.AddScoped<IRequestValidator<ListJobApplicationsByJobPostingIdQuery>, ListJobApplicationsByJobPostingIdQueryValidator>();
            services.AddScoped<IRequestValidator<ListJobPostingsByEmployerIdQuery>, ListJobPostingsByEmployerIdQueryValidator>();
            services.AddScoped<IRequestValidator<ListOpenJobPostingsQuery>, ListOpenJobPostingsQueryValidator>();
            services.AddScoped<IRequestValidator<ListSemanticMatchedJobPostingsQuery>, ListSemanticMatchedJobPostingsQueryValidator>();
            services.AddScoped<IRequestValidator<LoginSystemUserCommand>, LoginSystemUserCommandValidator>();
            services.AddScoped<IRequestValidator<LogoutSystemUserCommand>, LogoutSystemUserCommandValidator>();
            services.AddScoped<IRequestValidator<PublishJobPostingCommand>, PublishJobPostingCommandValidator>();
            services.AddScoped<IRequestValidator<ReactivateSystemUserCommand>, ReactivateSystemUserCommandValidator>();
            services.AddScoped<IRequestValidator<RegisterAdminCommand>, RegisterAdminCommandValidator>();
            services.AddScoped<IRequestValidator<RegisterEmployerCommand>, RegisterEmployerCommandValidator>();
            services.AddScoped<IRequestValidator<RegisterWorkerCommand>, RegisterWorkerCommandValidator>();
            services.AddScoped<IRequestValidator<RefreshSystemUserTokenCommand>, RefreshSystemUserTokenCommandValidator>();
            services.AddScoped<IRequestValidator<RejectJobPostingApplicationCommand>, RejectJobPostingApplicationCommandValidator>();
            services.AddScoped<IRequestValidator<RemoveJobPostingSkillCommand>, RemoveJobPostingSkillCommandValidator>();
            services.AddScoped<IRequestValidator<RequestSystemUserEmailVerificationCommand>, RequestSystemUserEmailVerificationCommandValidator>();
            services.AddScoped<IRequestValidator<SuspendEmployerCommand>, SuspendEmployerCommandValidator>();
            services.AddScoped<IRequestValidator<SuspendSystemUserCommand>, SuspendSystemUserCommandValidator>();
            services.AddScoped<IRequestValidator<SubmitJobPostingApplicationCommand>, SubmitJobPostingApplicationCommandValidator>();
            services.AddScoped<IRequestValidator<UpdateJobPostingCommand>, UpdateJobPostingCommandValidator>();
            services.AddScoped<IRequestValidator<VerifySystemUserEmailCommand>, VerifySystemUserEmailCommandValidator>();
            services.AddScoped<IRequestValidator<WithdrawJobPostingApplicationCommand>, WithdrawJobPostingApplicationCommandValidator>();
        }

        #endregion Methods
    }
}
