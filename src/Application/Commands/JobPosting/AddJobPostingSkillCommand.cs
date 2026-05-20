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
    /// Adds a skill requirement to a job posting.
    /// Handler result is the persisted <see cref="JobPostingSkill"/> primary key.
    /// </summary>
    public class AddJobPostingSkillCommand :
        CommandBase<int>
    {
        #region Properties
        /// <summary>
        /// When true, the skill is mandatory for applicants.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Identifier of the job posting receiving the skill.
        /// </summary>
        public int JobPostingId { get; set; }

        /// <summary>
        /// Raw skill tag text to normalize and attach.
        /// </summary>
        public string Tag { get; set; }
        #endregion Properties
    }

    internal class AddJobPostingSkillCommandValidator : IRequestValidator<AddJobPostingSkillCommand>
    {
        #region Methods

        /// <summary>
        /// Validates identifiers and tag text on the command.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(AddJobPostingSkillCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.JobPostingId <= 0)
            {
                failures.Add(ApplicationValidationCodes.AddJobPostingSkillJobPostingId.ForField(nameof(request.JobPostingId)));
            }

            if (string.IsNullOrWhiteSpace(request.Tag))
            {
                failures.Add(ApplicationValidationCodes.AddJobPostingSkillTag.ForField(nameof(request.Tag)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class AddJobPostingSkillCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<AddJobPostingSkillCommand, int>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the posting, adds the skill, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The persisted job posting skill identifier.</returns>
        protected override async Task<int> HandleAsync(AddJobPostingSkillCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            JobPosting? entity = await UnitOfWork.GetRepository<JobPosting>().GetByIdAsync(command.JobPostingId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (entity.EmployerId == employerId).ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            JobPostingSkill skill = entity.AddSkill(command.Tag, command.IsRequired);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await AdaIsReadModelCacheInvalidation.InvalidateJobPostingReadModelsAsync(
                CacheService,
                command.JobPostingId,
                entity.EmployerId,
                cancellationToken,
                includeApplicationScopes: false);
            await AdaIsReadModelCacheInvalidation.InvalidateSkillAndEmbeddingListCachesAsync(
                CacheService,
                cancellationToken);

            return skill.Id;
        }

        #endregion Utils
    }
}
