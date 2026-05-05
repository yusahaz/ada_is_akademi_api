namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Persistence;
    using System.Text;

    /// <summary>
    /// Returns CSV export package for overdue alarms.
    /// </summary>
    public class ExportOverdueAlarmsCsvQuery :
        QueryBase<OverdueAlarmExportPackageModel>;

    internal class ExportOverdueAlarmsCsvQueryValidator : IRequestValidator<ExportOverdueAlarmsCsvQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ExportOverdueAlarmsCsvQuery request)
            => new();

        #endregion Methods
    }

    internal class ExportOverdueAlarmsCsvQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ExportOverdueAlarmsCsvQuery, OverdueAlarmExportPackageModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<OverdueAlarmExportPackageModel> HandleAsync(
            ExportOverdueAlarmsCsvQuery query,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.OverdueAlarmExportPackageKey();
            OverdueAlarmExportPackageModel? cached = await CacheService.GetAsync<OverdueAlarmExportPackageModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            IRepository<OverdueJobAlarm> overdueAlarmRepository = UnitOfWork.GetRepository<OverdueJobAlarm>();
            IReadOnlyList<OverdueAlarmExportItemModel> rows = (await overdueAlarmRepository
                    .Filter()
                    .Include(x => x.JobPosting)
                    .OrderByDescending(x => x.AlarmDate)
                    .ThenByDescending(x => x.JobPostingId)
                    .ToListAsync(
                        x => new OverdueAlarmExportItemModel(
                            x.AlarmDate,
                            x.JobPosting.ShiftDate,
                            x.JobPostingId,
                            x.JobPosting.Status.ToString(),
                            x.JobPosting.Title),
                        cancellationToken))
                .ToList();

            string csv = BuildCsv(rows);
            string fileName = $"overdue-alarms-{DateOnly.FromDateTime(DateTime.UtcNow):yyyyMMdd}.csv";
            var package = new OverdueAlarmExportPackageModel(
                "text/csv",
                csv,
                fileName,
                rows.Count);

            await CacheService.SetAsync(
                cacheKey,
                package,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.OverdueAlarmAllDependency(),
                    AdaIsCacheKeys.JobPostingAllDependency()),
                cancellationToken);

            return package;
        }

        private static string BuildCsv(IReadOnlyList<OverdueAlarmExportItemModel> rows)
        {
            StringBuilder sb = new();
            sb.AppendLine("alarm_date,job_posting_id,title,job_posting_status,shift_date");

            foreach (OverdueAlarmExportItemModel row in rows)
            {
                sb.AppendLine(
                    $"{row.AlarmDate:yyyy-MM-dd},{row.JobPostingId},\"{EscapeCsv(row.Title)}\",{row.JobPostingStatus},{row.ShiftDate:yyyy-MM-dd}");
            }

            return sb.ToString();
        }

        private static string EscapeCsv(string value)
            => value.Replace("\"", "\"\"");

        #endregion Utils
    }
}
