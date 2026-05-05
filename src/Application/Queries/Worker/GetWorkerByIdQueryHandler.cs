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

    internal class GetWorkerByIdQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetWorkerByIdQuery, WorkerEmployerSafeDetailModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<WorkerEmployerSafeDetailModel> HandleAsync(GetWorkerByIdQuery query, CancellationToken cancellationToken)
        {
            IWorkerEmployerProfileAccess workerEmployerProfileAccess =
                ServiceProvider.GetRequiredService<IWorkerEmployerProfileAccess>();
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();
            await workerEmployerProfileAccess.EnsureEmployerSharesJobApplicationWithWorkerAsync(
                UnitOfWork,
                employerId,
                query.WorkerId,
                cancellationToken);

            CacheKey cacheKey = AdaIsCacheKeys.WorkerEmployerSafeDetailKey(employerId, query.WorkerId);
            WorkerEmployerSafeDetailModel? cached =
                await CacheService.GetAsync<WorkerEmployerSafeDetailModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            Worker? entity = await UnitOfWork
                .GetRepository<Worker>()
                .Filter(x => x.Id == query.WorkerId)
                .AsNoTracking()
                .Include(x => x.Skills)
                .FirstOrDefaultAsync(cancellationToken);

            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            int employerViews =
                await workerEmployerProfileAccess.GetEmployerSourcedProfileViewCountAsync(
                    UnitOfWork,
                    employerId,
                    entity.Id,
                    cancellationToken);

            IReadOnlyList<string> tags = entity.Skills
                .Select(x => x.Tag.Value)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();

            WorkerEmployerSafeDetailModel model = new(
                entity.Id,
                entity.SystemUserId,
                entity.Nationality,
                entity.University,
                entity.EmbeddingUpdatedAt,
                tags,
                employerViews);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.WorkerDependency(entity.Id),
                    AdaIsCacheKeys.EmployerWorkerProfileViewStatDependency(employerId, entity.Id)),
                cancellationToken);

            return model;
        }

        #endregion Utils
    }
}
