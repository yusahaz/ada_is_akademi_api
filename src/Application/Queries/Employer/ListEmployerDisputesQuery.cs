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
    /// Lists dispute-like rows for employer operations.
    /// </summary>
    public class ListEmployerDisputesQuery :
        QueryBase<PagedQueryResultModel<EmployerDisputeListItemModel>>
    {
        #region Properties

        public DateTimeOffset? DateFrom { get; set; }
        public DateTimeOffset? DateTo { get; set; }
        public int Limit { get; set; } = 20;
        public int? LocationId { get; set; }
        public int Offset { get; set; }
        public string? Status { get; set; }

        #endregion Properties
    }

    internal class ListEmployerDisputesQueryValidator : IRequestValidator<ListEmployerDisputesQuery>
    {
        /// <inheritdoc />
        public ValidationResult Validate(ListEmployerDisputesQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.Limit is < 1 or > 200)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListEmployerDisputesQuery.Limit)));
            }

            if (request.Offset < 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListEmployerDisputesQuery.Offset)));
            }

            if (request.DateFrom.HasValue && request.DateTo.HasValue && request.DateFrom > request.DateTo)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListEmployerDisputesQuery.DateTo)));
            }

            return new ValidationResult(failures);
        }
    }

    internal class ListEmployerDisputesQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListEmployerDisputesQuery, PagedQueryResultModel<EmployerDisputeListItemModel>>(serviceProvider)
    {
        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<EmployerDisputeListItemModel>> HandleAsync(
            ListEmployerDisputesQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            string normalizedStatus = query.Status?.Trim().ToLowerInvariant() ?? "all";
            CacheKey cacheKey = new("query", "EmployerDisputes", $"{employerId}:{normalizedStatus}:{query.LocationId}:{query.DateFrom}:{query.DateTo}:{query.Limit}:{query.Offset}");
            PagedQueryResultModel<EmployerDisputeListItemModel>? cached =
                await CacheService.GetAsync<PagedQueryResultModel<EmployerDisputeListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            var filter = UnitOfWork
                .GetRepository<ShiftAssignment>()
                .Filter(x =>
                    x.JobPosting.EmployerId == employerId
                    && (!query.LocationId.HasValue || x.JobPosting.EmployerLocationId == query.LocationId.Value)
                    && (!query.DateFrom.HasValue || x.AssignedAt >= query.DateFrom.Value)
                    && (!query.DateTo.HasValue || x.AssignedAt <= query.DateTo.Value)
                    && (x.IsAnomalyFlagged || x.Status != ShiftAssignmentStatus.CheckedOut))
                .AsNoTracking();

            List<EmployerDisputeListItemModel> allRows = (await filter
                .OrderByDescending(x => x.AssignedAt)
                .ToListAsync(
                    x => new EmployerDisputeListItemModel(
                        x.Id,
                        x.Id,
                        x.WorkerId,
                        x.AnomalyCode ?? "ASSIGNMENT_STATUS",
                        x.IsAnomalyFlagged
                            ? $"Anomaly flagged: {x.AnomalyCode}"
                            : "Assignment flow indicates review required.",
                        x.Status == ShiftAssignmentStatus.CheckedOut ? "Resolved" : "InReview",
                        x.AssignedAt,
                        x.Status == ShiftAssignmentStatus.CheckedOut ? x.CheckedOutAt : null,
                        x.IsAnomalyFlagged,
                        x.AnomalyCode),
                    cancellationToken))
                .ToList();

            if (normalizedStatus != "all")
            {
                allRows = allRows
                    .Where(x => x.Status.ToLowerInvariant() == normalizedStatus)
                    .ToList();
            }

            int totalCount = allRows.Count;
            IReadOnlyList<EmployerDisputeListItemModel> rows = allRows.Skip(query.Offset).Take(query.Limit).ToList();
            PagedQueryResultModel<EmployerDisputeListItemModel> result = new(rows, totalCount, query.Limit, query.Offset);

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
