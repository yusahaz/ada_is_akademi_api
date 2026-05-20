namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;

    /// <summary>
    /// Returns the latest open CV upload session for the authenticated worker actor.
    /// </summary>
    public sealed class GetWorkerActiveCvUploadSessionQuery :
        QueryBase<WorkerActiveCvUploadSessionModel?>
    {
    }

    internal sealed class GetWorkerActiveCvUploadSessionQueryValidator :
        IRequestValidator<GetWorkerActiveCvUploadSessionQuery>
    {
        public ValidationResult Validate(GetWorkerActiveCvUploadSessionQuery _) =>
            new ValidationResult([]);
    }
}
