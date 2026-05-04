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
    /// Adds or updates an allow/deny permission on a system user group.
    /// Handler result is the persisted <see cref="SystemUserGroupPermission"/> primary key.
    /// </summary>
    public class AddSystemUserGroupPermissionCommand :
        CommandBase<int>
    {
        #region Properties
        /// <summary>
        /// Effect to apply for the permission on this group.
        /// </summary>
        public PermissionEffect Effect { get; set; }

        /// <summary>
        /// Identifier of the permission being granted or denied.
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// Identifier of the group receiving the rule.
        /// </summary>
        public int SystemUserGroupId { get; set; }
        #endregion Properties
    }

    internal class AddSystemUserGroupPermissionCommandValidator : IRequestValidator<AddSystemUserGroupPermissionCommand>
    {
        #region Methods

        /// <summary>
        /// Validates group and permission identifiers on the command.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(AddSystemUserGroupPermissionCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.PermissionId <= 0)
            {
                failures.Add(ApplicationValidationCodes.AddSystemUserGroupPermissionPermissionId.ForField(nameof(request.PermissionId)));
            }

            if (request.SystemUserGroupId <= 0)
            {
                failures.Add(ApplicationValidationCodes.AddSystemUserGroupPermissionSystemUserGroupId.ForField(nameof(request.SystemUserGroupId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class AddSystemUserGroupPermissionCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<AddSystemUserGroupPermissionCommand, int>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the group, merges the permission rule, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The persisted group permission row identifier.</returns>
        protected override async Task<int> HandleAsync(AddSystemUserGroupPermissionCommand command, CancellationToken cancellationToken)
        {
            SystemUserGroup? entity = await UnitOfWork.GetRepository<SystemUserGroup>().GetByIdAsync(command.SystemUserGroupId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            SystemUserGroupPermission permission = entity.AddPermission(command.PermissionId, command.Effect);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return permission.Id;
        }

        #endregion Utils
    }
}
