namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    internal class GetWorkerLiveStatusFeedQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetWorkerLiveStatusFeedQuery, WorkerLiveStatusFeedModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<WorkerLiveStatusFeedModel> HandleAsync(
            GetWorkerLiveStatusFeedQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            CacheKey cacheKey = AdaIsCacheKeys.WorkerLiveStatusFeedKey(workerId, query.Limit);
            WorkerLiveStatusFeedModel? cached = await CacheService.GetAsync<WorkerLiveStatusFeedModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            List<WorkerLiveStatusFeedItemModel> items = [];

            List<ShiftAssignment> assignments = (await UnitOfWork
                    .GetRepository<ShiftAssignment>()
                    .Filter(x => x.WorkerId == workerId
                                 && !x.JobPosting.IsDeleted
                                 && x.JobPosting.ShiftDate >= today
                                 && (x.Status == ShiftAssignmentStatus.Pending
                                     || x.Status == ShiftAssignmentStatus.AwaitingMutualQr
                                     || x.Status == ShiftAssignmentStatus.CheckedIn))
                    .Include(x => x.JobPosting)
                    .AsNoTracking()
                    .OrderBy(x => x.JobPosting.ShiftDate)
                    .ThenBy(x => x.JobPosting.ShiftStartTime)
                    .Take(query.Limit)
                    .ToListAsync(cancellationToken))
                .ToList();

            foreach (ShiftAssignment assignment in assignments)
            {
                string body = assignment.Status switch
                {
                    ShiftAssignmentStatus.Pending => "Vardiya check-in bekliyor.",
                    ShiftAssignmentStatus.AwaitingMutualQr => "Karsilikli QR onayi bekleniyor.",
                    ShiftAssignmentStatus.CheckedIn => "Vardiya aktif, check-out zamani yaklasiyor.",
                    _ => "Vardiya durumu guncellendi.",
                };

                items.Add(new WorkerLiveStatusFeedItemModel(
                    "assignment_status",
                    assignment.Id,
                    assignment.JobPosting.Title,
                    body,
                    assignment.IsAnomalyFlagged ? "warning" : "info",
                    BuildShiftInstant(assignment.JobPosting.ShiftDate, assignment.JobPosting.ShiftStartTime)));
            }

            int remainingLimit = Math.Max(0, query.Limit - items.Count);
            if (remainingLimit > 0)
            {
                List<JobPosting> matchedPostings = (await UnitOfWork
                        .GetRepository<JobPosting>()
                        .Filter(x => x.Status == JobPostingStatus.Open
                                     && !x.IsDeleted
                                     && x.ShiftDate >= today
                                     && !x.Applications.Any(a => a.WorkerId == workerId))
                        .AsNoTracking()
                        .OrderBy(x => x.ShiftDate)
                        .ThenBy(x => x.ShiftStartTime)
                        .Take(remainingLimit)
                        .ToListAsync(cancellationToken))
                    .ToList();

                foreach (JobPosting posting in matchedPostings)
                {
                    items.Add(new WorkerLiveStatusFeedItemModel(
                        "matching_update",
                        posting.Id,
                        posting.Title,
                        "Profiline uygun yeni vardiya onerisi mevcut.",
                        "info",
                        BuildShiftInstant(posting.ShiftDate, posting.ShiftStartTime)));
                }
            }

            WorkerLiveStatusFeedModel model = new(
                items.OrderByDescending(x => x.OccurredAtUtc).Take(query.Limit).ToList(),
                DateTimeOffset.UtcNow);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.WorkerDependency(workerId),
                    AdaIsCacheKeys.ShiftAssignmentAllDependency(),
                    AdaIsCacheKeys.JobPostingAllDependency()),
                cancellationToken);

            return model;
        }

        private DateTimeOffset BuildShiftInstant(DateOnly shiftDate, TimeOnly shiftStartTime)
        {
            DateTime dateTime = shiftDate.ToDateTime(shiftStartTime, DateTimeKind.Utc);
            return new DateTimeOffset(dateTime);
        }

        #endregion Utils
    }
}
