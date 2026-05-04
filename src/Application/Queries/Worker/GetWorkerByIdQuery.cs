namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using System;

    /// <summary>
    /// Loads a single worker profile read model by identifier.
    /// </summary>
    public class GetWorkerByIdQuery :
        QueryBase<WorkerDetailModel>
    {
        #region Properties

        /// <summary>
        /// Worker primary key.
        /// </summary>
        public int WorkerId { get; set; }

        #endregion Properties
    }

    internal class GetWorkerByIdQueryValidator : IRequestValidator<GetWorkerByIdQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetWorkerByIdQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.WorkerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.GetWorkerByIdWorkerId.ForField(nameof(request.WorkerId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class GetWorkerByIdQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetWorkerByIdQuery, WorkerDetailModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<WorkerDetailModel> HandleAsync(GetWorkerByIdQuery query, CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.WorkerDetailKey(query.WorkerId);
            WorkerDetailModel? cached = await CacheService.GetAsync<WorkerDetailModel>(cacheKey, cancellationToken);
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

            IReadOnlyList<string> tags = entity.Skills
                .Select(x => x.Tag.Value)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();

            WorkerDetailModel model = new(
                entity.Id,
                entity.SystemUserId,
                entity.Nationality,
                entity.University,
                entity.EmbeddingUpdatedAt,
                tags);

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
