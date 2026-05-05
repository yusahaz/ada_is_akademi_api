namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using System.Text;

    /// <summary>
    /// Returns CSV export package for employer commission policies.
    /// </summary>
    public class ExportEmployerCommissionPoliciesCsvQuery :
        QueryBase<EmployerCommissionPolicyExportPackageModel>;

    internal class ExportEmployerCommissionPoliciesCsvQueryValidator : IRequestValidator<ExportEmployerCommissionPoliciesCsvQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ExportEmployerCommissionPoliciesCsvQuery request)
            => new();

        #endregion Methods
    }

    internal class ExportEmployerCommissionPoliciesCsvQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ExportEmployerCommissionPoliciesCsvQuery, EmployerCommissionPolicyExportPackageModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<EmployerCommissionPolicyExportPackageModel> HandleAsync(
            ExportEmployerCommissionPoliciesCsvQuery query,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.EmployerCommissionPolicyExportPackageKey();
            EmployerCommissionPolicyExportPackageModel? cached = await CacheService.GetAsync<EmployerCommissionPolicyExportPackageModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            IReadOnlyList<EmployerCommissionPolicyExportItemModel> rows = (await UnitOfWork
                    .GetRepository<Employer>()
                    .Filter()
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .ToListAsync(
                        x => new EmployerCommissionPolicyExportItemModel(
                            x.CommissionRate,
                            x.Id,
                            x.Name,
                            x.Status.ToString()),
                        cancellationToken))
                .ToList();

            string csv = BuildCsv(rows);
            var package = new EmployerCommissionPolicyExportPackageModel(
                "text/csv",
                csv,
                $"employer-commission-policy-{DateOnly.FromDateTime(DateTime.UtcNow):yyyyMMdd}.csv",
                rows.Count);

            await CacheService.SetAsync(
                cacheKey,
                package,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.EmployerAllDependency()),
                cancellationToken);

            return package;
        }

        private static string BuildCsv(IReadOnlyList<EmployerCommissionPolicyExportItemModel> rows)
        {
            StringBuilder sb = new();
            sb.AppendLine("employer_id,employer_name,employer_status,commission_rate");

            foreach (EmployerCommissionPolicyExportItemModel row in rows)
            {
                sb.AppendLine($"{row.EmployerId},\"{EscapeCsv(row.EmployerName)}\",{row.EmployerStatus},{row.CommissionRate:0.####}");
            }

            return sb.ToString();
        }

        private static string EscapeCsv(string value)
            => value.Replace("\"", "\"\"");

        #endregion Utils
    }
}
