namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using System;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Withdraws a pending application from a job posting.
    /// </summary>
    public class WithdrawJobPostingApplicationCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Application row to withdraw.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Owning job posting identifier.
        /// </summary>
        public int JobPostingId { get; set; }
        #endregion Properties
    }

    internal class WithdrawJobPostingApplicationCommandValidator : IRequestValidator<WithdrawJobPostingApplicationCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(WithdrawJobPostingApplicationCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.ApplicationId <= 0)
            {
                failures.Add(ApplicationValidationCodes.WithdrawJobPostingApplicationApplicationId.ForField(nameof(request.ApplicationId)));
            }

            if (request.JobPostingId <= 0)
            {
                failures.Add(ApplicationValidationCodes.WithdrawJobPostingApplicationJobPostingId.ForField(nameof(request.JobPostingId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class WithdrawJobPostingApplicationCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<WithdrawJobPostingApplicationCommand>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(
            WithdrawJobPostingApplicationCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            JobPosting? posting = await UnitOfWork
                .GetRepository<JobPosting>()
                .Filter(x => x.Id == command.JobPostingId)
                .Include(x => x.Applications)
                .FirstOrDefaultAsync(cancellationToken);
            posting = posting.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            JobApplication? application = posting.Applications.FirstOrDefault(x => x.Id == command.ApplicationId);
            application = application.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (application.WorkerId == workerId).ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            posting.WithdrawApplication(command.ApplicationId);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.JobPostingDependency(command.JobPostingId),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.EmployerJobPostingsSummaryDependency(posting.EmployerId),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.JobPostingAllDependency(),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.JobApplicationAllDependency(),
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
