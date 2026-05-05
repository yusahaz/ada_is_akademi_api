namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Queries;

    /// <summary>
    /// Lists workers with optional filters and paging.
    /// </summary>
    public class ListWorkersQuery :
        QueryBase<PagedQueryResultModel<WorkerListItemModel>>
    {
        public AccountStatus? AccountStatus { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; }
        public string? SearchEmail { get; set; }
    }

}
