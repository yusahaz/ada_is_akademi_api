namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using System;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Lists applications submitted to a job posting (read side).
    /// </summary>
    public class ListJobApplicationsByJobPostingIdQuery :
        QueryBase<PagedQueryResultModel<JobApplicationListItemModel>>
    {
        #region Properties

        /// <summary>
        /// Job posting whose applications are listed.
        /// </summary>
        public int JobPostingId { get; set; }

        /// <summary>
        /// Maximum row count to return.
        /// </summary>
        public int Limit { get; set; } = 20;

        /// <summary>
        /// Zero-based row offset.
        /// </summary>
        public int Offset { get; set; }

        #endregion Properties
    }

    internal class ListJobApplicationsByJobPostingIdQueryValidator : IRequestValidator<ListJobApplicationsByJobPostingIdQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ListJobApplicationsByJobPostingIdQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.JobPostingId <= 0)
            {
                failures.Add(ApplicationValidationCodes.ListJobApplicationsByJobPostingIdJobPostingId.ForField(nameof(ListJobApplicationsByJobPostingIdQuery.JobPostingId)));
            }

            if (request.Limit is < 1 or > 200)
            {
                failures.Add(ApplicationValidationCodes.ListJobApplicationsByJobPostingIdLimit.ForField(nameof(ListJobApplicationsByJobPostingIdQuery.Limit)));
            }

            if (request.Offset < 0)
            {
                failures.Add(ApplicationValidationCodes.ListJobApplicationsByJobPostingIdOffset.ForField(nameof(ListJobApplicationsByJobPostingIdQuery.Offset)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ListJobApplicationsByJobPostingIdQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListJobApplicationsByJobPostingIdQuery, PagedQueryResultModel<JobApplicationListItemModel>>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<JobApplicationListItemModel>> HandleAsync(
            ListJobApplicationsByJobPostingIdQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            JobPosting? posting = await UnitOfWork
                .GetRepository<JobPosting>()
                .Filter(x => x.Id == query.JobPostingId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            posting = posting.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (posting.EmployerId == employerId).ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            // Order in-memory: SQLite cannot translate OrderBy on DateTimeOffset; PostgreSQL can, and result set is bounded by posting.
            List<JobApplication> applications = (await UnitOfWork
                .GetRepository<JobApplication>()
                .Filter(x => x.JobPostingId == query.JobPostingId)
                .AsNoTracking()
                .ToListAsync(cancellationToken))
                .OrderByDescending(x => x.AppliedAt)
                .ToList();

            List<JobApplicationListItemModel> rows = applications
                .Skip(query.Offset)
                .Take(query.Limit)
                .Select(x => new JobApplicationListItemModel(x.Id, x.WorkerId, x.Status, x.AppliedAt, x.Note))
                .ToList();

            return new PagedQueryResultModel<JobApplicationListItemModel>(rows, applications.Count, query.Limit, query.Offset);
        }

        #endregion Utils
    }
}
