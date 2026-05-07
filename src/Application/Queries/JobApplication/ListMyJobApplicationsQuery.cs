namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Lists authenticated worker's own applications.
    /// </summary>
    public class ListMyJobApplicationsQuery :
        QueryBase<PagedQueryResultModel<WorkerJobApplicationListItemModel>>
    {
        #region Properties

        public int Limit { get; set; } = 20;
        public int Offset { get; set; }

        #endregion Properties
    }

    internal class ListMyJobApplicationsQueryValidator : IRequestValidator<ListMyJobApplicationsQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ListMyJobApplicationsQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.Limit is < 1 or > 200)
            {
                failures.Add(ApplicationValidationCodes.ListMyJobApplicationsLimit.ForField(nameof(ListMyJobApplicationsQuery.Limit)));
            }

            if (request.Offset < 0)
            {
                failures.Add(ApplicationValidationCodes.ListMyJobApplicationsOffset.ForField(nameof(ListMyJobApplicationsQuery.Offset)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ListMyJobApplicationsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListMyJobApplicationsQuery, PagedQueryResultModel<WorkerJobApplicationListItemModel>>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<WorkerJobApplicationListItemModel>> HandleAsync(
            ListMyJobApplicationsQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            CacheKey cacheKey = AdaIsCacheKeys.WorkerJobApplicationListKey(workerId, query.Limit, query.Offset);
            PagedQueryResultModel<WorkerJobApplicationListItemModel>? cached =
                await CacheService.GetAsync<PagedQueryResultModel<WorkerJobApplicationListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            var filter = UnitOfWork
                .GetRepository<JobApplication>()
                .Filter(x => x.WorkerId == workerId && !x.JobPosting.IsDeleted)
                .AsNoTracking();

            int totalCount = checked((int)await filter.CountAsync(cancellationToken));

            List<WorkerJobApplicationListItemModel> rows = (await filter
                .OrderByDescending(x => x.AppliedAt)
                .Skip(query.Offset)
                .Take(query.Limit)
                .ToListAsync(
                    x => new WorkerJobApplicationListItemModel(
                        x.Id,
                        x.JobPostingId,
                        x.Status,
                        x.AppliedAt,
                        x.Note,
                        x.JobPosting.Title,
                        x.JobPosting.Employer.Name,
                        x.JobPosting.Employer.LogoObjectKey,
                        x.JobPosting.EmployerLocation.Address.City + ", " + x.JobPosting.EmployerLocation.Address.Country,
                        x.JobPosting.ShiftDate,
                        x.JobPosting.ShiftStartTime,
                        x.JobPosting.ShiftEndTime,
                        null),
                    cancellationToken))
                .ToList();

            if (rows.Count > 0)
            {
                List<int> applicationIds = rows
                    .Select(x => x.ApplicationId)
                    .ToList();

                Dictionary<int, int> assignmentByApplicationId = (await UnitOfWork
                    .GetRepository<ShiftAssignment>()
                    .Filter(x => applicationIds.Contains(x.JobApplicationId))
                    .AsNoTracking()
                    .ToListAsync(
                        x => new { x.JobApplicationId, x.Id },
                        cancellationToken))
                    .GroupBy(x => x.JobApplicationId)
                    .ToDictionary(x => x.Key, x => x.Max(y => y.Id));

                rows = rows
                    .Select(x => x with
                    {
                        AssignmentId = assignmentByApplicationId.TryGetValue(x.ApplicationId, out int assignmentId)
                            ? assignmentId
                            : null
                    })
                    .ToList();
            }

            PagedQueryResultModel<WorkerJobApplicationListItemModel> result =
                new(rows, totalCount, query.Limit, query.Offset);

            await CacheService.SetAsync(
                cacheKey,
                result,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.WorkerDependency(workerId),
                    AdaIsCacheKeys.JobApplicationAllDependency(),
                    AdaIsCacheKeys.ShiftAssignmentAllDependency()),
                cancellationToken);

            return result;
        }

        #endregion Utils
    }
}
