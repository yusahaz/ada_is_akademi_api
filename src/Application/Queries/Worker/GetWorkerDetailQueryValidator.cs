namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;

    internal class GetWorkerDetailQueryValidator : IRequestValidator<GetWorkerDetailQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetWorkerDetailQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.WorkerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.GetWorkerByIdWorkerId.ForField(nameof(request.WorkerId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }
}
