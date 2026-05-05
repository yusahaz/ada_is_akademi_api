namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;

    /// <summary>
    /// Generates an idempotent commission receivable row for an employer period.
    /// </summary>
    public class GenerateCommissionReceivableCommand :
        CommandBase<int>
    {
        #region Properties

        /// <summary>
        /// Commission amount value.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Commission amount currency.
        /// </summary>
        public string Currency { get; set; } = "TRY";

        /// <summary>
        /// Employer identifier.
        /// </summary>
        public int EmployerId { get; set; }

        /// <summary>
        /// Billing period end (inclusive).
        /// </summary>
        public DateOnly PeriodEnd { get; set; }

        /// <summary>
        /// Billing period start (inclusive).
        /// </summary>
        public DateOnly PeriodStart { get; set; }

        #endregion Properties
    }

    internal class GenerateCommissionReceivableCommandValidator : IRequestValidator<GenerateCommissionReceivableCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GenerateCommissionReceivableCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.GenerateCommissionReceivableEmployerId.ForField(nameof(GenerateCommissionReceivableCommand.EmployerId)));
            }

            if (request.Amount < 0m)
            {
                failures.Add(ApplicationValidationCodes.GenerateCommissionReceivableAmount.ForField(nameof(GenerateCommissionReceivableCommand.Amount)));
            }

            if (request.Currency.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.GenerateCommissionReceivableCurrency.ForField(nameof(GenerateCommissionReceivableCommand.Currency)));
            }

            if (request.PeriodEnd < request.PeriodStart)
            {
                failures.Add(ApplicationValidationCodes.GenerateCommissionReceivablePeriod.ForField(nameof(GenerateCommissionReceivableCommand.PeriodEnd)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class GenerateCommissionReceivableCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<GenerateCommissionReceivableCommand, int>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<int> HandleAsync(GenerateCommissionReceivableCommand command, CancellationToken cancellationToken)
        {
            Employer? employer = await UnitOfWork.GetRepository<Employer>().GetByIdAsync(command.EmployerId, cancellationToken);
            employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (employer.Status == EmployerStatus.Active)
                .ThrowIfFalse(DomainErrorCodes.CommissionReceivableEmployerNotActive);

            CommissionReceivable? existing = await UnitOfWork
                .GetRepository<CommissionReceivable>()
                .Filter(x => x.EmployerId == command.EmployerId
                             && x.PeriodStart == command.PeriodStart
                             && x.PeriodEnd == command.PeriodEnd)
                .FirstOrDefaultAsync(cancellationToken);

            if (existing is not null)
            {
                return existing.Id;
            }

            CommissionReceivable receivable = new(
                new Money(command.Amount, command.Currency),
                command.EmployerId,
                command.PeriodEnd,
                command.PeriodStart);

            UnitOfWork.Add(receivable);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.CommissionReceivableDependency(command.EmployerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.CommissionReceivableAllDependency(), cancellationToken);

            return receivable.Id;
        }

        #endregion Utils
    }
}
