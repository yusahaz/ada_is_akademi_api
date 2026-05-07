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
    /// Lists authenticated employer locations.
    /// </summary>
    public class ListEmployerLocationsQuery :
        QueryBase<PagedQueryResultModel<EmployerLocationListItemModel>>
    {
        #region Properties

        public int Limit { get; set; } = 20;
        public int Offset { get; set; }

        #endregion Properties
    }

    internal class ListEmployerLocationsQueryValidator : IRequestValidator<ListEmployerLocationsQuery>
    {
        /// <inheritdoc />
        public ValidationResult Validate(ListEmployerLocationsQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.Limit is < 1 or > 200)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListEmployerLocationsQuery.Limit)));
            }

            if (request.Offset < 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListEmployerLocationsQuery.Offset)));
            }

            return new ValidationResult(failures);
        }
    }

    internal class ListEmployerLocationsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListEmployerLocationsQuery, PagedQueryResultModel<EmployerLocationListItemModel>>(serviceProvider)
    {
        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<EmployerLocationListItemModel>> HandleAsync(
            ListEmployerLocationsQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            CacheKey cacheKey = new("query", "EmployerLocations", $"{employerId}:{query.Limit}:{query.Offset}");
            PagedQueryResultModel<EmployerLocationListItemModel>? cached =
                await CacheService.GetAsync<PagedQueryResultModel<EmployerLocationListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            var filter = UnitOfWork
                .GetRepository<EmployerLocation>()
                .Filter(x => x.EmployerId == employerId && !x.IsDeleted)
                .AsNoTracking();
            int totalCount = checked((int)await filter.CountAsync(cancellationToken));

            List<EmployerLocationListItemModel> rows = (await filter
                .OrderBy(x => x.Name)
                .Skip(query.Offset)
                .Take(query.Limit)
                .ToListAsync(
                    x => new EmployerLocationListItemModel(
                        x.Id,
                        x.Name,
                        x.Address.City,
                        x.Coordinate.Latitude,
                        x.Coordinate.Longitude,
                        x.GeofenceRadiusMetres,
                        !x.IsDeleted),
                    cancellationToken))
                .ToList();

            PagedQueryResultModel<EmployerLocationListItemModel> result = new(rows, totalCount, query.Limit, query.Offset);
            await CacheService.SetAsync(
                cacheKey,
                result,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.EmployerDependency(employerId),
                    AdaIsCacheKeys.EmployerAllDependency()),
                cancellationToken);

            return result;
        }
    }
}
