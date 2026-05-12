namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;

    /// <summary>
    /// Registers an employer account and creates the employer aggregate.
    /// </summary>
    public class RegisterEmployerCommand :
        CommandBase<int>
    {
        #region Properties

        /// <summary>
        /// Optional employer description.
        /// </summary>
        public string? EmployerDescription { get; set; }

        /// <summary>
        /// Employer organization display name.
        /// </summary>
        public string EmployerName { get; set; }

        /// <summary>
        /// Employer primary address city.
        /// </summary>
        public string EmployerAddressCity { get; set; }

        /// <summary>
        /// Employer primary address country.
        /// </summary>
        public string EmployerAddressCountry { get; set; }

        /// <summary>
        /// Employer primary address line.
        /// </summary>
        public string EmployerAddressLine1 { get; set; }

        /// <summary>
        /// Employer tax number text.
        /// </summary>
        public string EmployerTaxNumber { get; set; }

        /// <summary>
        /// User email used as login identifier.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Given name used both for system user profile and employer contact.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Family name used both for system user profile and employer contact.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Password text for account bootstrap.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Phone used both for system user profile and employer contact.
        /// </summary>
        public string Phone { get; set; }

        #endregion Properties
    }

    internal class RegisterEmployerCommandValidator : IRequestValidator<RegisterEmployerCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(RegisterEmployerCommand request)
        {
            List<ValidationFailure> failures = [];

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                failures.Add(ApplicationValidationCodes.RegisterSystemUserEmailRequired.ForField(nameof(request.Email)));
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                failures.Add(ApplicationValidationCodes.RegisterSystemUserPasswordRequired.ForField(nameof(request.Password)));
            }

            if (string.IsNullOrWhiteSpace(request.EmployerName))
            {
                failures.Add(ApplicationValidationCodes.RegisterEmployerEmployerNameRequired.ForField(nameof(request.EmployerName)));
            }

            if (string.IsNullOrWhiteSpace(request.EmployerAddressLine1))
            {
                failures.Add(ApplicationValidationCodes.RegisterEmployerEmployerAddressLine1Required.ForField(nameof(request.EmployerAddressLine1)));
            }

            if (string.IsNullOrWhiteSpace(request.EmployerAddressCity))
            {
                failures.Add(ApplicationValidationCodes.RegisterEmployerEmployerAddressCityRequired.ForField(nameof(request.EmployerAddressCity)));
            }

            if (string.IsNullOrWhiteSpace(request.EmployerAddressCountry))
            {
                failures.Add(ApplicationValidationCodes.RegisterEmployerEmployerAddressCountryRequired.ForField(nameof(request.EmployerAddressCountry)));
            }

            if (string.IsNullOrWhiteSpace(request.EmployerTaxNumber))
            {
                failures.Add(ApplicationValidationCodes.RegisterEmployerEmployerTaxNumberRequired.ForField(nameof(request.EmployerTaxNumber)));
            }

            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                failures.Add(ApplicationValidationCodes.RegisterEmployerContactFirstNameRequired.ForField(nameof(request.FirstName)));
            }

            if (string.IsNullOrWhiteSpace(request.LastName))
            {
                failures.Add(ApplicationValidationCodes.RegisterEmployerContactLastNameRequired.ForField(nameof(request.LastName)));
            }

            if (string.IsNullOrWhiteSpace(request.Phone))
            {
                failures.Add(ApplicationValidationCodes.RegisterEmployerContactPhoneRequired.ForField(nameof(request.Phone)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class RegisterEmployerCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<RegisterEmployerCommand, int>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<int> HandleAsync(RegisterEmployerCommand command, CancellationToken cancellationToken)
        {
            bool emailExists = await UnitOfWork
                .GetRepository<SystemUser>()
                .AnyAsync(x => x.Email == command.Email, cancellationToken);

            if (emailExists)
            {
                ApplicationValidationCodes.RegisterSystemUserEmailAlreadyExists.Throw();
            }

            SystemUser user = new(command.Email, command.Password, SystemUserType.Employer);
            user.Update(command.FirstName, command.LastName, command.Phone);
            UnitOfWork.Add(user);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            Employer employer = new(command.EmployerName, command.EmployerDescription, command.EmployerTaxNumber);
            employer.SetAddress(new Address(command.EmployerAddressLine1, command.EmployerAddressCity, command.EmployerAddressCountry));
            employer.SetContact(new Contact(command.FirstName, command.LastName, command.Email, command.Phone));
            UnitOfWork.Add(employer);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            user.BindToEmployerOrganization(employer.Id);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.EmployerDependency(employer.Id),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.SystemUserAllDependency(),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.EmployerAllDependency(),
                cancellationToken);

            return employer.Id;
        }

        #endregion Utils
    }
}
