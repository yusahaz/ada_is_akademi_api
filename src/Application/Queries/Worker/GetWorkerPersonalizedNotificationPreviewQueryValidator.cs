namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;

    internal class GetWorkerPersonalizedNotificationPreviewQueryValidator : IRequestValidator<GetWorkerPersonalizedNotificationPreviewQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetWorkerPersonalizedNotificationPreviewQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.JobPostingId <= 0)
            {
                failures.Add(ApplicationValidationCodes.GetWorkerPersonalizedNotificationPreviewJobPostingId.ForField(nameof(GetWorkerPersonalizedNotificationPreviewQuery.JobPostingId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }
}
