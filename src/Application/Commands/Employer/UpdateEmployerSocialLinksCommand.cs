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
    /// Single outbound social/web link row supplied when replacing employer links.
    /// </summary>
    public sealed class EmployerSocialLinkUpdateItem
    {
        #region Properties

        /// <summary>
        /// Logical platform bucket (<see cref="SocialMediaPlatform"/>).
        /// </summary>
        public SocialMediaPlatform Platform { get; set; }

        /// <summary>
        /// Absolute HTTPS URL text (trimmed during validation).
        /// </summary>
        public string? Url { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Employer self: replaces the entire outbound company profile link list.
    /// </summary>
    public sealed class UpdateEmployerSocialLinksCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Replacement list (empty clears all outbound links).
        /// </summary>
        public List<EmployerSocialLinkUpdateItem> Links { get; set; } = [];

        #endregion Properties
    }

    internal sealed class UpdateEmployerSocialLinksCommandValidator :
        IRequestValidator<UpdateEmployerSocialLinksCommand>
    {
        #region Fields

        private const int MaxLinks = 12;
        private const int MaxUrlChars = 2048;

        #endregion Fields

        #region Utils

        private static bool TryValidateHttpsAbsolute(string url) =>
            Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) &&
            string.Equals(uriResult.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);

        #endregion Utils

        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(UpdateEmployerSocialLinksCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.Links.Count > MaxLinks)
            {
                failures.Add(ApplicationValidationCodes.UpdateEmployerSocialLinksCount.ForField(nameof(request.Links)));
            }

            for (int i = 0; i < request.Links.Count; i++)
            {
                EmployerSocialLinkUpdateItem row = request.Links[i];
                string field = $"{nameof(UpdateEmployerSocialLinksCommand.Links)}[{i}]";
                string? urlRaw = row.Url;

                if (urlRaw.IsNullOrWhiteSpace())
                {
                    failures.Add(
                        ApplicationValidationCodes.UpdateEmployerSocialLinksUrlRequired.ForField($"{field}.{nameof(EmployerSocialLinkUpdateItem.Url)}"));
                    continue;
                }

                string url = urlRaw.Trim();

                if (url.Length > MaxUrlChars ||
                    !TryValidateHttpsAbsolute(url))
                {
                    failures.Add(
                        ApplicationValidationCodes.UpdateEmployerSocialLinksUrlInvalid.ForField($"{field}.{nameof(EmployerSocialLinkUpdateItem.Url)}"));
                }
            }

            HashSet<SocialMediaPlatform> seen = new();

            foreach (EmployerSocialLinkUpdateItem row in request.Links)
            {
                if (!seen.Add(row.Platform))
                {
                    failures.Add(ApplicationValidationCodes.UpdateEmployerSocialLinksDuplicatePlatform.ForField(nameof(request.Links)));
                    break;
                }
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal sealed class UpdateEmployerSocialLinksCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<UpdateEmployerSocialLinksCommand>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(
            UpdateEmployerSocialLinksCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            Employer? employer = await UnitOfWork
                .GetRepository<Employer>()
                .GetByIdAsync(employerId, cancellationToken);
            employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            EmployerSocialLinkInput[] inputs =
                command.Links.ConvertAll(static x =>
                    new EmployerSocialLinkInput(x.Platform, x.Url!.Trim())).ToArray();

            employer.ReplaceSocialLinks(inputs);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await AdaIsReadModelCacheInvalidation.InvalidateEmployerReadModelsAsync(
                CacheService,
                employerId,
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
