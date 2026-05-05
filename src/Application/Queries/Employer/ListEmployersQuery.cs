namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Lists employers with optional filtering and paging.
    /// </summary>
    public class ListEmployersQuery :
        QueryBase<IReadOnlyList<EmployerListItemModel>>
    {
        #region Properties
        public decimal? CommissionRateMax { get; set; }
        public decimal? CommissionRateMin { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; }
        public string? SearchText { get; set; }
        public EmployerStatus? Status { get; set; }
        #endregion Properties
    }

    internal class ListEmployersQueryValidator : IRequestValidator<ListEmployersQuery>
    {
        public ValidationResult Validate(ListEmployersQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.Limit is < 1 or > 200)
            {
                failures.Add(ApplicationValidationCodes.ListEmployersLimit.ForField(nameof(ListEmployersQuery.Limit)));
            }
            if (request.Offset < 0)
            {
                failures.Add(ApplicationValidationCodes.ListEmployersOffset.ForField(nameof(ListEmployersQuery.Offset)));
            }
            if (request.CommissionRateMin.HasValue && request.CommissionRateMin.Value < 0m
                || request.CommissionRateMax.HasValue && request.CommissionRateMax.Value > 1m
                || request.CommissionRateMin.HasValue && request.CommissionRateMax.HasValue && request.CommissionRateMax.Value < request.CommissionRateMin.Value)
            {
                failures.Add(ApplicationValidationCodes.ListEmployersCommissionRange.ForField(nameof(ListEmployersQuery.CommissionRateMin)));
            }
            return new ValidationResult(failures);
        }
    }

    internal class ListEmployersQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListEmployersQuery, IReadOnlyList<EmployerListItemModel>>(serviceProvider)
    {
        protected override async Task<IReadOnlyList<EmployerListItemModel>> HandleAsync(ListEmployersQuery query, CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.EmployerListKey(query);
            IReadOnlyList<EmployerListItemModel>? cached = await CacheService.GetAsync<IReadOnlyList<EmployerListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null) return cached;

            var filter = UnitOfWork.GetRepository<Employer>().Filter().AsNoTracking();
            if (query.Status.HasValue)
            {
                filter = filter.Filter(x => x.Status == query.Status.Value);
            }
            if (!query.SearchText.IsNullOrWhiteSpace())
            {
                string s = query.SearchText.Trim().ToLowerInvariant();
                filter = filter.Filter(x => x.Name.ToLower().Contains(s));
            }
            if (query.CommissionRateMin.HasValue)
            {
                filter = filter.Filter(x => x.CommissionRate >= query.CommissionRateMin.Value);
            }
            if (query.CommissionRateMax.HasValue)
            {
                filter = filter.Filter(x => x.CommissionRate <= query.CommissionRateMax.Value);
            }

            IReadOnlyList<EmployerListItemModel> rows = (await filter
                    .OrderBy(x => x.Name)
                    .Skip(query.Offset)
                    .Take(query.Limit)
                    .ToListAsync(
                        x => new EmployerListItemModel(x.CommissionRate, x.Id, x.Name, x.Status, x.TaxNumber.Value),
                        cancellationToken))
                .ToList();

            await CacheService.SetAsync(cacheKey, rows, AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.EmployerAllDependency()), cancellationToken);
            return rows;
        }
    }
}
