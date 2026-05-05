namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    internal class GetWorkerPersonalizedNotificationPreviewQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetWorkerPersonalizedNotificationPreviewQuery, WorkerNotificationPreviewModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<WorkerNotificationPreviewModel> HandleAsync(
            GetWorkerPersonalizedNotificationPreviewQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            CacheKey cacheKey = AdaIsCacheKeys.WorkerNotificationPreviewKey(workerId, query.JobPostingId);
            WorkerNotificationPreviewModel? cached = await CacheService.GetAsync<WorkerNotificationPreviewModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .Filter(x => x.Id == workerId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            SystemUser? systemUser = await UnitOfWork
                .GetRepository<SystemUser>()
                .Filter(x => x.Id == worker.SystemUserId)
                .AsNoTracking()
                .Include(x => x.Devices)
                .FirstOrDefaultAsync(cancellationToken);
            systemUser = systemUser.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            JobPosting? posting = await UnitOfWork
                .GetRepository<JobPosting>()
                .Filter(x => x.Id == query.JobPostingId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            posting = posting.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            SystemUserDevice? pushDevice = systemUser.Devices
                .Where(x => !x.DeviceToken.IsNullOrWhiteSpace())
                .OrderByDescending(x => x.LastActiveAt)
                .FirstOrDefault();

            bool fallbackApplied = pushDevice is null;
            string channel = fallbackApplied ? "email" : "push";
            var message = new WorkerNotificationPreviewMessageModel(
                posting.Title,
                posting.ShiftDate,
                "worker.semantic.match");

            var model = new WorkerNotificationPreviewModel(
                posting.Id,
                channel,
                message,
                fallbackApplied);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.WorkerDependency(worker.Id),
                    AdaIsCacheKeys.JobPostingDependency(posting.Id)),
                cancellationToken);

            return model;
        }

        #endregion Utils
    }
}
