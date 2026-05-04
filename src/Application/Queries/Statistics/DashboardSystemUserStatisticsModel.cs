namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// System user aggregate counter block used in dashboard statistics caching.
    /// </summary>
    public sealed record DashboardSystemUserStatisticsModel(
        int TotalSystemUsers,
        int PendingSystemUserCount,
        int ActiveSystemUserCount,
        int SuspendedSystemUserCount,
        int BannedSystemUserCount,
        int ActivatedTodayCount) :
        ModelBase;
}
