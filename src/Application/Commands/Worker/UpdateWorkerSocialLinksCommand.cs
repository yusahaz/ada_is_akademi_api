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
    /// Güncellenmesi gereken işçi sosyal bağlantı satırı.
    /// </summary>
    public sealed class WorkerSocialLinkUpdateItem
    {
        #region Properties

        /// <summary>
        /// Mantıksal platform kovası (<see cref="SocialMediaPlatform"/>).
        /// </summary>
        public SocialMediaPlatform Platform { get; set; }

        /// <summary>
        /// Mutlak HTTPS URL metni (trimlenecek).
        /// </summary>
        public string? Url { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Worker self: işveren yüzlerine taşınmayan çık bağlantılar listesinin tamamen değiştirilmesi.
    /// </summary>
    public sealed class UpdateWorkerSocialLinksCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Yeni liste (boş = tüm çık bağlantılar silinir).
        /// </summary>
        public List<WorkerSocialLinkUpdateItem> Links { get; set; } = [];

        #endregion Properties
    }

    internal sealed class UpdateWorkerSocialLinksCommandValidator :
        IRequestValidator<UpdateWorkerSocialLinksCommand>
    {
        private const int MaxLinks = 12;
        private const int MaxUrlChars = 2048;

        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(UpdateWorkerSocialLinksCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.Links.Count > MaxLinks)
            {
                failures.Add(ApplicationValidationCodes.UpdateWorkerSocialLinksCount.ForField(nameof(request.Links)));
            }

            for (int i = 0; i < request.Links.Count; i++)
            {
                WorkerSocialLinkUpdateItem row = request.Links[i];
                string field = $"{nameof(UpdateWorkerSocialLinksCommand.Links)}[{i}]";
                string? urlRaw = row.Url;

                if (urlRaw.IsNullOrWhiteSpace())
                {
                    failures.Add(
                        ApplicationValidationCodes.UpdateWorkerSocialLinksUrlRequired.ForField($"{field}.{nameof(WorkerSocialLinkUpdateItem.Url)}"));
                    continue;
                }

                string url = urlRaw.Trim();

                if (url.Length > MaxUrlChars ||
                    !TryValidateHttpsAbsolute(url))
                {
                    failures.Add(
                        ApplicationValidationCodes.UpdateWorkerSocialLinksUrlInvalid.ForField($"{field}.{nameof(WorkerSocialLinkUpdateItem.Url)}"));
                }
            }

            HashSet<SocialMediaPlatform> seen = new();

            foreach (WorkerSocialLinkUpdateItem row in request.Links)
            {
                if (!seen.Add(row.Platform))
                {
                    failures.Add(ApplicationValidationCodes.UpdateWorkerSocialLinksDuplicatePlatform.ForField(nameof(request.Links)));
                    break;
                }
            }

            return new ValidationResult(failures);
        }

        private static bool TryValidateHttpsAbsolute(string url) =>
            Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) &&
            string.Equals(uriResult.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);

        #endregion Methods
    }

    internal sealed class UpdateWorkerSocialLinksCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<UpdateWorkerSocialLinksCommand>(serviceProvider)
    {
        #region Methods

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(
            UpdateWorkerSocialLinksCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .GetByIdAsync(workerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            WorkerSocialLinkInput[] inputs =
                command.Links.ConvertAll(static x =>
                    new WorkerSocialLinkInput(x.Platform, x.Url!.Trim())).ToArray();

            worker.ReplaceSocialLinks(inputs);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        #endregion Methods
    }
}
