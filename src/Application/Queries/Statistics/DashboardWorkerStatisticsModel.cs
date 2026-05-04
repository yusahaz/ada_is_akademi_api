namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Worker-related counter block used in dashboard statistics caching.
    /// </summary>
    public sealed record DashboardWorkerStatisticsModel(
        int TotalWorkerCount,
        int PendingWorkerCount,
        int ActiveWorkerCount,
        int SuspendedWorkerCount,
        int BannedWorkerCount) :
        ModelBase;
}
