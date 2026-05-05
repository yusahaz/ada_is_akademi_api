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
        QueryBase<IReadOnlyList<JobApplicationListItemModel>>
    {
        #region Properties

        /// <summary>
        /// Job posting whose applications are listed.
        /// </summary>
        public int JobPostingId { get; set; }

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
                failures.Add(ApplicationValidationCodes.ListJobApplicationsByJobPostingIdJobPostingId.ForField(nameof(request.JobPostingId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ListJobApplicationsByJobPostingIdQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListJobApplicationsByJobPostingIdQuery, IReadOnlyList<JobApplicationListItemModel>>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<IReadOnlyList<JobApplicationListItemModel>> HandleAsync(
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

            return applications
                .Select(x => new JobApplicationListItemModel(x.Id, x.WorkerId, x.Status, x.AppliedAt, x.Note))
                .ToList();
        }

        #endregion Utils
    }
}
