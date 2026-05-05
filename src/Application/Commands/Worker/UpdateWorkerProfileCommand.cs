namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Updates authenticated worker's profile basics.
    /// </summary>
    public class UpdateWorkerProfileCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Optional nationality text.
        /// </summary>
        public string? Nationality { get; set; }

        /// <summary>
        /// Optional university text.
        /// </summary>
        public string? University { get; set; }

        #endregion Properties
    }

    internal class UpdateWorkerProfileCommandValidator : IRequestValidator<UpdateWorkerProfileCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(UpdateWorkerProfileCommand request)
        {
            List<ValidationFailure> failures = [];

            if (!request.Nationality.IsNullOrWhiteSpace() &&
                request.Nationality.Trim().Length > 128)
            {
                failures.Add(ApplicationValidationCodes.UpdateWorkerProfileNationalityMaxLength.ForField(nameof(UpdateWorkerProfileCommand.Nationality)));
            }

            if (!request.University.IsNullOrWhiteSpace() &&
                request.University.Trim().Length > 512)
            {
                failures.Add(ApplicationValidationCodes.UpdateWorkerProfileUniversityMaxLength.ForField(nameof(UpdateWorkerProfileCommand.University)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class UpdateWorkerProfileCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<UpdateWorkerProfileCommand>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(UpdateWorkerProfileCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .GetByIdAsync(workerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            worker.UpdateProfile(command.Nationality, command.University);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.WorkerDependency(workerId),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.WorkerAllDependency(),
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
