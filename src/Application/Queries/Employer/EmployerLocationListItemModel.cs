namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Employer location list row.
    /// </summary>
    public sealed record EmployerLocationListItemModel(
        int LocationId,
        string Name,
        string City,
        double Latitude,
        double Longitude,
        int GeofenceRadiusMetres,
        bool IsActive) :
        ModelBase;
}
