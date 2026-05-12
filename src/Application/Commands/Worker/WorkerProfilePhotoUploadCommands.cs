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
    /// Worker self: nesne anahtarı ve presigned PUT URL üretimi (yüklemeyi bitirdikten sonra <see cref="ConfirmWorkerProfilePhotoUploadCommand"/> kullanın).
    /// </summary>
    public sealed class InitWorkerProfilePhotoUploadCommand :
        CommandBase<ObjectStorageUploadInitModel>
    {
        #region Properties

        /// <summary>
        /// PUT isteğinde kullanılacak içerik türü önerisi (<c>image/jpeg</c> vb.).
        /// </summary>
        public string? ContentType { get; set; }

        #endregion Properties
    }

    internal sealed class InitWorkerProfilePhotoUploadCommandValidator :
        IRequestValidator<InitWorkerProfilePhotoUploadCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(InitWorkerProfilePhotoUploadCommand request)
        {
            List<ValidationFailure> failures = [];

            if (!request.ContentType.IsNullOrWhiteSpace() &&
                request.ContentType.Trim().Length > 128)
            {
                failures.Add(
                    ApplicationValidationCodes.InitMediaUploadContentTypeMaxLength.ForField(
                        nameof(InitWorkerProfilePhotoUploadCommand.ContentType)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal sealed class InitWorkerProfilePhotoUploadCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<InitWorkerProfilePhotoUploadCommand, ObjectStorageUploadInitModel>(serviceProvider)
    {
        private static readonly TimeSpan UploadTtl = TimeSpan.FromMinutes(15);

        #region Methods

        /// <inheritdoc />
        protected override async Task<ObjectStorageUploadInitModel> HandleAsync(
            InitWorkerProfilePhotoUploadCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            _ = (await UnitOfWork
                    .GetRepository<Worker>()
                    .GetByIdAsync(workerId, cancellationToken))
                .ThrowIfNull(AzoxiaErrorCodes.NotFound);

            string objectKey = $"workers/{workerId}/profile-photo/{Guid.NewGuid():N}";
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
    /// Worker self: yüklemeden sonra veritabanında profil fotoğrafı nesne anahtarını kaydeder.
    /// </summary>
    public sealed class ConfirmWorkerProfilePhotoUploadCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Önceki adımda dönen object key tam metni (case-sensitive beklenir).
        /// </summary>
        public string? ObjectKey { get; set; }

        #endregion Properties
    }

    internal sealed class ConfirmWorkerProfilePhotoUploadCommandValidator :
        IRequestValidator<ConfirmWorkerProfilePhotoUploadCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ConfirmWorkerProfilePhotoUploadCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.ObjectKey.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.MediaBlobObjectKeyRequired.ForField(
                    nameof(ConfirmWorkerProfilePhotoUploadCommand.ObjectKey)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal sealed class ConfirmWorkerProfilePhotoUploadCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<ConfirmWorkerProfilePhotoUploadCommand>(serviceProvider)
    {
        private const int ObjectKeyMaxLength = 512;

        #region Methods

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(
            ConfirmWorkerProfilePhotoUploadCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            string objectKey = command.ObjectKey!.Trim();

            if (objectKey.Length > ObjectKeyMaxLength ||
                !OwnedWorkerPhotoKey(workerId, objectKey))
            {
                ApplicationValidationCodes.MediaBlobObjectKeyOwnership.Throw();
            }

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .GetByIdAsync(workerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            worker.SetProfilePhotoObjectKey(objectKey);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        private static bool OwnedWorkerPhotoKey(int workerId, string objectKey)
        {
            string prefix = $"workers/{workerId}/profile-photo/";
            return objectKey.StartsWith(prefix, StringComparison.Ordinal) &&
                   objectKey.Length > prefix.Length;
        }

        #endregion Methods
    }

    /// <summary>
    /// Worker self: profil foto nesne anahtarını kaldırır (silme senaryosu).
    /// </summary>
    public sealed class ClearWorkerProfilePhotoCommand :
        CommandBase
    {
    }

    internal sealed class ClearWorkerProfilePhotoCommandValidator :
        IRequestValidator<ClearWorkerProfilePhotoCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ClearWorkerProfilePhotoCommand _) =>
            new ValidationResult([]);

        #endregion Methods
    }

    internal sealed class ClearWorkerProfilePhotoCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<ClearWorkerProfilePhotoCommand>(serviceProvider)
    {
        #region Methods

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(
            ClearWorkerProfilePhotoCommand _,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .GetByIdAsync(workerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            worker.SetProfilePhotoObjectKey(null);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        #endregion Methods
    }
}
