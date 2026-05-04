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
    /// Activates a system user group for authorization evaluations.
    /// </summary>
    public class ActivateSystemUserGroupCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Identifier of the group to activate.
        /// </summary>
        public int SystemUserGroupId { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Validates <see cref="ActivateSystemUserGroupCommand"/> payloads before handling.
    /// </summary>
    internal class ActivateSystemUserGroupCommandValidator :
        IRequestValidator<ActivateSystemUserGroupCommand>
    {
        #region Methods

        /// <summary>
        /// Ensures the group identifier is positive.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(ActivateSystemUserGroupCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.SystemUserGroupId <= 0)
            {
                failures.Add(ApplicationValidationCodes.ActivateSystemUserGroupSystemUserGroupId.ForField(nameof(request.SystemUserGroupId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    /// <summary>
    /// Handles <see cref="ActivateSystemUserGroupCommand"/> by loading the group, activating it, and saving changes.
    /// </summary>
    internal class ActivateSystemUserGroupCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<ActivateSystemUserGroupCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the group, activates it, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(ActivateSystemUserGroupCommand command, CancellationToken cancellationToken)
        {
            SystemUserGroup? entity = await UnitOfWork.GetRepository<SystemUserGroup>().GetByIdAsync(command.SystemUserGroupId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            entity.Activate();

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
