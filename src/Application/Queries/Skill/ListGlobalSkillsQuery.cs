namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Lists the normalized global skill dictionary used by employer autocomplete.
    /// </summary>
    public class ListGlobalSkillsQuery :
        QueryBase<IReadOnlyList<string>>
    {
        /// <summary>
        /// Max number of skill values to return.
        /// </summary>
        public int Limit { get; set; } = 500;
    }

    internal class ListGlobalSkillsQueryValidator : IRequestValidator<ListGlobalSkillsQuery>
    {
        /// <inheritdoc />
        public ValidationResult Validate(ListGlobalSkillsQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.Limit is < 1 or > 2000)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListGlobalSkillsQuery.Limit)));
            }

            return new ValidationResult(failures);
        }
    }

    internal class ListGlobalSkillsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListGlobalSkillsQuery, IReadOnlyList<string>>(serviceProvider)
    {
        /// <inheritdoc />
        protected override async Task<IReadOnlyList<string>> HandleAsync(
            ListGlobalSkillsQuery query,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.GlobalSkillDictionaryKey(query.Limit);
            IReadOnlyList<string>? cached = await CacheService.GetAsync<IReadOnlyList<string>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            List<string> workerSkills = (await UnitOfWork
                .GetRepository<WorkerSkill>()
                .Filter(static x => !x.Worker.IsDeleted)
                .AsNoTracking()
                .ToListAsync(static x => x.Tag.Value, cancellationToken))
                .ToList();

            List<string> postingSkills = (await UnitOfWork
                .GetRepository<JobPostingSkill>()
                .Filter(static x => !x.JobPosting.IsDeleted)
                .AsNoTracking()
                .ToListAsync(static x => x.Tag.Value, cancellationToken))
                .ToList();

            List<string> seededSkills = (await UnitOfWork
                .GetRepository<JobSkill>()
                .Filter(static x => !x.IsDeleted)
                .AsNoTracking()
                .ToListAsync(static x => x.Name, cancellationToken))
                .ToList();

            IReadOnlyList<string> dictionary = seededSkills
                .Concat(workerSkills)
                .Concat(postingSkills)
                .Where(static x => !string.IsNullOrWhiteSpace(x))
                .Select(static x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(static x => x)
                .Take(query.Limit)
                .ToList();

            await CacheService.SetAsync(
                cacheKey,
                dictionary,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.WorkerAllDependency(),
                    AdaIsCacheKeys.JobPostingAllDependency()),
                cancellationToken);

            return dictionary;
        }
    }
}
