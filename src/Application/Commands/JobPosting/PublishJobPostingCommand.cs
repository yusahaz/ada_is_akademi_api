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
    /// Publishes a draft job posting so it can accept applications.
    /// </summary>
    public class PublishJobPostingCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Identifier of the job posting to publish.
        /// </summary>
        public int JobPostingId { get; set; }
        #endregion Properties
    }

    internal class PublishJobPostingCommandValidator : IRequestValidator<PublishJobPostingCommand>
    {
        #region Methods

        /// <summary>
        /// Ensures the job posting identifier is positive.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(PublishJobPostingCommand request)
        {
            return new ValidationResult(
                request.JobPostingId <= 0
                    ? [ApplicationValidationCodes.PublishJobPostingJobPostingId.ForField(nameof(request.JobPostingId))]
                    : []);
        }

        #endregion Methods
    }

    internal class PublishJobPostingCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<PublishJobPostingCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the posting, publishes it, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(PublishJobPostingCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            JobPosting? entity = await UnitOfWork.GetRepository<JobPosting>().GetByIdAsync(command.JobPostingId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (entity.EmployerId == employerId).ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            entity.Publish();

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
