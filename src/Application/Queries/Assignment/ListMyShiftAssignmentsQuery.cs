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
    /// Lists authenticated worker's own shift assignments.
    /// </summary>
    public class ListMyShiftAssignmentsQuery :
        QueryBase<PagedQueryResultModel<WorkerShiftAssignmentListItemModel>>
    {
        #region Properties

        public int Limit { get; set; } = 20;
        public int Offset { get; set; }

        #endregion Properties
    }

    internal class ListMyShiftAssignmentsQueryValidator : IRequestValidator<ListMyShiftAssignmentsQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ListMyShiftAssignmentsQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.Limit is < 1 or > 200)
            {
                failures.Add(ApplicationValidationCodes.ListMyShiftAssignmentsLimit.ForField(nameof(ListMyShiftAssignmentsQuery.Limit)));
            }

            if (request.Offset < 0)
            {
                failures.Add(ApplicationValidationCodes.ListMyShiftAssignmentsOffset.ForField(nameof(ListMyShiftAssignmentsQuery.Offset)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ListMyShiftAssignmentsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListMyShiftAssignmentsQuery, PagedQueryResultModel<WorkerShiftAssignmentListItemModel>>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<WorkerShiftAssignmentListItemModel>> HandleAsync(
            ListMyShiftAssignmentsQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            CacheKey cacheKey = AdaIsCacheKeys.WorkerShiftAssignmentListKey(workerId, query.Limit, query.Offset);
            PagedQueryResultModel<WorkerShiftAssignmentListItemModel>? cached =
                await CacheService.GetAsync<PagedQueryResultModel<WorkerShiftAssignmentListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            var filter = UnitOfWork
                .GetRepository<ShiftAssignment>()
                .Filter(x => x.WorkerId == workerId && !x.JobPosting.IsDeleted)
                .AsNoTracking();

            int totalCount = checked((int)await filter.CountAsync(cancellationToken));

            List<WorkerShiftAssignmentListItemModel> rows = (await filter
                .OrderByDescending(x => x.AssignedAt)
                .Skip(query.Offset)
                .Take(query.Limit)
                .ToListAsync(
                    x => new WorkerShiftAssignmentListItemModel(
                        x.Id,
                        x.JobPostingId,
                        x.JobApplicationId,
                        x.Status,
                        x.IsAnomalyFlagged,
                        x.AnomalyCode,
                        x.AssignedAt,
                        x.CheckedInAt,
                        x.CheckedOutAt,
                        x.JobPosting.ShiftDate,
                        x.JobPosting.ShiftStartTime,
                        x.JobPosting.ShiftEndTime),
                    cancellationToken))
                .ToList();

            PagedQueryResultModel<WorkerShiftAssignmentListItemModel> result =
                new(rows, totalCount, query.Limit, query.Offset);

            await CacheService.SetAsync(
                cacheKey,
                result,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.WorkerDependency(workerId),
                    AdaIsCacheKeys.ShiftAssignmentAllDependency()),
                cancellationToken);

            return result;
        }

        #endregion Utils
    }
}
