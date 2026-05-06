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
    /// Loads a single job posting read model by identifier.
    /// </summary>
    public class GetJobPostingByIdQuery :
        QueryBase<JobPostingDetailModel>
    {
        #region Properties

        /// <summary>
        /// Job posting primary key.
        /// </summary>
        public int JobPostingId { get; set; }

        #endregion Properties
    }

    internal class GetJobPostingByIdQueryValidator : IRequestValidator<GetJobPostingByIdQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetJobPostingByIdQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.JobPostingId <= 0)
            {
                failures.Add(ApplicationValidationCodes.GetJobPostingByIdJobPostingId.ForField(nameof(request.JobPostingId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class GetJobPostingByIdQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetJobPostingByIdQuery, JobPostingDetailModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<JobPostingDetailModel> HandleAsync(GetJobPostingByIdQuery query, CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.JobPostingDetailKey(query.JobPostingId);
            JobPostingDetailModel? cached = await CacheService.GetAsync<JobPostingDetailModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            JobPosting? entity = await UnitOfWork
                .GetRepository<JobPosting>()
                .Filter(x => x.Id == query.JobPostingId)
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Skills)
                .Include(x => x.Applications)
                .FirstOrDefaultAsync(cancellationToken);

            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            int pending = entity.Applications.Count(x => x.Status == JobApplicationStatus.Pending);
            int accepted = entity.Applications.Count(x => x.Status == JobApplicationStatus.Accepted);

            IReadOnlyList<JobPostingSkillItemModel> skills = entity.Skills
                .OrderBy(x => x.Tag.Value)
                .Select(x => new JobPostingSkillItemModel(x.Tag.Value, x.IsRequired))
                .ToList();

            JobPostingDetailModel model = new(
                entity.Id,
                entity.Title,
                entity.Description,
                entity.Status,
                entity.EmployerId,
                entity.EmployerLocationId,
                entity.JobCategoryId,
                entity.ShiftDate,
                entity.ShiftStartTime,
                entity.ShiftEndTime,
                entity.Wage.Amount,
                entity.Wage.Currency,
                entity.HeadCount,
                pending,
                accepted,
                skills);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.JobPostingDependency(entity.Id)),
                cancellationToken);

            return model;
        }

        #endregion Utils
    }
}
