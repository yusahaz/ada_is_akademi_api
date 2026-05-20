namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Sets monetization commission rate for an employer.
    /// </summary>
    public class SetEmployerCommissionRateCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Target employer identifier.
        /// </summary>
        public int EmployerId { get; set; }

        /// <summary>
        /// Commission rate in range 0..1.
        /// </summary>
        public decimal CommissionRate { get; set; }

        #endregion Properties
    }

    internal class SetEmployerCommissionRateCommandValidator : IRequestValidator<SetEmployerCommissionRateCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(SetEmployerCommissionRateCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.SetEmployerCommissionRateEmployerId.ForField(nameof(SetEmployerCommissionRateCommand.EmployerId)));
            }

            if (request.CommissionRate < 0m || request.CommissionRate > 1m)
            {
                failures.Add(ApplicationValidationCodes.SetEmployerCommissionRateCommissionRate.ForField(nameof(SetEmployerCommissionRateCommand.CommissionRate)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class SetEmployerCommissionRateCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<SetEmployerCommissionRateCommand>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(SetEmployerCommissionRateCommand command, CancellationToken cancellationToken)
        {
            Employer? employer = await UnitOfWork.GetRepository<Employer>().GetByIdAsync(command.EmployerId, cancellationToken);
            employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            employer.SetCommissionRate(command.CommissionRate);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await AdaIsReadModelCacheInvalidation.InvalidateEmployerReadModelsAsync(
                CacheService,
                command.EmployerId,
                cancellationToken);
            await AdaIsReadModelCacheInvalidation.InvalidateEmployerCommissionReadModelsAsync(
                CacheService,
                command.EmployerId,
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
