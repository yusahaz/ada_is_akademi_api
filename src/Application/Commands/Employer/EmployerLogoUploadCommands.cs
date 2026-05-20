namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// İşveren self: logo nesnesi için presigned PUT üretimi; ardından <see cref="ConfirmEmployerLogoUploadCommand"/>.
    /// </summary>
    public sealed class InitEmployerLogoUploadCommand :
        CommandBase<ObjectStorageUploadInitModel>
    {
        #region Properties

        /// <summary>
        /// PUT sırasında iletilecek içerik tipi önerisi.
        /// </summary>
        public string? ContentType { get; set; }

        #endregion Properties
    }

    internal sealed class InitEmployerLogoUploadCommandValidator :
        IRequestValidator<InitEmployerLogoUploadCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(InitEmployerLogoUploadCommand request)
        {
            List<ValidationFailure> failures = [];

            if (!request.ContentType.IsNullOrWhiteSpace() &&
                request.ContentType.Trim().Length > 128)
            {
                failures.Add(ApplicationValidationCodes.InitMediaUploadContentTypeMaxLength.ForField(
                    nameof(InitEmployerLogoUploadCommand.ContentType)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal sealed class InitEmployerLogoUploadCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<InitEmployerLogoUploadCommand, ObjectStorageUploadInitModel>(serviceProvider)
    {
        private static readonly TimeSpan UploadTtl = TimeSpan.FromMinutes(15);

        #region Methods

        /// <inheritdoc />
        protected override async Task<ObjectStorageUploadInitModel> HandleAsync(
            InitEmployerLogoUploadCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            _ = (await UnitOfWork
                    .GetRepository<Employer>()
                    .GetByIdAsync(employerId, cancellationToken))
                .ThrowIfNull(AzoxiaErrorCodes.NotFound);

            string objectKey = $"employers/{employerId}/logo/{Guid.NewGuid():N}";
            string contentType = command.ContentType.IsNullOrWhiteSpace()
                ? "application/octet-stream"
                : command.ContentType.Trim();

            IObjectStoragePresigner presigner = ServiceProvider.GetRequiredService<IObjectStoragePresigner>();
            PresignedBlobUploadResult signed = await presigner.CreatePresignedPutAsync(
                objectKey,
                contentType,
                UploadTtl,
                cancellationToken);

            return new ObjectStorageUploadInitModel(objectKey, signed.Url, signed.ExpiresAtUtc);
        }

        #endregion Methods
    }

    /// <summary>
    /// İşveren self: yüklemenin ardından logo nesnesi anahtarını kaydetme.
    /// </summary>
    public sealed class ConfirmEmployerLogoUploadCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Üretilmiş object key.
        /// </summary>
        public string? ObjectKey { get; set; }

        #endregion Properties
    }

    internal sealed class ConfirmEmployerLogoUploadCommandValidator :
        IRequestValidator<ConfirmEmployerLogoUploadCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ConfirmEmployerLogoUploadCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.ObjectKey.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.MediaBlobObjectKeyRequired.ForField(
                    nameof(ConfirmEmployerLogoUploadCommand.ObjectKey)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal sealed class ConfirmEmployerLogoUploadCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<ConfirmEmployerLogoUploadCommand>(serviceProvider)
    {
        private const int ObjectKeyMaxLength = 512;

        #region Methods

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(
            ConfirmEmployerLogoUploadCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            string objectKey = command.ObjectKey!.Trim();

            if (objectKey.Length > ObjectKeyMaxLength ||
                !OwnedEmployerLogoKey(employerId, objectKey))
            {
                ApplicationValidationCodes.MediaBlobObjectKeyOwnership.Throw();
            }

            Employer? employer = await UnitOfWork
                .GetRepository<Employer>()
                .GetByIdAsync(employerId, cancellationToken);
            employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            employer.SetLogoObjectKey(objectKey);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await AdaIsReadModelCacheInvalidation.InvalidateEmployerReadModelsAsync(
                CacheService,
                employerId,
                cancellationToken);

            return Unit.Value;
        }

        private static bool OwnedEmployerLogoKey(int employerId, string objectKey)
        {
            string prefix = $"employers/{employerId}/logo/";
            return objectKey.StartsWith(prefix, StringComparison.Ordinal) &&
                   objectKey.Length > prefix.Length;
        }

        #endregion Methods
    }

    /// <summary>
    /// İşveren self: logosu nesne anahtarını veritabanından kaldırma.
    /// </summary>
    public sealed class ClearEmployerLogoCommand :
        CommandBase
    {
    }

    internal sealed class ClearEmployerLogoCommandValidator :
        IRequestValidator<ClearEmployerLogoCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ClearEmployerLogoCommand _) =>
            new ValidationResult([]);

        #endregion Methods
    }

    internal sealed class ClearEmployerLogoCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<ClearEmployerLogoCommand>(serviceProvider)
    {
        #region Methods

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(
            ClearEmployerLogoCommand _,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            Employer? employer = await UnitOfWork
                .GetRepository<Employer>()
                .GetByIdAsync(employerId, cancellationToken);
            employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            employer.SetLogoObjectKey(null);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await AdaIsReadModelCacheInvalidation.InvalidateEmployerReadModelsAsync(
                CacheService,
                employerId,
                cancellationToken);

            return Unit.Value;
        }

        #endregion Methods
    }
}
