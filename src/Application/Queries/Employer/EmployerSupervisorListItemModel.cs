namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Supervisor list row for employer settings.
    /// </summary>
    public sealed record EmployerSupervisorListItemModel(
        int SystemUserId,
        string FullName,
        string Email,
        IReadOnlyList<int> AssignedLocationIds) :
        ModelBase;
}
