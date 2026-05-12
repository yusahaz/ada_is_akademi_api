namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Lists job postings that are currently open for applications.
    /// </summary>
    public class ListOpenJobPostingsQuery :
        QueryBase<PagedQueryResultModel<JobPostingSummaryModel>>
    {
        public string? CountryCode { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; }

        /// <summary>Optional anchor latitude (decimal degrees) for geographic filtering.</summary>
        public double? NearLatitude { get; set; }

        /// <summary>Optional anchor longitude (decimal degrees) for geographic filtering.</summary>
        public double? NearLongitude { get; set; }

        /// <summary>
        /// Maximum great-circle distance in metres between <see cref="NearLatitude"/> / <see cref="NearLongitude"/>
        /// and the employer location coordinate. When omitted but both coordinates are set, defaults to 50 km.
        /// </summary>
        public int? RadiusMetres { get; set; }
    }

    internal class ListOpenJobPostingsQueryValidator : IRequestValidator<ListOpenJobPostingsQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ListOpenJobPostingsQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.Limit is < 1 or > 200)
            {
                failures.Add(ApplicationValidationCodes.ListOpenJobPostingsLimit.ForField(nameof(ListOpenJobPostingsQuery.Limit)));
            }

            if (request.Offset < 0)
            {
                failures.Add(ApplicationValidationCodes.ListOpenJobPostingsOffset.ForField(nameof(ListOpenJobPostingsQuery.Offset)));
            }

            if (!request.CountryCode.IsNullOrWhiteSpace() && request.CountryCode.Trim().Length > 16)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListOpenJobPostingsQuery.CountryCode)));
            }

            bool hasLat = request.NearLatitude.HasValue;
            bool hasLon = request.NearLongitude.HasValue;
            if (hasLat != hasLon)
            {
                failures.Add(ApplicationValidationCodes.ListOpenJobPostingsGeoPair.ForField(nameof(ListOpenJobPostingsQuery.NearLatitude)));
            }

            if (hasLat && hasLon)
            {
                double lat = request.NearLatitude!.Value;
                double lon = request.NearLongitude!.Value;
                if (lat is < -90d or > 90d)
                {
                    failures.Add(ApplicationValidationCodes.ListOpenJobPostingsGeoLatitude.ForField(nameof(ListOpenJobPostingsQuery.NearLatitude)));
                }

                if (lon is < -180d or > 180d)
                {
                    failures.Add(ApplicationValidationCodes.ListOpenJobPostingsGeoLongitude.ForField(nameof(ListOpenJobPostingsQuery.NearLongitude)));
                }

                if (request.RadiusMetres is int r && r is < 100 or > 500_000)
                {
                    failures.Add(ApplicationValidationCodes.ListOpenJobPostingsGeoRadius.ForField(nameof(ListOpenJobPostingsQuery.RadiusMetres)));
                }
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ListOpenJobPostingsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListOpenJobPostingsQuery, PagedQueryResultModel<JobPostingSummaryModel>>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<JobPostingSummaryModel>> HandleAsync(
            ListOpenJobPostingsQuery query,
            CancellationToken cancellationToken)
        {
            bool useGeoFilter = query.NearLatitude is not null && query.NearLongitude is not null;
            if (!useGeoFilter)
            {
                CacheKey cacheKey = AdaIsCacheKeys.OpenJobPostingListKey(query.Limit, query.Offset, query.CountryCode);
                PagedQueryResultModel<JobPostingSummaryModel>? cached =
                    await CacheService.GetAsync<PagedQueryResultModel<JobPostingSummaryModel>>(cacheKey, cancellationToken);
                if (cached is not null)
                {
                    return cached;
                }
            }

            var filter = UnitOfWork
                .GetRepository<JobPosting>()
                .Filter(static x => x.Status == JobPostingStatus.Open && !x.IsDeleted)
                .AsNoTracking();

            if (!query.CountryCode.IsNullOrWhiteSpace())
            {
                string normalizedCountry = query.CountryCode.Trim().ToUpperInvariant();
                filter = filter.Filter(x => x.EmployerLocation.Address.Country.ToUpper() == normalizedCountry);
            }

            if (useGeoFilter)
            {
                int radiusM = query.RadiusMetres ?? 50_000;
                GeoCoordinate center = new(query.NearLatitude!.Value, query.NearLongitude!.Value);

                var pointRows = await filter.ToListAsync(
                    x => new { x.Id, Lat = x.EmployerLocation.Coordinate.Latitude, Lon = x.EmployerLocation.Coordinate.Longitude },
                    cancellationToken);

                List<(int Id, double Latitude, double Longitude)> points = pointRows
                    .Select(p => (p.Id, p.Lat, p.Lon))
                    .ToList();

                static bool IsUnsetCoordinate(double latitude, double longitude) =>
                    Math.Abs(latitude) < 1e-9 && Math.Abs(longitude) < 1e-9;

                List<(int Id, double DistanceMetres)> inRadius = points
                    .Where(p => !IsUnsetCoordinate(p.Latitude, p.Longitude))
                    .Select(p =>
                    {
                        GeoCoordinate atPosting = new(p.Latitude, p.Longitude);
                        return (p.Id, DistanceMetres: center.DistanceTo(atPosting));
                    })
                    .Where(x => x.DistanceMetres <= radiusM)
                    .OrderBy(x => x.DistanceMetres)
                    .ThenBy(x => x.Id)
                    .ToList();

                int totalCountGeo = inRadius.Count;
                List<(int Id, double DistanceMetres)> window = inRadius
                    .Skip(query.Offset)
                    .Take(query.Limit)
                    .ToList();

                if (window.Count == 0)
                {
                    return new PagedQueryResultModel<JobPostingSummaryModel>(
                        Array.Empty<JobPostingSummaryModel>(),
                        totalCountGeo,
                        query.Limit,
                        query.Offset);
                }

                HashSet<int> idSet = window.Select(x => x.Id).ToHashSet();
                IReadOnlyDictionary<int, double> distMap = window.ToDictionary(x => x.Id, x => x.DistanceMetres);
                IReadOnlyList<int> idOrder = window.Select(x => x.Id).ToList();

                var narrowed = filter.Filter(x => idSet.Contains(x.Id));
                IEnumerable<JobPostingSummaryModel> rowEnumerable = await narrowed.ToListAsync(
                    static x => new JobPostingSummaryModel(
                        x.Id,
                        x.Title,
                        x.ShiftDate,
                        x.ShiftStartTime,
                        x.ShiftEndTime,
                        x.Wage.Amount,
                        x.Wage.Currency,
                        x.EmployerId,
                        x.Employer.Name,
                        x.Employer.LogoObjectKey,
                        x.EmployerLocation.Address.City + ", " + x.EmployerLocation.Address.Country,
                        x.HeadCount,
                        x.Applications.Count,
                        x.Status,
                        x.Skills.Select(skill => skill.Tag.Value).ToList(),
                        x.Skills.Where(skill => skill.IsRequired).Select(skill => skill.Tag.Value).ToList(),
                        x.Description,
                        x.EmployerLocation.Coordinate.Latitude,
                        x.EmployerLocation.Coordinate.Longitude,
                        null),
                    cancellationToken);

                List<JobPostingSummaryModel> rows = rowEnumerable.ToList();
                Dictionary<int, JobPostingSummaryModel> byId = rows.ToDictionary(x => x.Id);
                List<JobPostingSummaryModel> ordered = new(capacity: idOrder.Count);
                foreach (int id in idOrder)
                {
                    if (!byId.TryGetValue(id, out JobPostingSummaryModel? row))
                    {
                        continue;
                    }

                    ordered.Add(row with { DistanceMetres = distMap[id] });
                }

                return new PagedQueryResultModel<JobPostingSummaryModel>(ordered, totalCountGeo, query.Limit, query.Offset);
            }

            int totalCount = checked((int)await filter.CountAsync(cancellationToken));

            IEnumerable<JobPostingSummaryModel> rowsNonGeo = await filter
                .OrderBy(x => x.ShiftDate)
                .ThenBy(x => x.ShiftStartTime)
                .Skip(query.Offset)
                .Take(query.Limit)
                .ToListAsync(
                    static x => new JobPostingSummaryModel(
                        x.Id,
                        x.Title,
                        x.ShiftDate,
                        x.ShiftStartTime,
                        x.ShiftEndTime,
                        x.Wage.Amount,
                        x.Wage.Currency,
                        x.EmployerId,
                        x.Employer.Name,
                        x.Employer.LogoObjectKey,
                        x.EmployerLocation.Address.City + ", " + x.EmployerLocation.Address.Country,
                        x.HeadCount,
                        x.Applications.Count,
                        x.Status,
                        x.Skills.Select(skill => skill.Tag.Value).ToList(),
                        x.Skills.Where(skill => skill.IsRequired).Select(skill => skill.Tag.Value).ToList(),
                        x.Description,
                        x.EmployerLocation.Coordinate.Latitude,
                        x.EmployerLocation.Coordinate.Longitude,
                        null),
                    cancellationToken);

            List<JobPostingSummaryModel> list = rowsNonGeo.ToList();
            PagedQueryResultModel<JobPostingSummaryModel> result =
                new(list, totalCount, query.Limit, query.Offset);

            CacheKey cacheKeyNonGeo = AdaIsCacheKeys.OpenJobPostingListKey(query.Limit, query.Offset, query.CountryCode);
            await CacheService.SetAsync(
                cacheKeyNonGeo,
                result,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.JobPostingAllDependency()),
                cancellationToken);

            return result;
        }

        #endregion Utils
    }
}
