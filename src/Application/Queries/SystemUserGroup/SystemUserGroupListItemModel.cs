namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// System user group row model for filtered listing.
    /// </summary>
    public sealed record SystemUserGroupListItemModel(
        int Id,
        bool IsActive,
        bool IsSystem,
        int Level,
        string Name) :
        ModelBase;
}
