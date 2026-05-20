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
using System;

    /// <summary>
    /// Updates authenticated worker's profile basics.
    /// </summary>
    public class UpdateWorkerProfileCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Optional target worker id for admin-initiated updates.
        /// </summary>
        public int? WorkerId { get; set; }

        /// <summary>
        /// Optional given name text.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Optional family name text.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Optional nationality text.
        /// </summary>
        public string? Nationality { get; set; }

        /// <summary>
        /// Optional university text.
        /// </summary>
        public string? University { get; set; }

        /// <summary>
        /// Optional gender; when omitted, existing value is kept.
        /// </summary>
        public WorkerGender? Gender { get; set; }

        /// <summary>
        /// Optional phone on linked system user; omitted when null and names are not updated.
        /// </summary>
        public string? Phone { get; set; }

        #endregion Properties
    }

    internal class UpdateWorkerProfileCommandValidator : IRequestValidator<UpdateWorkerProfileCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(UpdateWorkerProfileCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.WorkerId.HasValue &&
                request.WorkerId.Value <= 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(UpdateWorkerProfileCommand.WorkerId)));
            }

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

            if (!request.FirstName.IsNullOrWhiteSpace() &&
                request.FirstName.Trim().Length > 128)
            {
                failures.Add(ApplicationValidationCodes.UpdateWorkerProfileFirstNameMaxLength.ForField(nameof(UpdateWorkerProfileCommand.FirstName)));
            }

            if (!request.LastName.IsNullOrWhiteSpace() &&
                request.LastName.Trim().Length > 128)
            {
                failures.Add(ApplicationValidationCodes.UpdateWorkerProfileLastNameMaxLength.ForField(nameof(UpdateWorkerProfileCommand.LastName)));
            }

            if (request.Gender.HasValue &&
                !Enum.IsDefined(typeof(WorkerGender), request.Gender.Value))
            {
                failures.Add(ApplicationValidationCodes.UpdateWorkerProfileGenderInvalid.ForField(nameof(UpdateWorkerProfileCommand.Gender)));
            }

            if (!request.Phone.IsNullOrWhiteSpace() &&
                request.Phone.Trim().Length > 64)
            {
                failures.Add(ApplicationValidationCodes.UpdateWorkerProfilePhoneMaxLength.ForField(nameof(UpdateWorkerProfileCommand.Phone)));
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
            bool isAdmin = executionContext.GetClaim("system_user_type") == ((int)SystemUserType.Admin).ToString();
            int workerId = isAdmin && command.WorkerId.HasValue && command.WorkerId.Value > 0
                ? command.WorkerId.Value
                : executionContext.RequireAdaIsWorkerActorId();

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .GetByIdAsync(workerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            bool hasNameUpdate = !command.FirstName.IsNullOrWhiteSpace() || !command.LastName.IsNullOrWhiteSpace();
            bool shouldUpdateSystemUser = hasNameUpdate || command.Phone != null;
            if (shouldUpdateSystemUser)
            {
                SystemUser? systemUser = await UnitOfWork
                    .GetRepository<SystemUser>()
                    .GetByIdAsync(worker.SystemUserId, cancellationToken);
                systemUser = systemUser.ThrowIfNull(AzoxiaErrorCodes.NotFound);

                string resolvedFirstName = hasNameUpdate
                    ? command.FirstName.IsNullOrWhiteSpace()
                        ? systemUser.FirstName ?? string.Empty
                        : command.FirstName.Trim()
                    : systemUser.FirstName ?? string.Empty;
                string resolvedLastName = hasNameUpdate
                    ? command.LastName.IsNullOrWhiteSpace()
                        ? systemUser.LastName ?? string.Empty
                        : command.LastName.Trim()
                    : systemUser.LastName ?? string.Empty;
                string? resolvedPhone = command.Phone != null
                    ? command.Phone.IsNullOrWhiteSpace()
                        ? null
                        : command.Phone.Trim()
                    : hasNameUpdate
                        ? null
                        : systemUser.Phone;
                systemUser.Update(resolvedFirstName, resolvedLastName, resolvedPhone);
            }

            worker.UpdateProfile(command.Nationality, command.University, command.Gender);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await AdaIsReadModelCacheInvalidation.InvalidateWorkerReadModelsAsync(
                CacheService,
                workerId,
                cancellationToken,
                invalidateSystemUserScopes: shouldUpdateSystemUser);

            return Unit.Value;
        }

        #endregion Utils
    }
}
