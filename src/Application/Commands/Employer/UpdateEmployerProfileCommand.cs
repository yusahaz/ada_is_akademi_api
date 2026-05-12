namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Updates employer display profile, tax id, description, and primary contact (admin).
    /// </summary>
    public class UpdateEmployerProfileCommand : CommandBase
    {
        public int EmployerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string TaxNumber { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
    }

    internal class UpdateEmployerProfileCommandValidator : IRequestValidator<UpdateEmployerProfileCommand>
    {
        public ValidationResult Validate(UpdateEmployerProfileCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.UpdateEmployerProfileEmployerId.ForField(nameof(request.EmployerId)));
            }

            if (request.Name.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.UpdateEmployerProfileNameRequired.ForField(nameof(request.Name)));
            }

            if (request.TaxNumber.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.UpdateEmployerProfileTaxNumberRequired.ForField(nameof(request.TaxNumber)));
            }

            if (request.FirstName.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.UpdateEmployerProfileFirstNameRequired.ForField(nameof(request.FirstName)));
            }

            if (request.LastName.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.UpdateEmployerProfileLastNameRequired.ForField(nameof(request.LastName)));
            }

            if (request.Email.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.UpdateEmployerProfileEmailRequired.ForField(nameof(request.Email)));
            }

            if (request.Phone.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.UpdateEmployerProfilePhoneRequired.ForField(nameof(request.Phone)));
            }

            return new ValidationResult(failures);
        }
    }

    internal class UpdateEmployerProfileCommandHandler(IServiceProvider serviceProvider)
        : CommandHandlerBase<UpdateEmployerProfileCommand>(serviceProvider)
    {
        protected override async Task<Unit> HandleAsync(UpdateEmployerProfileCommand command, CancellationToken cancellationToken)
        {
            try
            {
                Employer? employer = await UnitOfWork
                    .GetRepository<Employer>()
                    .GetByIdAsync(command.EmployerId, cancellationToken);
                employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);

                string? description = command.Description.IsNullOrWhiteSpace()
                    ? null
                    : command.Description.Trim();

                employer.UpdateEmployer(command.Name.Trim(), command.TaxNumber.Trim(), description);
                employer.SetContact(new Contact(
                    command.FirstName.Trim(),
                    command.LastName.Trim(),
                    command.Email.Trim(),
                    command.Phone.Trim()));

                await UnitOfWork.SaveChangesAsync(cancellationToken);

                try
                {
                    await CacheService.InvalidateByDependencyAsync(
                        AdaIsCacheKeys.EmployerDependency(command.EmployerId),
                        cancellationToken);
                    await CacheService.InvalidateByDependencyAsync(
                        AdaIsCacheKeys.EmployerAllDependency(),
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    // Cache invalidation should not fail the core persistence path.
                    Logger.LogWarning(ex, "Employer profile updated but cache invalidation failed for employer {EmployerId}.", command.EmployerId);
                }

                return Unit.Value;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Employer profile update failed for employer {EmployerId}.", command.EmployerId);
                throw new AzoxiaException(
                    new ErrorCode("AZX_EMPLOYER_UPDATE_FAILED", $"Employer profile update failed: {ex.Message}"),
                    ex);
            }
        }
    }
}
