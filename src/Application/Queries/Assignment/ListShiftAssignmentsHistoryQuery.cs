namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Lists employer shift assignment history.
    /// </summary>
    public class ListShiftAssignmentsHistoryQuery :
        QueryBase<PagedQueryResultModel<ShiftAssignmentHistoryListItemModel>>
    {
        #region Properties

        public DateTimeOffset? DateFrom { get; set; }
        public DateTimeOffset? DateTo { get; set; }
        public int Limit { get; set; } = 20;
        public int? LocationId { get; set; }
        public int Offset { get; set; }
        public ShiftAssignmentStatus? Status { get; set; }

        #endregion Properties
    }

    internal class ListShiftAssignmentsHistoryQueryValidator : IRequestValidator<ListShiftAssignmentsHistoryQuery>
    {
        /// <inheritdoc />
        public ValidationResult Validate(ListShiftAssignmentsHistoryQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.Limit is < 1 or > 200)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListShiftAssignmentsHistoryQuery.Limit)));
            }

            if (request.Offset < 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListShiftAssignmentsHistoryQuery.Offset)));
            }

            if (request.DateFrom.HasValue && request.DateTo.HasValue && request.DateFrom > request.DateTo)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListShiftAssignmentsHistoryQuery.DateTo)));
            }

            return new ValidationResult(failures);
        }
    }

    internal class ListShiftAssignmentsHistoryQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListShiftAssignmentsHistoryQuery, PagedQueryResultModel<ShiftAssignmentHistoryListItemModel>>(serviceProvider)
    {
        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<ShiftAssignmentHistoryListItemModel>> HandleAsync(
            ListShiftAssignmentsHistoryQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            CacheKey cacheKey = new("query", "ShiftAssignmentsHistory", $"{employerId}:{query.LocationId}:{query.Status}:{query.DateFrom}:{query.DateTo}:{query.Limit}:{query.Offset}");
            PagedQueryResultModel<ShiftAssignmentHistoryListItemModel>? cached =
                await CacheService.GetAsync<PagedQueryResultModel<ShiftAssignmentHistoryListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            var filter = UnitOfWork
                .GetRepository<ShiftAssignment>()
                .Filter(x =>
                    x.JobPosting.EmployerId == employerId
                    && (!query.LocationId.HasValue || x.JobPosting.EmployerLocationId == query.LocationId.Value)
                    && (!query.Status.HasValue || x.Status == query.Status.Value)
                    && (!query.DateFrom.HasValue || x.AssignedAt >= query.DateFrom.Value)
                    && (!query.DateTo.HasValue || x.AssignedAt <= query.DateTo.Value))
                .AsNoTracking();

            int totalCount = checked((int)await filter.CountAsync(cancellationToken));

            List<ShiftAssignmentHistoryListItemModel> rows = (await filter
                .OrderByDescending(x => x.AssignedAt)
                .Skip(query.Offset)
                .Take(query.Limit)
                .ToListAsync(
                    x => new ShiftAssignmentHistoryListItemModel(
                        x.Id,
                        x.WorkerId,
                        x.Status,
                        x.Status == ShiftAssignmentStatus.Pending && x.CheckedInAt == null && x.CheckedOutAt == null,
                        x.CheckedOutAt,
                        x.IsAnomalyFlagged ? x.AnomalyCode : null,
                        x.Status != ShiftAssignmentStatus.CheckedOut ? "Assignment status indicates potential dispute." : null),
                    cancellationToken))
                .ToList();

            PagedQueryResultModel<ShiftAssignmentHistoryListItemModel> result = new(rows, totalCount, query.Limit, query.Offset);
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
