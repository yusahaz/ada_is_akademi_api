namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Azoxia.Core.ValueTypes;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Updates the authenticated worker's private matching preferences (expected salary and interested job categories).
    /// </summary>
    public class UpdateWorkerMatchingPreferencesCommand :
        CommandBase
    {
        /// <summary>
        /// When true, replaces expected salary bounds (<see cref="Money"/> payloads built from paired amount/currency fields).
        /// </summary>
        public bool SetExpectedSalary { get; set; }

        /// <summary>
        /// Optional lower inclusive amount; paired with <see cref="ExpectedSalaryMinCurrency"/>.
        /// </summary>
        public decimal? ExpectedSalaryMinAmount { get; set; }

        /// <summary>
        /// ISO currency for <see cref="ExpectedSalaryMinAmount"/> when declared.
        /// </summary>
        public string? ExpectedSalaryMinCurrency { get; set; }

        /// <summary>
        /// Optional upper inclusive amount; paired with <see cref="ExpectedSalaryMaxCurrency"/>.
        /// </summary>
        public decimal? ExpectedSalaryMaxAmount { get; set; }

        /// <summary>
        /// ISO currency for <see cref="ExpectedSalaryMaxAmount"/> when declared.
        /// </summary>
        public string? ExpectedSalaryMaxCurrency { get; set; }

        /// <summary>
        /// When true, replaces the interested job categories list (use an empty list to clear).
        /// </summary>
        public bool SetInterestedJobCategories { get; set; }

        /// <summary>
        /// Distinct <see cref="JobCategory"/> ids to associate; required when <see cref="SetInterestedJobCategories"/> is true.
        /// </summary>
        public List<int>? InterestedJobCategoryIds { get; set; }
    }

    internal class UpdateWorkerMatchingPreferencesCommandValidator :
        IRequestValidator<UpdateWorkerMatchingPreferencesCommand>
    {
        private const int InterestedCategoryMaxCount = 32;
        private const decimal SalaryAmountMaxInclusive = 999_999_999.99m;

        /// <inheritdoc />
        public ValidationResult Validate(UpdateWorkerMatchingPreferencesCommand request)
        {
            List<ValidationFailure> failures = [];

            if (!request.SetExpectedSalary && !request.SetInterestedJobCategories)
            {
                failures.Add(
                    ApplicationValidationCodes.UpdateWorkerMatchingNoOp.ForField(nameof(UpdateWorkerMatchingPreferencesCommand.SetExpectedSalary)));
            }

            if (request.SetExpectedSalary)
            {
                ValidateSalaryBound(
                    request.ExpectedSalaryMinAmount,
                    request.ExpectedSalaryMinCurrency,
                    nameof(UpdateWorkerMatchingPreferencesCommand.ExpectedSalaryMinAmount),
                    nameof(UpdateWorkerMatchingPreferencesCommand.ExpectedSalaryMinCurrency),
                    failures);

                ValidateSalaryBound(
                    request.ExpectedSalaryMaxAmount,
                    request.ExpectedSalaryMaxCurrency,
                    nameof(UpdateWorkerMatchingPreferencesCommand.ExpectedSalaryMaxAmount),
                    nameof(UpdateWorkerMatchingPreferencesCommand.ExpectedSalaryMaxCurrency),
                    failures);

                bool clearingAll = !request.ExpectedSalaryMinAmount.HasValue &&
                    !request.ExpectedSalaryMaxAmount.HasValue &&
                    request.ExpectedSalaryMinCurrency.IsNullOrWhiteSpace() &&
                    request.ExpectedSalaryMaxCurrency.IsNullOrWhiteSpace();

                if (!clearingAll &&
                    failures.Count == 0 &&
                    request.ExpectedSalaryMinAmount.HasValue &&
                    request.ExpectedSalaryMaxAmount.HasValue &&
                    request.ExpectedSalaryMinAmount!.Value > request.ExpectedSalaryMaxAmount!.Value)
                {
                    failures.Add(
                        ApplicationValidationCodes.UpdateWorkerMatchingExpectedSalaryMinMax.ForField(
                            nameof(UpdateWorkerMatchingPreferencesCommand.ExpectedSalaryMinAmount)));
                }

                if (!clearingAll &&
                    failures.Count == 0 &&
                    request.ExpectedSalaryMinAmount.HasValue &&
                    request.ExpectedSalaryMaxAmount.HasValue)
                {
                    string curMin = request.ExpectedSalaryMinCurrency!.Trim().ToUpperInvariant();
                    string curMax = request.ExpectedSalaryMaxCurrency!.Trim().ToUpperInvariant();
                    if (!string.Equals(curMin, curMax, StringComparison.Ordinal))
                    {
                        failures.Add(
                            ApplicationValidationCodes.UpdateWorkerMatchingExpectedSalaryCurrencyMismatch.ForField(
                                nameof(UpdateWorkerMatchingPreferencesCommand.ExpectedSalaryMaxCurrency)));
                    }
                }
            }

            if (request.SetInterestedJobCategories)
            {
                if (request.InterestedJobCategoryIds is null)
                {
                    failures.Add(
                        ApplicationValidationCodes.UpdateWorkerMatchingInterestedCategoryIdsRequired.ForField(
                            nameof(UpdateWorkerMatchingPreferencesCommand.InterestedJobCategoryIds)));
                }
                else
                {
                    if (request.InterestedJobCategoryIds.Count > InterestedCategoryMaxCount)
                    {
                        failures.Add(
                            ApplicationValidationCodes.UpdateWorkerMatchingInterestedCategoryCount.ForField(
                                nameof(UpdateWorkerMatchingPreferencesCommand.InterestedJobCategoryIds)));
                    }

                    if (request.InterestedJobCategoryIds.Any(id => id <= 0))
                    {
                        failures.Add(
                            ApplicationValidationCodes.UpdateWorkerMatchingInterestedCategoryId.ForField(
                                nameof(UpdateWorkerMatchingPreferencesCommand.InterestedJobCategoryIds)));
                    }

                    if (request.InterestedJobCategoryIds.Count != request.InterestedJobCategoryIds.Distinct().Count())
                    {
                        failures.Add(
                            ApplicationValidationCodes.UpdateWorkerMatchingInterestedCategoryDuplicates.ForField(
                                nameof(UpdateWorkerMatchingPreferencesCommand.InterestedJobCategoryIds)));
                    }
                }
            }

            return new ValidationResult(failures);
        }

        private static void ValidateSalaryBound(
            decimal? amount,
            string? currency,
            string amountField,
            string currencyField,
            List<ValidationFailure> failures)
        {
            bool hasAmount = amount.HasValue;
            bool hasCurrencyText = !currency.IsNullOrWhiteSpace();

            if (hasCurrencyText && currency!.Trim().Length != 3)
            {
                failures.Add(
                    ApplicationValidationCodes.UpdateWorkerMatchingExpectedSalaryCurrencyLength.ForField(currencyField));
            }

            if (hasAmount && !hasCurrencyText)
            {
                failures.Add(
                    ApplicationValidationCodes.UpdateWorkerMatchingExpectedSalaryCurrencyRequired.ForField(currencyField));
            }

            if (!hasAmount && hasCurrencyText)
            {
                failures.Add(
                    ApplicationValidationCodes.UpdateWorkerMatchingExpectedSalaryCurrencyWithoutAmount.ForField(currencyField));
            }

            if (hasAmount && amount!.Value < 0m)
            {
                failures.Add(
                    ApplicationValidationCodes.UpdateWorkerMatchingExpectedSalaryAmountRange.ForField(amountField));
            }

            if (hasAmount && amount!.Value > SalaryAmountMaxInclusive)
            {
                failures.Add(
                    ApplicationValidationCodes.UpdateWorkerMatchingExpectedSalaryAmountRange.ForField(amountField));
            }
        }
    }

    internal class UpdateWorkerMatchingPreferencesCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<UpdateWorkerMatchingPreferencesCommand>(serviceProvider)
    {
        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(
            UpdateWorkerMatchingPreferencesCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .GetByIdAsync(workerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            if (command.SetExpectedSalary)
            {
                Money? minimum = MoneyFromOptional(command.ExpectedSalaryMinAmount, command.ExpectedSalaryMinCurrency);
                Money? maximum = MoneyFromOptional(command.ExpectedSalaryMaxAmount, command.ExpectedSalaryMaxCurrency);
                worker.UpdateExpectedSalaryRange(minimum, maximum);
            }

            if (command.SetInterestedJobCategories)
            {
                IReadOnlyList<int> distinctIds = command.InterestedJobCategoryIds!
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                if (distinctIds.Count > 0)
                {
                    long found = await UnitOfWork
                        .GetRepository<JobCategory>()
                        .Filter(c => distinctIds.Contains(c.Id))
                        .AsNoTracking()
                        .CountAsync(cancellationToken);

                    (found == distinctIds.Count).ThrowIfFalse(
                        ApplicationValidationCodes.UpdateWorkerMatchingUnknownJobCategory);
                }

                worker.ReplaceInterestedJobCategories(distinctIds);
            }

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        private static Money? MoneyFromOptional(decimal? amount, string? currency)
        {
            if (!amount.HasValue || currency.IsNullOrWhiteSpace())
            {
                return null;
            }

            string code = currency.Trim().ToUpperInvariant();
            return new Money(amount!.Value, code);
        }
    }
}
