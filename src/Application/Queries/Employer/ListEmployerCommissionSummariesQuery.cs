namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;

    /// <summary>
    /// Lists employer commission summaries for monetization management.
    /// </summary>
    public class ListEmployerCommissionSummariesQuery :
        QueryBase<IReadOnlyList<EmployerCommissionListItemModel>>
    {
        #region Properties

        /// <summary>
        /// Maximum row count to return.
        /// </summary>
        public int Limit { get; set; } = 20;

        #endregion Properties
    }

    internal class ListEmployerCommissionSummariesQueryValidator : IRequestValidator<ListEmployerCommissionSummariesQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ListEmployerCommissionSummariesQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.Limit is < 1 or > 100)
            {
                failures.Add(ApplicationValidationCodes.ListEmployerCommissionSummariesLimitRange.ForField(nameof(ListEmployerCommissionSummariesQuery.Limit)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ListEmployerCommissionSummariesQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListEmployerCommissionSummariesQuery, IReadOnlyList<EmployerCommissionListItemModel>>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<IReadOnlyList<EmployerCommissionListItemModel>> HandleAsync(
            ListEmployerCommissionSummariesQuery query,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.EmployerCommissionSummaryListKey(query.Limit);
            IReadOnlyList<EmployerCommissionListItemModel>? cached =
                await CacheService.GetAsync<IReadOnlyList<EmployerCommissionListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            IEnumerable<Employer> employers = await UnitOfWork
                .GetRepository<Employer>()
                .Filter(x => x.Status == EmployerStatus.Active)
                .AsNoTracking()
                .OrderByDescending(x => x.CommissionRate)
                .Take(query.Limit)
                .ToListAsync(cancellationToken);

            var rows = new List<EmployerCommissionListItemModel>();
            foreach (Employer employer in employers)
            {
                IEnumerable<decimal> acceptedWages = await UnitOfWork
                    .GetRepository<JobApplication>()
                    .Filter(x => x.JobPosting.EmployerId == employer.Id && x.Status == JobApplicationStatus.Accepted)
                    .ToListAsync(x => x.JobPosting.Wage.Amount, cancellationToken);

                decimal gross = acceptedWages.Sum();
                int acceptedCount = acceptedWages.Count();
                decimal commission = decimal.Round(gross * employer.CommissionRate, 2, MidpointRounding.AwayFromZero);

                rows.Add(new EmployerCommissionListItemModel(
                    acceptedCount,
                    employer.CommissionRate,
                    employer.Id,
                    employer.Name,
                    employer.Status,
                    commission,
                    gross));
            }

            IReadOnlyList<EmployerCommissionListItemModel> result = rows
                .OrderByDescending(x => x.EstimatedCommissionAmount)
                .ThenByDescending(x => x.EstimatedGrossTransactionVolume)
                .ToList();

            await CacheService.SetAsync(
                cacheKey,
                result,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.EmployerAllDependency(),
                    AdaIsCacheKeys.JobApplicationAllDependency(),
                    AdaIsCacheKeys.JobPostingAllDependency()),
                cancellationToken);

            return result;
        }

        #endregion Utils
    }
}
