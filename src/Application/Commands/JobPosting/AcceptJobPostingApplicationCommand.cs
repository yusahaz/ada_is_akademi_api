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
    /// Accepts a worker application for a job posting when capacity allows.
    /// </summary>
    public class AcceptJobPostingApplicationCommand : CommandBase
    {
        #region Properties

        /// <summary>
        /// Identifier of the application to accept.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Identifier of the job posting receiving the decision.
        /// </summary>
        public int JobPostingId { get; set; }
        #endregion Properties
    }

    internal class AcceptJobPostingApplicationCommandValidator : IRequestValidator<AcceptJobPostingApplicationCommand>
    {
        #region Methods

        /// <summary>
        /// Validates application and posting identifiers on the command.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(AcceptJobPostingApplicationCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.ApplicationId <= 0)
            {
                failures.Add(ApplicationValidationCodes.AcceptJobPostingApplicationApplicationId.ForField(nameof(request.ApplicationId)));
            }

            if (request.JobPostingId <= 0)
            {
                failures.Add(ApplicationValidationCodes.AcceptJobPostingApplicationJobPostingId.ForField(nameof(request.JobPostingId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class AcceptJobPostingApplicationCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<AcceptJobPostingApplicationCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the posting, enforces employer ownership, delegates to <see cref="JobPosting.AcceptApplication"/>, then saves and invalidates posting cache.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(AcceptJobPostingApplicationCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            JobPosting? entity = await UnitOfWork
                .GetRepository<JobPosting>()
                .Filter(x => x.Id == command.JobPostingId)
                .Include(x => x.Applications)
                .FirstOrDefaultAsync(cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            (entity.EmployerId == employerId).ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            entity.AcceptApplication(command.ApplicationId);

            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await AdaIsReadModelCacheInvalidation.InvalidateJobPostingReadModelsAsync(
                CacheService,
                entity.Id,
                entity.EmployerId,
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
