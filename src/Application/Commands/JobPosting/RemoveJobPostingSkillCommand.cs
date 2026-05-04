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
    /// Removes a skill requirement from a job posting.
    /// </summary>
    public class RemoveJobPostingSkillCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Identifier of the job posting owning the skill.
        /// </summary>
        public int JobPostingId { get; set; }

        /// <summary>
        /// Identifier of the skill row to remove.
        /// </summary>
        public int SkillId { get; set; }
        #endregion Properties
    }

    internal class RemoveJobPostingSkillCommandValidator : IRequestValidator<RemoveJobPostingSkillCommand>
    {
        #region Methods

        /// <summary>
        /// Validates posting and skill identifiers on the command.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(RemoveJobPostingSkillCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.JobPostingId <= 0)
            {
                failures.Add(ApplicationValidationCodes.RemoveJobPostingSkillJobPostingId.ForField(nameof(request.JobPostingId)));
            }

            if (request.SkillId <= 0)
            {
                failures.Add(ApplicationValidationCodes.RemoveJobPostingSkillSkillId.ForField(nameof(request.SkillId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class RemoveJobPostingSkillCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<RemoveJobPostingSkillCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the posting, removes the skill, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(RemoveJobPostingSkillCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            JobPosting? entity = await UnitOfWork.GetRepository<JobPosting>().GetByIdAsync(command.JobPostingId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (entity.EmployerId == employerId).ThrowIfFalse(AzoxiaErrorCodes.NotFound);

            entity.RemoveSkill(command.SkillId);

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
