namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Lists shift assignments for the authenticated employer (all postings), optionally omitting completed rows.
    /// </summary>
    public class ListEmployerShiftAssignmentsQuery :
        QueryBase<PagedQueryResultModel<WorkerShiftAssignmentListItemModel>>
    {
        #region Properties

        /// <summary>
        /// When true, assignments in <see cref="ShiftAssignmentStatus.CheckedOut"/> are excluded (active-board view).
        /// </summary>
        public bool ExcludeCompleted { get; set; }

        public int Limit { get; set; } = 50;

        public int Offset { get; set; }

        #endregion Properties
    }

    internal class ListEmployerShiftAssignmentsQueryValidator : IRequestValidator<ListEmployerShiftAssignmentsQuery>
    {
        /// <inheritdoc />
        public ValidationResult Validate(ListEmployerShiftAssignmentsQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.Limit is < 1 or > 200)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListEmployerShiftAssignmentsQuery.Limit)));
            }

            if (request.Offset < 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListEmployerShiftAssignmentsQuery.Offset)));
            }

            return new ValidationResult(failures);
        }
    }

    internal class ListEmployerShiftAssignmentsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListEmployerShiftAssignmentsQuery, PagedQueryResultModel<WorkerShiftAssignmentListItemModel>>(serviceProvider)
    {
        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<WorkerShiftAssignmentListItemModel>> HandleAsync(
            ListEmployerShiftAssignmentsQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            CacheKey cacheKey = AdaIsCacheKeys.EmployerShiftAssignmentListKey(
                employerId,
                query.ExcludeCompleted,
                query.Limit,
                query.Offset);

            PagedQueryResultModel<WorkerShiftAssignmentListItemModel>? cached =
                await CacheService.GetAsync<PagedQueryResultModel<WorkerShiftAssignmentListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            var filter = UnitOfWork
                .GetRepository<ShiftAssignment>()
                .Filter(x =>
                    x.JobPosting.EmployerId == employerId
                    && !x.JobPosting.IsDeleted
                    && (!query.ExcludeCompleted || x.Status != ShiftAssignmentStatus.CheckedOut))
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
                        x.WorkerId,
                        x.Status,
                        x.IsAnomalyFlagged,
                        x.AnomalyCode,
                        x.IsAnomalyFlagged ? x.AnomalyCode : null,
                        x.IsAnomalyFlagged ? (x.CheckedOutAt ?? x.CheckedInAt) : null,
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
                    AdaIsCacheKeys.EmployerDependency(employerId),
                    AdaIsCacheKeys.ShiftAssignmentAllDependency()),
                cancellationToken);

            return result;
        }
    }
}
