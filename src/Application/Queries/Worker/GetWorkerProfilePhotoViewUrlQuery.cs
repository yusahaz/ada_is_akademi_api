namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Worker self: saklı profil foto anahtarından kısa ömürlü GET URL üretir.
    /// </summary>
    public sealed class GetWorkerProfilePhotoViewUrlQuery :
        QueryBase<MediaBlobViewUrlModel>
    {
    }

    internal sealed class GetWorkerProfilePhotoViewUrlQueryValidator :
        IRequestValidator<GetWorkerProfilePhotoViewUrlQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetWorkerProfilePhotoViewUrlQuery _) =>
            new ValidationResult([]);

        #endregion Methods
    }

    internal sealed class GetWorkerProfilePhotoViewUrlQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetWorkerProfilePhotoViewUrlQuery, MediaBlobViewUrlModel>(serviceProvider)
    {
        private static readonly TimeSpan ViewTtl = TimeSpan.FromMinutes(10);

        #region Methods

        /// <inheritdoc />
        protected override async Task<MediaBlobViewUrlModel> HandleAsync(
            GetWorkerProfilePhotoViewUrlQuery _,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .GetByIdAsync(workerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            string? objectKey = worker.ProfilePhotoObjectKey;
            if (objectKey.IsNullOrWhiteSpace())
            {
                AzoxiaErrorCodes.NotFound.Throw();
            }

            IObjectStoragePresigner presigner = ServiceProvider.GetRequiredService<IObjectStoragePresigner>();

            DateTimeOffset expiresAtUtc = DateTimeOffset.UtcNow.Add(ViewTtl);
            string url = await presigner.CreatePresignedGetAsync(
                objectKey!,
                ViewTtl,
                cancellationToken);

            return new MediaBlobViewUrlModel(url, expiresAtUtc);
        }

        #endregion Methods
    }
}
