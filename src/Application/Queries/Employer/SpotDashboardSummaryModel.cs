namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Employer spot-market dashboard counters.
    /// </summary>
    public sealed record SpotDashboardSummaryModel(
        decimal DailyFillRatePercent,
        int ActiveWorkerCount,
        int OpenPostingCount,
        int PendingApplicationCount,
        int ActiveAnomalyCount,
        int PendingPayoutCount) :
        ModelBase;
}
