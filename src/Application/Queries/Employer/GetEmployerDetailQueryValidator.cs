namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;

    internal class GetEmployerDetailQueryValidator : IRequestValidator<GetEmployerDetailQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetEmployerDetailQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.GetEmployerByIdEmployerId.ForField(nameof(request.EmployerId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }
}
