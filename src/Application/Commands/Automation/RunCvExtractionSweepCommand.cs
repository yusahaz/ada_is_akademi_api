namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Scans uploaded CV sessions and drives extraction state transitions.
    /// </summary>
    public class RunCvExtractionSweepCommand :
        CommandBase<int>;

    internal class RunCvExtractionSweepCommandValidator :
        IRequestValidator<RunCvExtractionSweepCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(RunCvExtractionSweepCommand request)
            => new();

        #endregion Methods
    }

    internal class RunCvExtractionSweepCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<RunCvExtractionSweepCommand, int>(serviceProvider)
    {
        #region Fields

        private const int BatchSize = 50;

        #endregion Fields

        #region Utils

        /// <inheritdoc />
        protected override async Task<int> HandleAsync(
            RunCvExtractionSweepCommand command,
            CancellationToken cancellationToken)
        {
            _ = command;

            ICvExtractionService extractionService = ServiceProvider.GetRequiredService<ICvExtractionService>();
            List<CvUploadSession> sessions = (await UnitOfWork
                    .GetRepository<CvUploadSession>()
                    .Filter(x => x.Status == CvUploadSessionStatus.Uploaded)
                    .OrderBy(x => x.CreatedAt)
                    .Take(BatchSize)
                    .ToListAsync(cancellationToken))
                .ToList();

            if (sessions.Count == 0)
            {
                return 0;
            }

            foreach (CvUploadSession session in sessions)
            {
                session.MarkAsExtracting();

                if (!extractionService.Supports(session.FileFormat))
                {
                    session.MarkAsFailed($"Unsupported format: {session.FileFormat}.");
                    continue;
                }

                CvExtractionResult extractionResult = await extractionService.ExtractAsync(
                    new CvExtractionRequest(
                        session.Id,
                        session.WorkerId,
                        session.ObjectKey,
                        session.FileName,
                        session.ContentType,
                        session.FileFormat),
                    cancellationToken);

                if (extractionResult.IsSuccess)
                {
                    session.MarkAsAwaitingReview(extractionResult.ExtractedJson);
                    continue;
                }

                session.MarkAsFailed(extractionResult.FailureReason);
            }

            await UnitOfWork.SaveChangesAsync(cancellationToken);
            return sessions.Count;
        }

        #endregion Utils
    }
}
