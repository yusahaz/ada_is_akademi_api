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
    /// Bans an employer organization.
    /// </summary>
    public class BanEmployerCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Identifier of the employer to ban.
        /// </summary>
        public int EmployerId { get; set; }
        #endregion Properties
    }

    internal class BanEmployerCommandValidator : IRequestValidator<BanEmployerCommand>
    {
        #region Methods

        /// <summary>
        /// Ensures the employer identifier is positive.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(BanEmployerCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.BanEmployerEmployerId.ForField(nameof(request.EmployerId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class BanEmployerCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<BanEmployerCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the employer, bans it, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(BanEmployerCommand command, CancellationToken cancellationToken)
        {
            Employer? entity = await UnitOfWork.GetRepository<Employer>().GetByIdAsync(command.EmployerId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            entity.SetAsBanned();

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await AdaIsReadModelCacheInvalidation.InvalidateEmployerReadModelsAsync(
                CacheService,
                command.EmployerId,
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
