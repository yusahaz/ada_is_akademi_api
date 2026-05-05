namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Employer commission policy read model.
    /// </summary>
    public sealed record EmployerCommissionPolicyModel(
        decimal CommissionRate,
        int EmployerId) :
        ModelBase;
}
