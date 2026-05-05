namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Single row model for employer commission policy export.
    /// </summary>
    public sealed record EmployerCommissionPolicyExportItemModel(
        decimal CommissionRate,
        int EmployerId,
        string EmployerName,
        string EmployerStatus) :
        ModelBase;
}
