namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// System user row model for filtered listing.
    /// </summary>
    public sealed record SystemUserListItemModel(
        AccountStatus AccountStatus,
        string Email,
        int Id,
        SystemUserType Type) :
        ModelBase;
}
