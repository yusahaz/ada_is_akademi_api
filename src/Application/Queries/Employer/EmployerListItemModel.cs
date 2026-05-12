namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Employer row model for filtered listing.
    /// </summary>
    public sealed record EmployerListItemModel(
        decimal CommissionRate,
        int EmployerId,
        string Name,
        EmployerStatus Status,
        string TaxNumber,
        string? LogoObjectKey = null,
        string? LogoViewUrl = null) :
        ModelBase;
}
