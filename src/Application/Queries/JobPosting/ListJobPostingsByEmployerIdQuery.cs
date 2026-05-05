namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using System;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Lists all non-deleted job postings for a single employer (dashboard / yönetim; tüm ilan durumları).
    /// Owning employer is resolved exclusively from the authenticated token's <c>employer_id</c> claim.
    /// </summary>
    public class ListJobPostingsByEmployerIdQuery :
        QueryBase<PagedQueryResultModel<JobPostingSummaryModel>>
    {
    }

    internal class ListJobPostingsByEmployerIdQueryValidator : IRequestValidator<ListJobPostingsByEmployerIdQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ListJobPostingsByEmployerIdQuery request)
        {
            List<ValidationFailure> failures = [];

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ListJobPostingsByEmployerIdQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListJobPostingsByEmployerIdQuery, PagedQueryResultModel<JobPostingSummaryModel>>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<JobPostingSummaryModel>> HandleAsync(
            ListJobPostingsByEmployerIdQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            CacheKey cacheKey = AdaIsCacheKeys.EmployerJobPostingsSummaryKey(employerId);
            PagedQueryResultModel<JobPostingSummaryModel>? cached =
                await CacheService.GetAsync<PagedQueryResultModel<JobPostingSummaryModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            // Sıralama ve projeksiyon tek IQueryable üzerinde; EF SQL üretir (ORDER BY ShiftDate, ShiftStartTime, Id).
            List<JobPostingSummaryModel> rows = (await UnitOfWork
                .GetRepository<JobPosting>()
                .Filter(x => x.EmployerId == employerId && !x.IsDeleted)
                .AsNoTracking()
                .OrderByDescending(x => x.ShiftDate)
                .ThenByDescending(x => x.ShiftStartTime)
                .ThenBy(x => x.Id)
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
                    cancellationToken))
                .ToList();

            PagedQueryResultModel<JobPostingSummaryModel> result = new(
                rows,
                rows.Count,
                rows.Count,
                0);

            await CacheService.SetAsync(
                cacheKey,
                result,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.EmployerJobPostingsSummaryDependency(employerId)),
                cancellationToken);

            return result;
        }

        #endregion Utils
    }
}
