namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Job posting aggregate counter block used in dashboard statistics caching.
    /// </summary>
    public sealed record DashboardJobPostingStatisticsModel(
        int TotalJobPostingCount,
        int DraftJobPostingCount,
        int OpenJobPostingCount,
        int FilledJobPostingCount,
        int CompletedJobPostingCount,
        int CancelledJobPostingCount) :
        ModelBase;
}
