namespace Azoxia.AdaIsAkademi.Application
{
    /// <summary>
    /// Generic paged query result including rows and paging metadata.
    /// </summary>
    /// <typeparam name="T">Row model type.</typeparam>
    public sealed record PagedQueryResultModel<T>(
        IReadOnlyList<T> Items,
        int TotalCount,
        int Limit,
        int Offset);
}
