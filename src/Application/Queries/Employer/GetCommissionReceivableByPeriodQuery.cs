namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Returns commission receivable detail by employer and period.
    /// </summary>
    public class GetCommissionReceivableByPeriodQuery :
        QueryBase<CommissionReceivableDetailModel>
    {
        #region Properties

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

    internal class GetCommissionReceivableByPeriodQueryValidator : IRequestValidator<GetCommissionReceivableByPeriodQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetCommissionReceivableByPeriodQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.GetCommissionReceivableByPeriodEmployerId.ForField(nameof(GetCommissionReceivableByPeriodQuery.EmployerId)));
            }

            if (request.PeriodEnd < request.PeriodStart)
            {
                failures.Add(ApplicationValidationCodes.GetCommissionReceivableByPeriodPeriod.ForField(nameof(GetCommissionReceivableByPeriodQuery.PeriodEnd)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class GetCommissionReceivableByPeriodQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetCommissionReceivableByPeriodQuery, CommissionReceivableDetailModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<CommissionReceivableDetailModel> HandleAsync(GetCommissionReceivableByPeriodQuery query, CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.CommissionReceivableDetailKey(query.EmployerId, query.PeriodStart, query.PeriodEnd);
            CommissionReceivableDetailModel? cached = await CacheService.GetAsync<CommissionReceivableDetailModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            CommissionReceivable? entity = await UnitOfWork
                .GetRepository<CommissionReceivable>()
                .Filter(x => x.EmployerId == query.EmployerId
                             && x.PeriodStart == query.PeriodStart
                             && x.PeriodEnd == query.PeriodEnd)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            CommissionReceivableDetailModel model = new(
                entity.Amount.Amount,
                entity.Amount.Currency,
                entity.CreatedAt,
                entity.EmployerId,
                entity.Id,
                $"{entity.PeriodStart:yyyy-MM}",
                entity.PeriodEnd,
                entity.PeriodStart,
                "Invoiced",
                entity.PeriodEnd.AddDays(14),
                null);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.CommissionReceivableDependency(query.EmployerId),
                    AdaIsCacheKeys.CommissionReceivableAllDependency()),
                cancellationToken);

            return model;
        }

        #endregion Utils
    }
}
