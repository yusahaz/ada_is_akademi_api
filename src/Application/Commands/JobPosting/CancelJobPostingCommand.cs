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
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// Cancels a job posting that is not yet completed.
    /// </summary>
    public class CancelJobPostingCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Identifier of the job posting to cancel.
        /// </summary>
        public int JobPostingId { get; set; }
        #endregion Properties
    }

    internal class CancelJobPostingCommandValidator : IRequestValidator<CancelJobPostingCommand>
    {
        #region Methods

        /// <summary>
        /// Ensures the job posting identifier is positive.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(CancelJobPostingCommand request)
        {
            return new ValidationResult(
                request.JobPostingId <= 0
                    ? [ApplicationValidationCodes.CancelJobPostingJobPostingId.ForField(nameof(request.JobPostingId))]
                    : []);
        }

        #endregion Methods
    }

    internal class CancelJobPostingCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<CancelJobPostingCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the posting, cancels it, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(CancelJobPostingCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            JobPosting? entity = await UnitOfWork.GetRepository<JobPosting>().GetByIdAsync(command.JobPostingId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (entity.EmployerId == employerId).ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            entity.Cancel();

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.JobPostingDependency(command.JobPostingId),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.EmployerJobPostingsSummaryDependency(entity.EmployerId),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.JobPostingAllDependency(),
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
