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
    using System;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Submits a worker application to an open job posting.
    /// Handler result is the persisted <see cref="JobApplication"/> primary key (existing pending row returns its id).
    /// </summary>
    public class SubmitJobPostingApplicationCommand :
        CommandBase<int>
    {
        #region Properties
        /// <summary>
        /// When true, the worker is blocked from applying due to an overlapping shift (domain rule).
        /// </summary>
        public bool HasConflictingShift { get; set; }

        /// <summary>
        /// Target job posting identifier.
        /// </summary>
        public int JobPostingId { get; set; }

        /// <summary>
        /// Optional note from the worker.
        /// </summary>
        public string? Note { get; set; }
        #endregion Properties
    }

    internal class SubmitJobPostingApplicationCommandValidator : IRequestValidator<SubmitJobPostingApplicationCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(SubmitJobPostingApplicationCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.JobPostingId <= 0)
            {
                failures.Add(ApplicationValidationCodes.SubmitJobPostingApplicationJobPostingId.ForField(nameof(request.JobPostingId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class SubmitJobPostingApplicationCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<SubmitJobPostingApplicationCommand, int>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<int> HandleAsync(
            SubmitJobPostingApplicationCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            (await UnitOfWork
                .GetRepository<Worker>()
                .GetByIdAsync(workerId, cancellationToken))
                .ThrowIfNull(AzoxiaErrorCodes.NotFound);

            JobPosting? posting = await UnitOfWork
                .GetRepository<JobPosting>()
                .Filter(x => x.Id == command.JobPostingId)
                .Include(x => x.Applications)
                .FirstOrDefaultAsync(cancellationToken);
            posting = posting.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            JobApplication application = posting.AddApplication(
                workerId,
                command.HasConflictingShift,
                command.Note);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.JobPostingDependency(command.JobPostingId),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.EmployerJobPostingsSummaryDependency(posting.EmployerId),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.JobApplicationAllDependency(),
                cancellationToken);

            return application.Id;
        }

        #endregion Utils
    }
}
