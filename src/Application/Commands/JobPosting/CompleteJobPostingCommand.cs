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
    /// Marks a job posting as completed after staffing.
    /// </summary>
    public class CompleteJobPostingCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Identifier of the job posting to complete.
        /// </summary>
        public int JobPostingId { get; set; }
        #endregion Properties
    }

    internal class CompleteJobPostingCommandValidator : IRequestValidator<CompleteJobPostingCommand>
    {
        #region Methods

        /// <summary>
        /// Ensures the job posting identifier is positive.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(CompleteJobPostingCommand request)
        {
            return new ValidationResult(
                request.JobPostingId <= 0
                    ? [ApplicationValidationCodes.CompleteJobPostingJobPostingId.ForField(nameof(request.JobPostingId))]
                    : []);
        }

        #endregion Methods
    }

    internal class CompleteJobPostingCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<CompleteJobPostingCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the posting, completes it, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(CompleteJobPostingCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            JobPosting? entity = await UnitOfWork.GetRepository<JobPosting>().GetByIdAsync(command.JobPostingId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (entity.EmployerId == employerId).ThrowIfFalse(AzoxiaErrorCodes.NotFound);

            entity.Complete();

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
