namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Employer aggregate counter block used in dashboard statistics caching.
    /// </summary>
    public sealed record DashboardEmployerStatisticsModel(
        int TotalEmployerCount,
        int PendingEmployerCount,
        int ActiveEmployerCount,
        int SuspendedEmployerCount,
        int BannedEmployerCount) :
        ModelBase;
}
