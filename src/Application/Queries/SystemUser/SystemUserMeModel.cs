namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Authenticated system user profile snapshot for the <c>/me</c> endpoint.
    /// </summary>
    public sealed record SystemUserMeModel(
        int SystemUserId,
        int SystemUserType,
        string Email,
        string? FirstName,
        string? LastName,
        string? Phone,
        AccountStatus AccountStatus,
        bool IsLocked,
        int? EmployerId,
        int? WorkerId) :
        ModelBase;
}
