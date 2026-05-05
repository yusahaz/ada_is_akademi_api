namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;

    /// <summary>
    /// Lists commission receivable rows by employer.
    /// </summary>
    public class ListCommissionReceivablesByEmployerQuery :
        QueryBase<IReadOnlyList<CommissionReceivableListItemModel>>
    {
        #region Properties

        /// <summary>
        /// Employer identifier.
        /// </summary>
        public int EmployerId { get; set; }

        /// <summary>
        /// Maximum row count to return.
        /// </summary>
        public int Limit { get; set; } = 20;

        #endregion Properties
    }

    internal class ListCommissionReceivablesByEmployerQueryValidator : IRequestValidator<ListCommissionReceivablesByEmployerQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ListCommissionReceivablesByEmployerQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.ListCommissionReceivablesByEmployerEmployerId.ForField(nameof(ListCommissionReceivablesByEmployerQuery.EmployerId)));
            }

            if (request.Limit is < 1 or > 100)
            {
                failures.Add(ApplicationValidationCodes.ListCommissionReceivablesByEmployerLimit.ForField(nameof(ListCommissionReceivablesByEmployerQuery.Limit)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ListCommissionReceivablesByEmployerQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListCommissionReceivablesByEmployerQuery, IReadOnlyList<CommissionReceivableListItemModel>>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<IReadOnlyList<CommissionReceivableListItemModel>> HandleAsync(
            ListCommissionReceivablesByEmployerQuery query,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.CommissionReceivableListKey(query.EmployerId, query.Limit);
            IReadOnlyList<CommissionReceivableListItemModel>? cached =
                await CacheService.GetAsync<IReadOnlyList<CommissionReceivableListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            IReadOnlyList<CommissionReceivableListItemModel> rows = (await UnitOfWork
                    .GetRepository<CommissionReceivable>()
                    .Filter(x => x.EmployerId == query.EmployerId)
                    .AsNoTracking()
                    .OrderByDescending(x => x.PeriodStart)
                    .ThenByDescending(x => x.PeriodEnd)
                    .Take(query.Limit)
                    .ToListAsync(
                        x => new CommissionReceivableListItemModel(
                            x.Amount.Amount,
                            x.Amount.Currency,
                            x.CreatedAt,
                            x.Id,
                            x.PeriodEnd,
                            x.PeriodStart),
                        cancellationToken))
                .ToList();

            await CacheService.SetAsync(
                cacheKey,
                rows,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.CommissionReceivableDependency(query.EmployerId),
                    AdaIsCacheKeys.CommissionReceivableAllDependency()),
                cancellationToken);

            return rows;
        }

        #endregion Utils
    }
}
