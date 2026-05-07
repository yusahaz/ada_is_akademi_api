namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Supervisor list row for employer settings RBAC UI.
    /// </summary>
    public sealed record EmployerSupervisorListItemModel(
        int SystemUserId,
        string FullName,
        string Email,
        IReadOnlyList<int> AssignedLocationIds,
        IReadOnlyList<int> GroupIds,
        MembershipScopeType ScopeType) :
        ModelBase;
}
