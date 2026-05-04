namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using System;

    /// <summary>
    /// Lists job postings that are currently open for applications.
    /// </summary>
    public class ListOpenJobPostingsQuery :
        QueryBase<IReadOnlyList<JobPostingSummaryModel>>
    {
    }

    internal class ListOpenJobPostingsQueryValidator : IRequestValidator<ListOpenJobPostingsQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ListOpenJobPostingsQuery request)
        {
            return new ValidationResult([]);
        }

        #endregion Methods
    }

    internal class ListOpenJobPostingsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListOpenJobPostingsQuery, IReadOnlyList<JobPostingSummaryModel>>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<IReadOnlyList<JobPostingSummaryModel>> HandleAsync(
            ListOpenJobPostingsQuery query,
            CancellationToken cancellationToken)
        {
            IEnumerable<JobPostingSummaryModel> rows = await UnitOfWork
                .GetRepository<JobPosting>()
                .Filter(x => x.Status == JobPostingStatus.Open)
                .AsNoTracking()
                .OrderBy(x => x.ShiftDate)
                .ThenBy(x => x.ShiftStartTime)
                .ToListAsync(
                    x => new JobPostingSummaryModel(
                        x.Id,
                        x.Title,
                        x.ShiftDate,
                        x.ShiftStartTime,
                        x.ShiftEndTime,
                        x.Wage.Amount,
                        x.Wage.Currency,
                        x.EmployerId,
                        x.HeadCount),
                    cancellationToken);

            return rows.ToList();
        }

        #endregion Utils
    }
}
