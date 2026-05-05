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

            bool hasPush = pushDevice is not null;
            bool hasVerifiedEmail = systemUser.EmailVerifiedAt.HasValue && !systemUser.Email.IsNullOrWhiteSpace();

            bool fallbackApplied = !hasPush;
            string? fallbackReason = null;
            string channel = "push";
            if (!hasPush && hasVerifiedEmail)
            {
                channel = "email";
                fallbackReason = "missing_push_token";
            }
            else if (!hasPush)
            {
                channel = "in_app";
                fallbackReason = "missing_push_token_and_unverified_email";
            }

            double personalizationScore = ComputePersonalizationScore(worker, posting);
            string personalizationSource = personalizationScore > 0d
                ? "semantic_cosine"
                : "rule_based_fallback";

            var message = new WorkerNotificationPreviewMessageModel(
                posting.Title,
                posting.ShiftDate,
                personalizationScore > 0d
                    ? "worker.semantic.match.personalized"
                    : "worker.semantic.match.fallback");

            var model = new WorkerNotificationPreviewModel(
                posting.Id,
                channel,
                message,
                fallbackApplied,
                fallbackReason,
                personalizationScore,
                personalizationSource,
                DateTimeOffset.UtcNow);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.WorkerDependency(worker.Id),
                    AdaIsCacheKeys.JobPostingDependency(posting.Id)),
                cancellationToken);

            return model;
        }

        private double ComputePersonalizationScore(Worker worker, JobPosting posting)
        {
            if (worker.SkillEmbedding is null || posting.DescriptionEmbedding is null)
            {
                return 0d;
            }

            if (worker.SkillEmbedding.Length == 0
                || posting.DescriptionEmbedding.Length == 0
                || worker.SkillEmbedding.Length != posting.DescriptionEmbedding.Length)
            {
                return 0d;
            }

            double dot = 0d;
            double leftNorm = 0d;
            double rightNorm = 0d;

            for (int i = 0; i < worker.SkillEmbedding.Length; i++)
            {
                float left = worker.SkillEmbedding[i];
                float right = posting.DescriptionEmbedding[i];
                dot += left * right;
                leftNorm += left * left;
                rightNorm += right * right;
            }

            if (leftNorm <= 0d || rightNorm <= 0d)
            {
                return 0d;
            }

            double cosine = dot / (Math.Sqrt(leftNorm) * Math.Sqrt(rightNorm));
            return Math.Round(Math.Max(0d, Math.Min(1d, cosine)), 4);
        }

        #endregion Utils
    }
}
