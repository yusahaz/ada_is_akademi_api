namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Registers a worker account and creates the related worker profile row.
    /// </summary>
    public class RegisterWorkerCommand :
        CommandBase<int>
    {
        #region Properties

        /// <summary>
        /// User email used as login identifier.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Optional given name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Optional family name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Password text for account bootstrap.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Optional phone contact.
        /// </summary>
        public string? Phone { get; set; }

        #endregion Properties
    }

    internal class RegisterWorkerCommandValidator : IRequestValidator<RegisterWorkerCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(RegisterWorkerCommand request)
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

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class RegisterWorkerCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<RegisterWorkerCommand, int>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<int> HandleAsync(RegisterWorkerCommand command, CancellationToken cancellationToken)
        {
            bool emailExists = await UnitOfWork
                .GetRepository<SystemUser>()
                .AnyAsync(x => x.Email == command.Email, cancellationToken);

            if (emailExists)
            {
                ApplicationValidationCodes.RegisterSystemUserEmailAlreadyExists.Throw();
            }

            SystemUser user = new(command.Email, command.Password, SystemUserType.Worker);
            user.Update(command.FirstName, command.LastName, command.Phone);
            UnitOfWork.Add(user);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            Worker worker = new(user.Id);
            UnitOfWork.Add(worker);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.WorkerDependency(worker.Id),
                cancellationToken);

            return worker.Id;
        }

        #endregion Utils
    }
}
