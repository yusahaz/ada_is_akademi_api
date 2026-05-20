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
        /// Legacy compatibility field; ignored by handler and conflict is computed server-side.
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

            bool hasConflictingShift = await HasConflictingShiftAsync(
                workerId,
                command.JobPostingId,
                posting.ShiftDate,
                posting.ShiftStartTime,
                posting.ShiftEndTime,
                cancellationToken);

            JobApplication application = posting.AddApplication(
                workerId,
                hasConflictingShift,
                command.Note);

            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await AdaIsReadModelCacheInvalidation.InvalidateJobPostingReadModelsAsync(
                CacheService,
                posting.Id,
                posting.EmployerId,
                cancellationToken);
            await AdaIsReadModelCacheInvalidation.InvalidateWorkerReadModelsAsync(
                CacheService,
                workerId,
                cancellationToken,
                removeSelfDetailKeys: false);

            return application.Id;
        }

        private async Task<bool> HasConflictingShiftAsync(
            int workerId,
            int targetPostingId,
            DateOnly shiftDate,
            TimeOnly shiftStartTime,
            TimeOnly shiftEndTime,
            CancellationToken cancellationToken)
        {
            IEnumerable<JobApplication> acceptedApplications = await UnitOfWork
                .GetRepository<JobApplication>()
                .Filter(x => x.WorkerId == workerId && x.Status == JobApplicationStatus.Accepted)
                .Include(x => x.JobPosting)
                .ToListAsync(cancellationToken);

            return acceptedApplications.Any(x =>
                x.JobPostingId != targetPostingId &&
                x.JobPosting.ShiftDate == shiftDate &&
                shiftStartTime < x.JobPosting.ShiftEndTime &&
                shiftEndTime > x.JobPosting.ShiftStartTime);
        }

        #endregion Utils
    }
}
