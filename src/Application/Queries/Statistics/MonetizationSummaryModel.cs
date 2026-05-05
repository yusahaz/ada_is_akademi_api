namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Monetization-oriented summary counters and estimates.
    /// </summary>
    public sealed record MonetizationSummaryModel(
        int AcceptedJobApplicationCount,
        int ActiveEmployerCount,
        decimal EstimatedCommissionAmount,
        decimal EstimatedGrossTransactionVolume,
        int FilledOrCompletedJobPostingCount) :
        ModelBase;
}
