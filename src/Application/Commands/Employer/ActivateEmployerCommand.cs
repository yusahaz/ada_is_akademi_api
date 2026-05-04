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
    /// Activates an employer that is not banned.
    /// </summary>
    public class ActivateEmployerCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Identifier of the employer to activate.
        /// </summary>
        public int EmployerId { get; set; }
        #endregion Properties
    }

    internal class ActivateEmployerCommandValidator : IRequestValidator<ActivateEmployerCommand>
    {
        #region Methods

        /// <summary>
        /// Ensures the employer identifier is positive.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(ActivateEmployerCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.ActivateEmployerEmployerId.ForField(nameof(request.EmployerId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ActivateEmployerCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<ActivateEmployerCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the employer, activates it, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(ActivateEmployerCommand command, CancellationToken cancellationToken)
        {
            Employer? entity = await UnitOfWork.GetRepository<Employer>().GetByIdAsync(command.EmployerId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            entity.SetAsActive();

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.EmployerDependency(command.EmployerId),
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
