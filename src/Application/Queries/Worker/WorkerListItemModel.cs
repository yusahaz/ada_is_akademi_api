namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Worker row model for filtered listing.
    /// </summary>
    public sealed record WorkerListItemModel(
        AccountStatus AccountStatus,
        string Email,
        int SystemUserId,
        int WorkerId) :
        ModelBase;
}
