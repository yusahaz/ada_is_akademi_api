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
    /// Bans a system user account.
    /// </summary>
    public class BanSystemUserCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Identifier of the user to ban.
        /// </summary>
        public int SystemUserId { get; set; }
        #endregion Properties
    }

    internal class BanSystemUserCommandValidator : IRequestValidator<BanSystemUserCommand>
    {
        #region Methods

        /// <summary>
        /// Ensures the user identifier is positive.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(BanSystemUserCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.SystemUserId <= 0)
            {
                failures.Add(ApplicationValidationCodes.BanSystemUserSystemUserId.ForField(nameof(request.SystemUserId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class BanSystemUserCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<BanSystemUserCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the user, bans the account, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(BanSystemUserCommand command, CancellationToken cancellationToken)
        {
            SystemUser? entity = await UnitOfWork.GetRepository<SystemUser>().GetByIdAsync(command.SystemUserId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            entity.Ban();

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
