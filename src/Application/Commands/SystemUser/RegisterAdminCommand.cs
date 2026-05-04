namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Registers an admin user and attaches it to the default admin group.
    /// </summary>
    public class RegisterAdminCommand :
        CommandBase<int>
    {
        #region Properties

        /// <summary>
        /// User email used as login identifier.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Optional given name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Optional family name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Password text for account bootstrap.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Optional phone contact.
        /// </summary>
        public string? Phone { get; set; }

        #endregion Properties
    }

    internal class RegisterAdminCommandValidator : IRequestValidator<RegisterAdminCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(RegisterAdminCommand request)
        {
            List<ValidationFailure> failures = [];

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                failures.Add(ApplicationValidationCodes.RegisterSystemUserEmailRequired.ForField(nameof(request.Email)));
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                failures.Add(ApplicationValidationCodes.RegisterSystemUserPasswordRequired.ForField(nameof(request.Password)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class RegisterAdminCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<RegisterAdminCommand, int>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<int> HandleAsync(RegisterAdminCommand command, CancellationToken cancellationToken)
        {
            bool emailExists = await UnitOfWork
                .GetRepository<SystemUser>()
                .AnyAsync(x => x.Email == command.Email, cancellationToken);

            if (emailExists)
            {
                ApplicationValidationCodes.RegisterSystemUserEmailAlreadyExists.Throw();
            }

            SystemUser user = new(command.Email, command.Password, SystemUserType.Admin);
            user.Update(command.FirstName, command.LastName, command.Phone);
            UnitOfWork.Add(user);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            SystemUserGroup? adminGroup = await UnitOfWork
                .GetRepository<SystemUserGroup>()
                .Filter(x => x.Name == "Default Admin")
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (adminGroup is not null)
            {
                bool membershipExists = await UnitOfWork
                    .GetRepository<SystemUserGroupMembership>()
                    .AnyAsync(
                        x => x.SystemUserGroupId == adminGroup.Id
                            && x.SystemUserId == user.Id
                            && x.ScopeType == MembershipScopeType.Global
                            && x.ScopeId == null,
                        cancellationToken);

                if (!membershipExists)
                {
                    SystemUserGroupMembership membership = new(adminGroup.Id, user.Id);
                    UnitOfWork.Add(membership);
                    await UnitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.SystemUserAllDependency(),
                cancellationToken);

            return user.Id;
        }

        #endregion Utils
    }
}
