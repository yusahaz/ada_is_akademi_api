namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Commission amount for a single currency within a time bucket.
    /// </summary>
    public sealed record CommissionRevenueCurrencyAmountModel(string Currency, decimal Amount) :
        ModelBase;

    /// <summary>
    /// One chart bucket with summed commission receivables attributed by period end date.
    /// </summary>
    public sealed record CommissionRevenueSeriesBucketModel(
        string Label,
        DateOnly PeriodStart,
        DateOnly PeriodEnd,
        IReadOnlyList<CommissionRevenueCurrencyAmountModel> Amounts) :
        ModelBase;

    /// <summary>
    /// Full commission revenue series for a granularity preset (aligned buckets, sparse fills zero externally).
    /// </summary>
    public sealed record CommissionRevenueSeriesModel(
        CommissionRevenueGranularity Granularity,
        IReadOnlyList<CommissionRevenueSeriesBucketModel> Buckets) :
        ModelBase;
}
