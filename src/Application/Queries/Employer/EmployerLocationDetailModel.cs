namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Employer location detail row.
    /// </summary>
    public sealed record EmployerLocationDetailModel(
        int Id,
        string Name,
        string? Description,
        int GeofenceRadiusMetres) :
        ModelBase;
}
