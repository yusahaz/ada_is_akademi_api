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
    /// İşveren self: logo nesne anahtarından kısa ömürlü görüntüleme/indir adresi üretimi.
    /// </summary>
    public sealed class GetEmployerLogoViewUrlQuery :
        QueryBase<MediaBlobViewUrlModel>
    {
    }

    internal sealed class GetEmployerLogoViewUrlQueryValidator :
        IRequestValidator<GetEmployerLogoViewUrlQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetEmployerLogoViewUrlQuery _) =>
            new ValidationResult([]);

        #endregion Methods
    }

    internal sealed class GetEmployerLogoViewUrlQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetEmployerLogoViewUrlQuery, MediaBlobViewUrlModel>(serviceProvider)
    {
        private static readonly TimeSpan ViewTtl = TimeSpan.FromMinutes(10);

        #region Methods

        /// <inheritdoc />
        protected override async Task<MediaBlobViewUrlModel> HandleAsync(
            GetEmployerLogoViewUrlQuery _,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            Employer? employer = await UnitOfWork
                .GetRepository<Employer>()
                .GetByIdAsync(employerId, cancellationToken);
            employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            string? objectKey = employer.LogoObjectKey;
            if (objectKey.IsNullOrWhiteSpace())
            {
                AzoxiaErrorCodes.NotFound.Throw();
            }

            DateTimeOffset expiresAtUtc = DateTimeOffset.UtcNow.Add(ViewTtl);
            IObjectStoragePresigner presigner = ServiceProvider.GetRequiredService<IObjectStoragePresigner>();
            string url = await presigner.CreatePresignedGetAsync(
                objectKey!,
                ViewTtl,
                cancellationToken);

            return new MediaBlobViewUrlModel(url, expiresAtUtc);
        }

        #endregion Methods
    }
}
