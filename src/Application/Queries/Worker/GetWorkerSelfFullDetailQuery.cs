namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;

    /// <summary>
    /// Loads the authenticated worker's full profile including private matching preferences.
    /// </summary>
    public class GetWorkerSelfFullDetailQuery :
        QueryBase<WorkerSelfFullDetailModel>
    {
    }

    internal class GetWorkerSelfFullDetailQueryValidator : IRequestValidator<GetWorkerSelfFullDetailQuery>
    {
        /// <inheritdoc />
        public ValidationResult Validate(GetWorkerSelfFullDetailQuery request) =>
            new ValidationResult([]);
    }
}
