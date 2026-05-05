namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Employer-specific commission estimate summary.
    /// </summary>
    public sealed record EmployerCommissionEstimateModel(
        int AcceptedApplicationCount,
        decimal CommissionRate,
        int EmployerId,
        decimal EstimatedCommissionAmount,
        decimal EstimatedGrossTransactionVolume) :
        ModelBase;
}
