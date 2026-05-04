namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Summary counters for web dashboard statistic cards.
    /// </summary>
    public sealed record DashboardStatisticsModel(
        int TotalSystemUsers,
        int PendingSystemUserCount,
        int ActiveSystemUserCount,
        int SuspendedSystemUserCount,
        int BannedSystemUserCount,
        int ActivatedTodayCount,
        int TotalWorkerCount,
        int PendingWorkerCount,
        int ActiveWorkerCount,
        int SuspendedWorkerCount,
        int BannedWorkerCount,
        int TotalEmployerCount,
        int PendingEmployerCount,
        int ActiveEmployerCount,
        int SuspendedEmployerCount,
        int BannedEmployerCount,
        int TotalJobPostingCount,
        int DraftJobPostingCount,
        int OpenJobPostingCount,
        int FilledJobPostingCount,
        int CompletedJobPostingCount,
        int CancelledJobPostingCount,
        int TotalJobApplicationCount,
        int PendingJobApplicationCount,
        int AcceptedJobApplicationCount,
        int RejectedJobApplicationCount,
        int WithdrawnJobApplicationCount) :
        ModelBase;
}
