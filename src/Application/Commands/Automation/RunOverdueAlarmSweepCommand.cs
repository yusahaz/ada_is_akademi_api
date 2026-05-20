namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Persistence;

    /// <summary>
    /// Scans overdue job postings and creates idempotent alarm rows.
    /// </summary>
    public class RunOverdueAlarmSweepCommand :
        CommandBase<int>;

    internal class RunOverdueAlarmSweepCommandValidator : IRequestValidator<RunOverdueAlarmSweepCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(RunOverdueAlarmSweepCommand request)
            => new();

        #endregion Methods
    }

    internal class RunOverdueAlarmSweepCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<RunOverdueAlarmSweepCommand, int>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<int> HandleAsync(
            RunOverdueAlarmSweepCommand command,
            CancellationToken cancellationToken)
        {
            IRepository<JobPosting> jobPostingRepository = UnitOfWork.GetRepository<JobPosting>();
            IRepository<OverdueJobAlarm> overdueJobAlarmRepository = UnitOfWork.GetRepository<OverdueJobAlarm>();

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

            IReadOnlyList<JobPosting> overduePostings = (await jobPostingRepository
                .Filter(x => (x.Status == JobPostingStatus.Open || x.Status == JobPostingStatus.Filled)
                             && x.ShiftDate < today
                             && !x.IsDeleted)
                .ToListAsync(cancellationToken))
                .ToList();

            if (overduePostings.Count == 0)
            {
                return 0;
            }

            int[] postingIds = overduePostings.Select(x => x.Id).ToArray();
            HashSet<int> existingPostingIds = (await overdueJobAlarmRepository
                    .Filter(x => x.AlarmDate == today && postingIds.Contains(x.JobPostingId))
                    .ToListAsync(x => x.JobPostingId, cancellationToken))
                .ToHashSet();

            int createdCount = 0;
            foreach (JobPosting overduePosting in overduePostings)
            {
                if (existingPostingIds.Contains(overduePosting.Id))
                {
                    continue;
                }

                UnitOfWork.Add(new OverdueJobAlarm(overduePosting.Id, today));
                createdCount++;
            }

            if (createdCount == 0)
            {
                return 0;
            }

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await AdaIsReadModelCacheInvalidation.InvalidateOverdueAlarmReadModelsAsync(
                CacheService,
                cancellationToken);

            return createdCount;
        }

        #endregion Utils
    }
}
