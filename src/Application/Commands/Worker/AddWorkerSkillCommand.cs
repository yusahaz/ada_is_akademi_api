namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using System;

    /// <summary>
    /// Adds a normalized skill tag to a worker profile.
    /// Handler result is the persisted <see cref="WorkerSkill"/> primary key.
    /// </summary>
    public class AddWorkerSkillCommand :
        CommandBase<int>
    {
        #region Properties
        /// <summary>
        /// Normalized skill label or keyword to attach.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Identifier of the worker receiving the skill.
        /// </summary>
        public int WorkerId { get; set; }
        #endregion Properties
    }

    internal class AddWorkerSkillCommandValidator : IRequestValidator<AddWorkerSkillCommand>
    {
        #region Methods

        /// <summary>
        /// Validates identifiers and tag text on the command.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(AddWorkerSkillCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.WorkerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.AddWorkerSkillWorkerId.ForField(nameof(request.WorkerId)));
            }

            if (string.IsNullOrWhiteSpace(request.Tag))
            {
                failures.Add(ApplicationValidationCodes.AddWorkerSkillTagRequired.ForField(nameof(request.Tag)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class AddWorkerSkillCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<AddWorkerSkillCommand, int>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the worker, adds the skill, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The persisted worker skill identifier.</returns>
        protected override async Task<int> HandleAsync(AddWorkerSkillCommand command, CancellationToken cancellationToken)
        {
            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .GetByIdAsync(command.WorkerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            WorkerSkill skill = worker.AddSkill(command.Tag);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return skill.Id;
        }

        #endregion Utils
    }
}
