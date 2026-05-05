namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Persistence;
    using System.Text;

    /// <summary>
    /// Returns CSV export package for system-user notification dispatches.
    /// </summary>
    public class ExportSystemUserNotificationDispatchesCsvQuery :
        QueryBase<SystemUserNotificationDispatchExportPackageModel>;

    internal class ExportSystemUserNotificationDispatchesCsvQueryValidator : IRequestValidator<ExportSystemUserNotificationDispatchesCsvQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ExportSystemUserNotificationDispatchesCsvQuery request)
            => new();

        #endregion Methods
    }

    internal class ExportSystemUserNotificationDispatchesCsvQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ExportSystemUserNotificationDispatchesCsvQuery, SystemUserNotificationDispatchExportPackageModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<SystemUserNotificationDispatchExportPackageModel> HandleAsync(
            ExportSystemUserNotificationDispatchesCsvQuery query,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.SystemUserNotificationDispatchExportPackageKey();
            SystemUserNotificationDispatchExportPackageModel? cached =
                await CacheService.GetAsync<SystemUserNotificationDispatchExportPackageModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            IRepository<SystemUserNotificationDispatch> repository = UnitOfWork.GetRepository<SystemUserNotificationDispatch>();
            IReadOnlyList<SystemUserNotificationDispatchExportItemModel> rows = (await repository
                    .Filter()
                    .Include(x => x.SystemUser)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync(
                        x => new SystemUserNotificationDispatchExportItemModel(
                            x.Id,
                            x.SystemUserId,
                            x.SystemUser.Type.ToString(),
                            x.SystemUser.Email,
                            x.Channel.ToString(),
                            x.Status.ToString(),
                            x.TemplateCode,
                            x.Title,
                            x.RetryCount,
                            x.FallbackReason,
                            x.CreatedAt,
                            x.LastAttemptAt,
                            x.SentAt),
                        cancellationToken))
                .ToList();

            string csv = BuildCsv(rows);
            string fileName = $"system-user-notification-dispatches-{DateOnly.FromDateTime(DateTime.UtcNow):yyyyMMdd}.csv";
            var package = new SystemUserNotificationDispatchExportPackageModel(
                "text/csv",
                csv,
                fileName,
                rows.Count);

            await CacheService.SetAsync(
                cacheKey,
                package,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.SystemUserNotificationDispatchAllDependency(),
                    AdaIsCacheKeys.SystemUserAllDependency()),
                cancellationToken);

            return package;
        }

        private static string BuildCsv(IReadOnlyList<SystemUserNotificationDispatchExportItemModel> rows)
        {
            StringBuilder sb = new();
            sb.AppendLine("dispatch_id,system_user_id,system_user_type,email,channel,status,template_code,title,retry_count,fallback_reason,created_at,last_attempt_at,sent_at");

            foreach (SystemUserNotificationDispatchExportItemModel row in rows)
            {
                sb.AppendLine(
                    $"{row.DispatchId},{row.SystemUserId},{row.SystemUserType},\"{EscapeCsv(row.Email)}\",{row.Channel},{row.Status},\"{EscapeCsv(row.TemplateCode)}\",\"{EscapeCsv(row.Title)}\",{row.RetryCount},\"{EscapeCsv(row.FallbackReason ?? string.Empty)}\",{row.CreatedAt:O},{row.LastAttemptAt:O},{row.SentAt:O}");
            }

            return sb.ToString();
        }

        private static string EscapeCsv(string value)
            => value.Replace("\"", "\"\"");

        #endregion Utils
    }
}
