namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Persistence;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Lists employers with optional filtering and paging.
    /// </summary>
    public class ListEmployersQuery :
        QueryBase<PagedQueryResultModel<EmployerListItemModel>>
    {
        #region Properties
        public decimal? CommissionRateMax { get; set; }
        public decimal? CommissionRateMin { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; }
        public string? SearchText { get; set; }
        public EmployerStatus? Status { get; set; }

        /// <summary>
        /// Sort field: name, taxNumber, status, commissionRate, employerId (case-insensitive).
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// When true, sorts descending.
        /// </summary>
        [JsonPropertyName("sortDescending")]
        public bool SortDescending { get; set; }
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

            if (!request.SortBy.IsNullOrWhiteSpace())
            {
                string s = request.SortBy.Trim().ToLowerInvariant();
                if (s is not ("name" or "taxnumber" or "status" or "commissionrate" or "employerid"))
                {
                    failures.Add(ApplicationValidationCodes.ListEmployersSortBy.ForField(nameof(ListEmployersQuery.SortBy)));
                }
            }

            return new ValidationResult(failures);
        }
    }

    internal class ListEmployersQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListEmployersQuery, PagedQueryResultModel<EmployerListItemModel>>(serviceProvider)
    {
        private static readonly TimeSpan LogoViewTtl = TimeSpan.FromMinutes(10);

        protected override async Task<PagedQueryResultModel<EmployerListItemModel>> HandleAsync(ListEmployersQuery query, CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.EmployerListKey(query);
            PagedQueryResultModel<EmployerListItemModel>? cached = await CacheService.GetAsync<PagedQueryResultModel<EmployerListItemModel>>(cacheKey, cancellationToken);

            IReadOnlyList<EmployerListItemModel> rows;
            int totalCount;
            if (cached is not null)
            {
                rows = cached.Items;
                totalCount = cached.TotalCount;
            }
            else
            {
                IEntityFilterContext<Employer> filter = UnitOfWork.GetRepository<Employer>().Filter().AsNoTracking();
                filter = filter.Filter(x => !x.IsDeleted);
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

                string sort = query.SortBy.IsNullOrWhiteSpace()
                    ? "name"
                    : query.SortBy.Trim().ToLowerInvariant();
                bool desc = query.SortDescending;

                filter = (sort, desc) switch
                {
                    ("taxnumber", false) => filter.OrderBy(x => x.TaxNumber.Value),
                    ("taxnumber", true) => filter.OrderByDescending(x => x.TaxNumber.Value),
                    ("status", false) => filter.OrderBy(x => x.Status),
                    ("status", true) => filter.OrderByDescending(x => x.Status),
                    ("commissionrate", false) => filter.OrderBy(x => x.CommissionRate),
                    ("commissionrate", true) => filter.OrderByDescending(x => x.CommissionRate),
                    ("employerid", false) => filter.OrderBy(x => x.Id),
                    ("employerid", true) => filter.OrderByDescending(x => x.Id),
                    ("name", true) => filter.OrderByDescending(x => x.Name),
                    _ => filter.OrderBy(x => x.Name),
                };

                totalCount = checked((int)await filter.CountAsync(cancellationToken));

                rows = (await filter
                        .Skip(query.Offset)
                        .Take(query.Limit)
                        .ToListAsync(
                            x => new EmployerListItemModel(
                                x.CommissionRate,
                                x.Id,
                                x.Name,
                                x.Status,
                                x.TaxNumber.Value,
                                x.LogoObjectKey),
                            cancellationToken))
                    .ToList();

                PagedQueryResultModel<EmployerListItemModel> toCache = new(rows, totalCount, query.Limit, query.Offset);
                await CacheService.SetAsync(cacheKey, toCache, AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.EmployerAllDependency()), cancellationToken);
            }

            IReadOnlyList<EmployerListItemModel> enriched = await EnrichLogoViewUrlsAsync(rows, cancellationToken);
            return new PagedQueryResultModel<EmployerListItemModel>(enriched, totalCount, query.Limit, query.Offset);
        }

        private async Task<IReadOnlyList<EmployerListItemModel>> EnrichLogoViewUrlsAsync(
            IReadOnlyList<EmployerListItemModel> rows,
            CancellationToken cancellationToken)
        {
            if (rows.Count == 0)
            {
                return rows;
            }

            IObjectStoragePresigner presigner = ServiceProvider.GetRequiredService<IObjectStoragePresigner>();
            List<EmployerListItemModel> result = new(capacity: rows.Count);

            foreach (EmployerListItemModel row in rows)
            {
                if (row.LogoObjectKey.IsNullOrWhiteSpace())
                {
                    result.Add(row with { LogoViewUrl = null });
                    continue;
                }

                try
                {
                    string url = await presigner.CreatePresignedGetAsync(row.LogoObjectKey!, LogoViewTtl, cancellationToken);
                    result.Add(row with { LogoViewUrl = url });
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Employer list logo presign failed for employer {EmployerId}.", row.EmployerId);
                    result.Add(row with { LogoViewUrl = null });
                }
            }

            return result;
        }
    }
}
