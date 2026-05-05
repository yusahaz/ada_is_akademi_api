namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Employer row model for commission-focused listing.
    /// </summary>
    public sealed record EmployerCommissionListItemModel(
        int AcceptedApplicationCount,
        decimal CommissionRate,
        int EmployerId,
        string EmployerName,
        EmployerStatus EmployerStatus,
        decimal EstimatedCommissionAmount,
        decimal EstimatedGrossTransactionVolume) :
        ModelBase;
}
