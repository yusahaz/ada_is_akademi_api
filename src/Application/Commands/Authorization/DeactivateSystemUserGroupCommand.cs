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
    /// Deactivates a system user group so it no longer participates in evaluations.
    /// </summary>
    public class DeactivateSystemUserGroupCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Identifier of the group to deactivate.
        /// </summary>
        public int SystemUserGroupId { get; set; }
        #endregion Properties
    }

    internal class DeactivateSystemUserGroupCommandValidator : IRequestValidator<DeactivateSystemUserGroupCommand>
    {
        #region Methods

        /// <summary>
        /// Ensures the group identifier is positive.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(DeactivateSystemUserGroupCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.SystemUserGroupId <= 0)
            {
                failures.Add(ApplicationValidationCodes.DeactivateSystemUserGroupSystemUserGroupId.ForField(nameof(request.SystemUserGroupId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class DeactivateSystemUserGroupCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<DeactivateSystemUserGroupCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the group, deactivates it, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(DeactivateSystemUserGroupCommand command, CancellationToken cancellationToken)
        {
            SystemUserGroup? entity = await UnitOfWork.GetRepository<SystemUserGroup>().GetByIdAsync(command.SystemUserGroupId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            entity.Deactivate();

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
