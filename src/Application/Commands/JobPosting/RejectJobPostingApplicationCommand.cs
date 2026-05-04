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
    /// Rejects a worker application for a job posting.
    /// </summary>
    public class RejectJobPostingApplicationCommand : CommandBase
    {
        #region Properties

        /// <summary>
        /// Identifier of the application to reject.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Identifier of the job posting receiving the decision.
        /// </summary>
        public int JobPostingId { get; set; }

        /// <summary>
        /// Optional human-readable reason for rejection.
        /// </summary>
        public string? Reason { get; set; }
        #endregion Properties
    }

    internal class RejectJobPostingApplicationCommandValidator : IRequestValidator<RejectJobPostingApplicationCommand>
    {
        #region Methods

        /// <summary>
        /// Validates application and posting identifiers on the command.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(RejectJobPostingApplicationCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.ApplicationId <= 0)
            {
                failures.Add(ApplicationValidationCodes.RejectJobPostingApplicationApplicationId.ForField(nameof(request.ApplicationId)));
            }

            if (request.JobPostingId <= 0)
            {
                failures.Add(ApplicationValidationCodes.RejectJobPostingApplicationJobPostingId.ForField(nameof(request.JobPostingId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class RejectJobPostingApplicationCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<RejectJobPostingApplicationCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the posting, enforces employer ownership, delegates to <see cref="JobPosting.RejectApplication"/>, then saves and invalidates posting cache.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(RejectJobPostingApplicationCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            JobPosting? entity = await UnitOfWork.GetRepository<JobPosting>().GetByIdAsync(command.JobPostingId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            (entity.EmployerId == employerId).ThrowIfFalse(AzoxiaErrorCodes.NotFound);

            entity.RejectApplication(command.ApplicationId, command.Reason);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.JobPostingDependency(command.JobPostingId),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.EmployerJobPostingsSummaryDependency(entity.EmployerId),
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
