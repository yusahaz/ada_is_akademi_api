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
    /// Worker self: starts CV upload by issuing an object key and presigned PUT URL.
    /// </summary>
    public sealed class InitWorkerCvUploadCommand :
        CommandBase<ObjectStorageUploadInitModel>
    {
        #region Properties

        /// <summary>
        /// Optional content type hint (defaults to <c>application/octet-stream</c>).
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Original client-side file name with extension.
        /// </summary>
        public string? FileName { get; set; }

        #endregion Properties
    }

    internal sealed class InitWorkerCvUploadCommandValidator :
        IRequestValidator<InitWorkerCvUploadCommand>
    {
        private const int ContentTypeMaxLength = 128;
        private const int FileNameMaxLength = 256;

        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(InitWorkerCvUploadCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.FileName.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.WorkerCvFileNameRequired.ForField(nameof(InitWorkerCvUploadCommand.FileName)));
            }
            else if (request.FileName.Trim().Length > FileNameMaxLength)
            {
                failures.Add(ApplicationValidationCodes.WorkerCvFileNameMaxLength.ForField(nameof(InitWorkerCvUploadCommand.FileName)));
            }

            if (!request.ContentType.IsNullOrWhiteSpace() &&
                request.ContentType.Trim().Length > ContentTypeMaxLength)
            {
                failures.Add(ApplicationValidationCodes.WorkerCvContentTypeMaxLength.ForField(nameof(InitWorkerCvUploadCommand.ContentType)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal sealed class InitWorkerCvUploadCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<InitWorkerCvUploadCommand, ObjectStorageUploadInitModel>(serviceProvider)
    {
        private readonly TimeSpan _uploadTtl = TimeSpan.FromMinutes(20);

        #region Methods

        /// <inheritdoc />
        protected override async Task<ObjectStorageUploadInitModel> HandleAsync(
            InitWorkerCvUploadCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            _ = (await UnitOfWork
                    .GetRepository<Worker>()
                    .GetByIdAsync(workerId, cancellationToken))
                .ThrowIfNull(AzoxiaErrorCodes.NotFound);

            string normalizedFileName = command.FileName!.Trim();
            CvFileFormat format = ResolveFormat(normalizedFileName);
            string extension = format == CvFileFormat.Pdf ? ".pdf" : ".docx";

            string objectKey = $"workers/{workerId}/cv/{Guid.NewGuid():N}{extension}";
            string contentType = command.ContentType.IsNullOrWhiteSpace()
                ? "application/octet-stream"
                : command.ContentType.Trim();

            IObjectStoragePresigner presigner = ServiceProvider.GetRequiredService<IObjectStoragePresigner>();
            PresignedBlobUploadResult signed = await presigner.CreatePresignedPutAsync(
                objectKey,
                contentType,
                _uploadTtl,
                cancellationToken);

            return new ObjectStorageUploadInitModel(objectKey, signed.Url, signed.ExpiresAtUtc);
        }

        private CvFileFormat ResolveFormat(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (extension == ".pdf")
            {
                return CvFileFormat.Pdf;
            }

            if (extension == ".docx")
            {
                return CvFileFormat.Docx;
            }

            ApplicationValidationCodes.WorkerCvFileFormatNotSupported.Throw();
            return CvFileFormat.Pdf;
        }

        #endregion Methods
    }

    /// <summary>
    /// Worker self: persists a CV upload session row after successful object upload.
    /// </summary>
    public sealed class ConfirmWorkerCvUploadCommand :
        CommandBase<int>
    {
        #region Properties

        /// <summary>
        /// Object key returned by init endpoint.
        /// </summary>
        public string? ObjectKey { get; set; }

        /// <summary>
        /// Original client-side file name with extension.
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Uploaded object content type.
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Uploaded object size in bytes.
        /// </summary>
        public long FileSizeBytes { get; set; }

        #endregion Properties
    }

    internal sealed class ConfirmWorkerCvUploadCommandValidator :
        IRequestValidator<ConfirmWorkerCvUploadCommand>
    {
        private const int ContentTypeMaxLength = 128;
        private const int FileNameMaxLength = 256;
        private const long MaxSizeBytes = 10L * 1024 * 1024;

        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ConfirmWorkerCvUploadCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.ObjectKey.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.MediaBlobObjectKeyRequired.ForField(nameof(ConfirmWorkerCvUploadCommand.ObjectKey)));
            }

            if (request.FileName.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.WorkerCvFileNameRequired.ForField(nameof(ConfirmWorkerCvUploadCommand.FileName)));
            }
            else if (request.FileName.Trim().Length > FileNameMaxLength)
            {
                failures.Add(ApplicationValidationCodes.WorkerCvFileNameMaxLength.ForField(nameof(ConfirmWorkerCvUploadCommand.FileName)));
            }

            if (request.ContentType.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.WorkerCvContentTypeRequired.ForField(nameof(ConfirmWorkerCvUploadCommand.ContentType)));
            }
            else if (request.ContentType.Trim().Length > ContentTypeMaxLength)
            {
                failures.Add(ApplicationValidationCodes.WorkerCvContentTypeMaxLength.ForField(nameof(ConfirmWorkerCvUploadCommand.ContentType)));
            }

            if (request.FileSizeBytes <= 0 || request.FileSizeBytes > MaxSizeBytes)
            {
                failures.Add(ApplicationValidationCodes.WorkerCvFileSizeOutOfRange.ForField(nameof(ConfirmWorkerCvUploadCommand.FileSizeBytes)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal sealed class ConfirmWorkerCvUploadCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<ConfirmWorkerCvUploadCommand, int>(serviceProvider)
    {
        #region Methods

        /// <inheritdoc />
        protected override async Task<int> HandleAsync(
            ConfirmWorkerCvUploadCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .GetByIdAsync(workerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            string objectKey = command.ObjectKey!.Trim();
            if (!OwnsCvKey(workerId, objectKey))
            {
                ApplicationValidationCodes.MediaBlobObjectKeyOwnership.Throw();
            }

            string fileName = command.FileName!.Trim();
            CvFileFormat format = ResolveFormat(fileName);
            CvUploadSession session = new(
                workerId,
                objectKey,
                fileName,
                command.ContentType!.Trim(),
                command.FileSizeBytes,
                format);

            UnitOfWork.Add(session);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await AdaIsReadModelCacheInvalidation.InvalidateWorkerReadModelsAsync(
                CacheService,
                workerId,
                cancellationToken);
            return session.Id;
        }

        private bool OwnsCvKey(int workerId, string objectKey)
        {
            string prefix = $"workers/{workerId}/cv/";
            return objectKey.StartsWith(prefix, StringComparison.Ordinal) &&
                   objectKey.Length > prefix.Length;
        }

        private CvFileFormat ResolveFormat(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (extension == ".pdf")
            {
                return CvFileFormat.Pdf;
            }

            if (extension == ".docx")
            {
                return CvFileFormat.Docx;
            }

            ApplicationValidationCodes.WorkerCvFileFormatNotSupported.Throw();
            return CvFileFormat.Pdf;
        }

        #endregion Methods
    }
}
