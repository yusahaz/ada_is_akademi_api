namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Employer worker portfolio row model.
    /// </summary>
    public sealed record WorkerPortfolioListItemModel(
        int WorkerId,
        string FullName,
        decimal ReliabilityScore,
        int CompletedAssignmentCount,
        int NoShowCount,
        int DisputeCount,
        DateTimeOffset? LastWorkedAt) :
        ModelBase;
}
