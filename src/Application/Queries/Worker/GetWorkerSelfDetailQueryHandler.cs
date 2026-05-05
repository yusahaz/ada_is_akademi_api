namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    internal class GetWorkerSelfDetailQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetWorkerSelfDetailQuery, WorkerSelfDetailModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<WorkerSelfDetailModel> HandleAsync(
            GetWorkerSelfDetailQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            CacheKey cacheKey = AdaIsCacheKeys.WorkerSelfDetailKey(workerId);
            WorkerSelfDetailModel? cached =
                await CacheService.GetAsync<WorkerSelfDetailModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            Worker? entity = await UnitOfWork
                .GetRepository<Worker>()
                .Filter(x => x.Id == workerId)
                .AsNoTracking()
                .Include(x => x.Skills)
                .Include(x => x.Availabilities)
                .Include(x => x.Certificates)
                .Include(x => x.Educations)
                .Include(x => x.Experiences)
                .Include(x => x.Languages)
                .Include(x => x.InterestedJobCategories)
                .FirstOrDefaultAsync(cancellationToken);

            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            IReadOnlyList<string> tags = entity.Skills
                .Select(x => x.Tag.Value)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var categoryItemsReader = new WorkerInterestedJobCategoryItemsReader(UnitOfWork);
            IReadOnlyList<WorkerInterestedJobCategoryItemModel> categories =
                await categoryItemsReader.ListForWorkerAsync(entity, cancellationToken);

            IWorkerProfileCompletionEvaluator completionEvaluator =
                ServiceProvider.GetRequiredService<IWorkerProfileCompletionEvaluator>();
            int profileCompletionPercent = completionEvaluator.CompletionPercentOf(entity);

            WorkerSelfDetailModel model = new(
                entity.Id,
                entity.SystemUserId,
                entity.Nationality,
                entity.University,
                entity.Bio,
                entity.ProfilePhotoObjectKey,
                entity.EmbeddingUpdatedAt,
                tags,
                entity.ExpectedSalaryMinMoney(),
                entity.ExpectedSalaryMaxMoney(),
                profileCompletionPercent,
                categories);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.WorkerDependency(entity.Id)),
                cancellationToken);

            return model;
        }

        #endregion Utils
    }
}
