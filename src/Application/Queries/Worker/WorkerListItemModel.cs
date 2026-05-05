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
        string? FirstName,
        string? LastName,
        int SystemUserId,
        int WorkerId) :
        ModelBase
    {
        public string FullName =>
            string.Join(
                " ",
                new[] { FirstName, LastName }
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x!.Trim()));
    }
}
