namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Job application aggregate counter block used in dashboard statistics caching.
    /// </summary>
    public sealed record DashboardJobApplicationStatisticsModel(
        int TotalJobApplicationCount,
        int PendingJobApplicationCount,
        int AcceptedJobApplicationCount,
        int RejectedJobApplicationCount,
        int WithdrawnJobApplicationCount) :
        ModelBase;
}
