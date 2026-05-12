namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Persistence;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Returns summed commission receivable amounts grouped into aligned calendar buckets by granularity.
    /// </summary>
    public sealed class GetCommissionRevenueSeriesQuery :
        QueryBase<CommissionRevenueSeriesModel>
    {
        /// <summary>
        /// Reporting granularity (calendar months, quarters, half-years, or full years).
        /// </summary>
        public CommissionRevenueGranularity Granularity { get; set; }
    }

    internal sealed class GetCommissionRevenueSeriesQueryValidator :
        IRequestValidator<GetCommissionRevenueSeriesQuery>
    {
        /// <inheritdoc />
        public ValidationResult Validate(GetCommissionRevenueSeriesQuery request)
        {
            List<ValidationFailure> failures = [];

            if (!Enum.IsDefined(typeof(CommissionRevenueGranularity), request.Granularity))
            {
                failures.Add(
                    new ValidationFailure(
                        nameof(GetCommissionRevenueSeriesQuery.Granularity),
                        "INVALID_ENUM_VALUE",
                        "Granularity is not supported."));
            }

            return new ValidationResult(failures);
        }
    }

    internal sealed class GetCommissionRevenueSeriesQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetCommissionRevenueSeriesQuery, CommissionRevenueSeriesModel>(serviceProvider)
    {
        /// <inheritdoc />
        protected override async Task<CommissionRevenueSeriesModel> HandleAsync(
            GetCommissionRevenueSeriesQuery query,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.DashboardCommissionRevenueSeriesKey(query.Granularity);
            CommissionRevenueSeriesModel? cached =
                await CacheService.GetAsync<CommissionRevenueSeriesModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            IReadOnlyList<(DateOnly Start, DateOnly End, string Label)> bucketDefs =
                query.Granularity switch
                {
                    CommissionRevenueGranularity.Monthly => BuildMonthlyBuckets(today, monthCount: 12),
                    CommissionRevenueGranularity.Quarterly => BuildQuarterlyBuckets(today, quarterCount: 8),
                    CommissionRevenueGranularity.HalfYearly => BuildHalfYearlyBuckets(today, halfYearCount: 4),
                    CommissionRevenueGranularity.Yearly => BuildYearlyBuckets(today, yearCount: 5),
                    _ => BuildMonthlyBuckets(today, monthCount: 12),
                };

            IReadOnlyList<CommissionReceivable> receivables = (await UnitOfWork
                    .GetRepository<CommissionReceivable>()
                    .Filter()
                    .AsNoTracking()
                    .ToListAsync(cancellationToken))
                .ToList();

            Dictionary<int, Dictionary<string, decimal>> sumsByBucketAndCurrency = new();

            foreach (CommissionReceivable row in receivables)
            {
                DateOnly pe = row.PeriodEnd;
                for (int bi = 0; bi < bucketDefs.Count; bi++)
                {
                    (DateOnly start, DateOnly end, _) = bucketDefs[bi];
                    if (pe < start || pe > end)
                    {
                        continue;
                    }

                    string currency = row.Amount.Currency.Trim().Length > 0
                        ? row.Amount.Currency.Trim().ToUpperInvariant()
                        : "???";

                    if (!sumsByBucketAndCurrency.TryGetValue(bi, out Dictionary<string, decimal>? perCurrency))
                    {
                        perCurrency = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
                        sumsByBucketAndCurrency[bi] = perCurrency;
                    }

                    decimal amt = row.Amount.Amount;
                    _ = perCurrency.TryGetValue(currency, out decimal prev);
                    perCurrency[currency] = prev + amt;

                    break;
                }
            }

            CommissionRevenueSeriesBucketModel[] buckets = bucketDefs
                .Select((def, index) =>
                {
                    List<CommissionRevenueCurrencyAmountModel> amounts = sumsByBucketAndCurrency
                            .TryGetValue(index, out Dictionary<string, decimal>? dict)
                        ? dict
                            .OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
                            .Select(kv => new CommissionRevenueCurrencyAmountModel(kv.Key, decimal.Round(kv.Value, 2, MidpointRounding.AwayFromZero)))
                            .ToList()
                        : new List<CommissionRevenueCurrencyAmountModel>();

                    return new CommissionRevenueSeriesBucketModel(
                        def.Label,
                        def.Start,
                        def.End,
                        amounts);
                })
                .ToArray();

            CommissionRevenueSeriesModel model = new(query.Granularity, buckets);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.CommissionReceivableAllDependency()),
                cancellationToken);

            return model;
        }

        private static IReadOnlyList<(DateOnly Start, DateOnly End, string Label)> BuildMonthlyBuckets(
            DateOnly today,
            int monthCount)
        {
            DateOnly firstMonthStart = new DateOnly(today.Year, today.Month, 1).AddMonths(-(monthCount - 1));
            List<(DateOnly, DateOnly, string)> list = new(capacity: monthCount);

            for (int i = 0; i < monthCount; i++)
            {
                DateOnly monthStart = firstMonthStart.AddMonths(i);
                DateOnly monthEnd = new DateOnly(
                    monthStart.Year,
                    monthStart.Month,
                    DateTime.DaysInMonth(monthStart.Year, monthStart.Month));

                string label = monthStart.ToString("yyyy-MM", CultureInfo.InvariantCulture);

                list.Add((monthStart, monthEnd, label));
            }

            return list;
        }

        private static IReadOnlyList<(DateOnly Start, DateOnly End, string Label)> BuildQuarterlyBuckets(
            DateOnly today,
            int quarterCount)
        {
            int cq = ((today.Month - 1) / 3) + 1;
            int cy = today.Year;

            int sy = cy;
            int sq = cq;
            StepQuarter(ref sy, ref sq, -(quarterCount - 1));

            List<(DateOnly, DateOnly, string)> list = new(capacity: quarterCount);

            for (int i = 0; i < quarterCount; i++)
            {
                int y = sy;
                int q = sq;
                StepQuarter(ref y, ref q, i);

                DateOnly start = QuarterStart(y, q);
                DateOnly end = QuarterEnd(y, q);

                string label = $"{y.ToString(CultureInfo.InvariantCulture)} Q{q.ToString(CultureInfo.InvariantCulture)}";

                list.Add((start, end, label));
            }

            return list;
        }

        private static IReadOnlyList<(DateOnly Start, DateOnly End, string Label)> BuildHalfYearlyBuckets(
            DateOnly today,
            int halfYearCount)
        {
            int ch = today.Month <= 6 ? 1 : 2;
            int cy = today.Year;

            int sy = cy;
            int sh = ch;
            StepHalfYear(ref sy, ref sh, -(halfYearCount - 1));

            List<(DateOnly, DateOnly, string)> list = new(capacity: halfYearCount);

            for (int i = 0; i < halfYearCount; i++)
            {
                int y = sy;
                int h = sh;
                StepHalfYear(ref y, ref h, i);

                DateOnly start = h == 1 ? new DateOnly(y, 1, 1) : new DateOnly(y, 7, 1);
                DateOnly end = h == 1 ? new DateOnly(y, 6, 30) : new DateOnly(y, 12, 31);

                string label = $"{y.ToString(CultureInfo.InvariantCulture)} H{h.ToString(CultureInfo.InvariantCulture)}";

                list.Add((start, end, label));
            }

            return list;
        }

        private static IReadOnlyList<(DateOnly Start, DateOnly End, string Label)> BuildYearlyBuckets(
            DateOnly today,
            int yearCount)
        {
            int endYear = today.Year;
            int startYear = endYear - (yearCount - 1);

            List<(DateOnly, DateOnly, string)> list = new(capacity: yearCount);

            for (int y = startYear; y <= endYear; y++)
            {
                DateOnly start = new DateOnly(y, 1, 1);
                DateOnly end = new DateOnly(y, 12, 31);

                string label = y.ToString(CultureInfo.InvariantCulture);

                list.Add((start, end, label));
            }

            return list;
        }

        private static void StepQuarter(ref int year, ref int quarter, int delta)
        {
            quarter += delta;
            while (quarter > 4)
            {
                quarter -= 4;
                year++;
            }

            while (quarter < 1)
            {
                quarter += 4;
                year--;
            }
        }

        private static void StepHalfYear(ref int year, ref int half, int delta)
        {
            half += delta;
            while (half > 2)
            {
                half -= 2;
                year++;
            }

            while (half < 1)
            {
                half += 2;
                year--;
            }
        }

        private static DateOnly QuarterStart(int year, int quarter)
        {
            int month = ((quarter - 1) * 3) + 1;

            return new DateOnly(year, month, 1);
        }

        private static DateOnly QuarterEnd(int year, int quarter)
        {
            int month = quarter * 3;

            return new DateOnly(year, month, DateTime.DaysInMonth(year, month));
        }
    }
}
