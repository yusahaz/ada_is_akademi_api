namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Validation;

    internal class GetWorkerLiveStatusFeedQueryValidator : IRequestValidator<GetWorkerLiveStatusFeedQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetWorkerLiveStatusFeedQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.Limit is < 1 or > 50)
            {
                failures.Add(ApplicationValidationCodes.GetWorkerLiveStatusFeedLimit.ForField(nameof(GetWorkerLiveStatusFeedQuery.Limit)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }
}
