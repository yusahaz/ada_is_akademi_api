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
    using System.Text.Json;

    /// <summary>
    /// Worker self: confirms extracted CV payload for one session.
    /// </summary>
    public sealed class ConfirmWorkerCvReviewCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Target upload session identifier.
        /// </summary>
        public int CvUploadSessionId { get; set; }

        /// <summary>
        /// Applies extracted education rows when true.
        /// </summary>
        public bool ApplyEducations { get; set; } = true;

        /// <summary>
        /// Applies extracted experience rows when true.
        /// </summary>
        public bool ApplyExperiences { get; set; } = true;

        /// <summary>
        /// Applies extracted certificate rows when true.
        /// </summary>
        public bool ApplyCertificates { get; set; } = true;

        /// <summary>
        /// Applies extracted language rows when true.
        /// </summary>
        public bool ApplyLanguages { get; set; } = true;

        /// <summary>
        /// Applies extracted skill rows when true.
        /// </summary>
        public bool ApplySkills { get; set; } = true;

        #endregion Properties
    }

    internal sealed class ConfirmWorkerCvReviewCommandValidator :
        IRequestValidator<ConfirmWorkerCvReviewCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ConfirmWorkerCvReviewCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.CvUploadSessionId <= 0)
            {
                failures.Add(ApplicationValidationCodes.WorkerCvUploadSessionIdRequired.ForField(nameof(ConfirmWorkerCvReviewCommand.CvUploadSessionId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal sealed class ConfirmWorkerCvReviewCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<ConfirmWorkerCvReviewCommand>(serviceProvider)
    {
        #region Methods

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(
            ConfirmWorkerCvReviewCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            CvUploadSession? session = await UnitOfWork
                .GetRepository<CvUploadSession>()
                .Filter(x => x.Id == command.CvUploadSessionId && x.WorkerId == workerId)
                .FirstOrDefaultAsync(cancellationToken);
            session = session.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            if (session.Status == CvUploadSessionStatus.Confirmed)
            {
                return Unit.Value;
            }

            // Apply flow is allowed only once while review is pending; terminal non-confirmed states stay invalid.
            (session.Status == CvUploadSessionStatus.AwaitingReview)
                .ThrowIfFalse(DomainErrorCodes.CvUploadSessionInvalidStatusTransition);

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .GetByIdAsync(workerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            CvExtractionPayload payload = ParsePayload(session.ExtractedJson);
            ApplyPayload(worker, payload, command);
            session.Confirm();
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerDependency(workerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerAllDependency(), cancellationToken);
            return Unit.Value;
        }

        private void ApplyPayload(Worker worker, CvExtractionPayload payload, ConfirmWorkerCvReviewCommand command)
        {
            if (command.ApplyCertificates)
            {
                foreach (CvCertificateRow row in payload.Certificates)
                {
                    if (row.Name.IsNullOrWhiteSpace() || row.IssuingOrganization.IsNullOrWhiteSpace())
                    {
                        continue;
                    }

                    if (!TryParseDate(row.IssuedAt, out DateOnly issuedAt))
                    {
                        continue;
                    }

                    DateOnly? expiresAt = TryParseDate(row.ExpiresAt, out DateOnly parsedExpires)
                        ? parsedExpires
                        : null;

                    worker.AddCertificate(
                        row.Name.Trim(),
                        row.IssuingOrganization.Trim(),
                        issuedAt,
                        expiresAt,
                        null);
                }
            }

            if (command.ApplyEducations)
            {
                foreach (CvEducationRow row in payload.Educations)
                {
                    if (row.School.IsNullOrWhiteSpace() || row.Department.IsNullOrWhiteSpace())
                    {
                        continue;
                    }

                    if (!Enum.TryParse(row.EducationType, true, out EducationType educationType))
                    {
                        educationType = EducationType.Other;
                    }

                    bool hasStartYear = int.TryParse(row.StartYear, out int startYear);
                    if (!hasStartYear || startYear <= 0)
                    {
                        continue;
                    }

                    int? endYear = int.TryParse(row.EndYear, out int parsedEndYear) && parsedEndYear > 0
                        ? parsedEndYear
                        : null;

                    bool isOngoing = row.IsOngoing ?? !endYear.HasValue;
                    worker.AddEducation(
                        row.School.Trim(),
                        row.Department.Trim(),
                        educationType,
                        startYear,
                        endYear,
                        isOngoing);
                }
            }

            if (command.ApplyExperiences)
            {
                foreach (CvExperienceRow row in payload.Experiences)
                {
                    if (row.CompanyName.IsNullOrWhiteSpace() || row.Position.IsNullOrWhiteSpace())
                    {
                        continue;
                    }

                    if (!TryParseDate(row.StartDate, out DateOnly startDate))
                    {
                        continue;
                    }

                    DateOnly? endDate = TryParseDate(row.EndDate, out DateOnly parsedEndDate)
                        ? parsedEndDate
                        : null;

                    worker.AddExperience(
                        row.CompanyName.Trim(),
                        row.Position.Trim(),
                        startDate,
                        endDate,
                        row.Description);
                }
            }

            if (command.ApplyLanguages)
            {
                foreach (CvLanguageRow row in payload.Languages)
                {
                    if (row.Language.IsNullOrWhiteSpace() ||
                        !Enum.TryParse(row.Level, true, out LanguageLevel level))
                    {
                        continue;
                    }

                    worker.AddLanguage(row.Language.Trim(), level);
                }
            }

            if (command.ApplySkills)
            {
                foreach (string skill in payload.Skills)
                {
                    if (skill.IsNullOrWhiteSpace())
                    {
                        continue;
                    }

                    worker.AddSkill(skill.Trim());
                }
            }
        }

        private CvExtractionPayload ParsePayload(string? extractedJson)
        {
            if (extractedJson.IsNullOrWhiteSpace())
            {
                return new CvExtractionPayload();
            }

            CvExtractionPayload? parsed = JsonSerializer.Deserialize<CvExtractionPayload>(
                extractedJson,
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    PropertyNameCaseInsensitive = true,
                });

            return parsed ?? new CvExtractionPayload();
        }

        private bool TryParseDate(string? value, out DateOnly date)
            => DateOnly.TryParse(value, out date);

        #endregion Methods
    }

    /// <summary>
    /// Worker self: discards extracted CV payload for one session.
    /// </summary>
    public sealed class DiscardWorkerCvReviewCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Target upload session identifier.
        /// </summary>
        public int CvUploadSessionId { get; set; }

        #endregion Properties
    }

    internal sealed class DiscardWorkerCvReviewCommandValidator :
        IRequestValidator<DiscardWorkerCvReviewCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(DiscardWorkerCvReviewCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.CvUploadSessionId <= 0)
            {
                failures.Add(ApplicationValidationCodes.WorkerCvUploadSessionIdRequired.ForField(nameof(DiscardWorkerCvReviewCommand.CvUploadSessionId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal sealed class DiscardWorkerCvReviewCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<DiscardWorkerCvReviewCommand>(serviceProvider)
    {
        #region Methods

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(
            DiscardWorkerCvReviewCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            CvUploadSession? session = await UnitOfWork
                .GetRepository<CvUploadSession>()
                .Filter(x => x.Id == command.CvUploadSessionId && x.WorkerId == workerId)
                .FirstOrDefaultAsync(cancellationToken);
            session = session.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            session.Discard();
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerDependency(workerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerAllDependency(), cancellationToken);
            return Unit.Value;
        }

        #endregion Methods
    }

    internal sealed record CvCertificateRow(
        string? Name,
        string? IssuingOrganization,
        string? IssuedAt,
        string? ExpiresAt);

    internal sealed record CvEducationRow(
        string? School,
        string? Department,
        string? EducationType,
        string? StartYear,
        string? EndYear,
        bool? IsOngoing);

    internal sealed record CvExperienceRow(
        string? CompanyName,
        string? Position,
        string? StartDate,
        string? EndDate,
        string? Description);

    internal sealed record CvLanguageRow(
        string? Language,
        string? Level);

    internal sealed class CvExtractionPayload
    {
        #region Properties

        public IReadOnlyList<CvCertificateRow> Certificates { get; init; } = [];

        public IReadOnlyList<CvEducationRow> Educations { get; init; } = [];

        public IReadOnlyList<CvExperienceRow> Experiences { get; init; } = [];

        public IReadOnlyList<CvLanguageRow> Languages { get; init; } = [];

        public IReadOnlyList<string> Skills { get; init; } = [];

        #endregion Properties
    }
}
