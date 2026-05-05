namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using System;

    /// <summary>
    /// Lists job postings that are currently open for applications.
    /// </summary>
    public class ListOpenJobPostingsQuery :
        QueryBase<PagedQueryResultModel<JobPostingSummaryModel>>
    {
        public string? CountryCode { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; }
    }

    internal class ListOpenJobPostingsQueryValidator : IRequestValidator<ListOpenJobPostingsQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ListOpenJobPostingsQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.Limit is < 1 or > 200)
            {
                failures.Add(ApplicationValidationCodes.ListOpenJobPostingsLimit.ForField(nameof(ListOpenJobPostingsQuery.Limit)));
            }

            if (request.Offset < 0)
            {
                failures.Add(ApplicationValidationCodes.ListOpenJobPostingsOffset.ForField(nameof(ListOpenJobPostingsQuery.Offset)));
            }

            if (!request.CountryCode.IsNullOrWhiteSpace() && request.CountryCode.Trim().Length > 16)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListOpenJobPostingsQuery.CountryCode)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ListOpenJobPostingsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListOpenJobPostingsQuery, PagedQueryResultModel<JobPostingSummaryModel>>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<JobPostingSummaryModel>> HandleAsync(
            ListOpenJobPostingsQuery query,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.OpenJobPostingListKey(query.Limit, query.Offset, query.CountryCode);
            PagedQueryResultModel<JobPostingSummaryModel>? cached =
                await CacheService.GetAsync<PagedQueryResultModel<JobPostingSummaryModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            var filter = UnitOfWork
                .GetRepository<JobPosting>()
                .Filter(x => x.Status == JobPostingStatus.Open && !x.IsDeleted)
                .AsNoTracking();

            if (!query.CountryCode.IsNullOrWhiteSpace())
            {
                string normalizedCountry = query.CountryCode.Trim().ToUpperInvariant();
                filter = filter.Filter(x => x.EmployerLocation.Address.Country.ToUpper() == normalizedCountry);
            }

            int totalCount = checked((int)await filter.CountAsync(cancellationToken));

            IEnumerable<JobPostingSummaryModel> rows = await filter
                .OrderBy(x => x.ShiftDate)
                .ThenBy(x => x.ShiftStartTime)
                .Skip(query.Offset)
                .Take(query.Limit)
                .ToListAsync(
                    x => new JobPostingSummaryModel(
                        x.Id,
                        x.Title,
                        x.ShiftDate,
                        x.ShiftStartTime,
                        x.ShiftEndTime,
                        x.Wage.Amount,
                        x.Wage.Currency,
                        x.EmployerId,
                        x.HeadCount),
                    cancellationToken);

            List<JobPostingSummaryModel> list = rows.ToList();
            PagedQueryResultModel<JobPostingSummaryModel> result =
                new(list, totalCount, query.Limit, query.Offset);

            await CacheService.SetAsync(
                cacheKey,
                result,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.JobPostingAllDependency()),
                cancellationToken);

            return result;
        }

        #endregion Utils
    }
}
