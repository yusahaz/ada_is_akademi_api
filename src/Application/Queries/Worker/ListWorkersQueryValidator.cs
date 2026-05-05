namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;

    internal class ListWorkersQueryValidator : IRequestValidator<ListWorkersQuery>
    {
        public ValidationResult Validate(ListWorkersQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.Limit is < 1 or > 200) failures.Add(ApplicationValidationCodes.ListWorkersLimit.ForField(nameof(ListWorkersQuery.Limit)));
            if (request.Offset < 0) failures.Add(ApplicationValidationCodes.ListWorkersOffset.ForField(nameof(ListWorkersQuery.Offset)));
            return new ValidationResult(failures);
        }
    }
}
