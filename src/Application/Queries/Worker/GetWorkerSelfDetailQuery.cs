namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;

    /// <summary>
    /// Loads the authenticated worker's profile summary including private matching preferences.
    /// </summary>
    public class GetWorkerSelfDetailQuery :
        QueryBase<WorkerSelfDetailModel>
    {
    }

    internal class GetWorkerSelfDetailQueryValidator : IRequestValidator<GetWorkerSelfDetailQuery>
    {
        /// <inheritdoc />
        public ValidationResult Validate(GetWorkerSelfDetailQuery request) =>
            new ValidationResult([]);
    }
}
