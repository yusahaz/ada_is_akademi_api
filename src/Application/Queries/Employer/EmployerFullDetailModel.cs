namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Full employer detail snapshot.
    /// </summary>
    public sealed record EmployerFullDetailModel(
        int Id,
        string Name,
        string? Description,
        EmployerStatus Status,
        string TaxNumber,
        decimal CommissionRate,
        EmployerContactModel? Contact,
        IReadOnlyList<EmployerLocationDetailModel> Locations,
        IReadOnlyList<EmployerSupervisorDetailModel> Supervisors) :
        ModelBase;
}
