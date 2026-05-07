namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Semantic worker search row model.
    /// </summary>
    public sealed record SemanticSearchedWorkerListItemModel(
        int WorkerId,
        string FullName,
        decimal SemanticScore,
        decimal ReliabilityScore,
        DateTimeOffset? LastWorkedAt,
        IReadOnlyList<string> Skills,
        IReadOnlyList<string> Languages,
        string City) :
        ModelBase;
}
