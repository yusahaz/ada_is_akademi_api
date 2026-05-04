namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Employer snapshot for read APIs.
    /// </summary>
    /// <param name="Id">Employer identifier.</param>
    /// <param name="Name">Display name.</param>
    /// <param name="Description">Optional description.</param>
    /// <param name="Status">Lifecycle status.</param>
    /// <param name="TaxNumber">Normalized tax number text.</param>
    /// <param name="Contact">Primary contact, if configured.</param>
    public sealed record EmployerDetailModel(
        int Id,
        string Name,
        string? Description,
        EmployerStatus Status,
        string TaxNumber,
        EmployerContactModel? Contact) :
        ModelBase;
}
