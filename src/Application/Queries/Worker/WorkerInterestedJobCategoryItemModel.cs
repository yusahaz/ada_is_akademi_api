namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Read model row for an interested job category on a worker profile (self-scope only).
    /// </summary>
    public sealed record WorkerInterestedJobCategoryItemModel(int JobCategoryId, string Name) :
        ModelBase;
}
