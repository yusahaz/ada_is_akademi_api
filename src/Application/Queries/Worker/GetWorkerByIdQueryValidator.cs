namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;

    internal class GetWorkerByIdQueryValidator : IRequestValidator<GetWorkerByIdQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetWorkerByIdQuery request)
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
