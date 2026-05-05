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
    using System;

    /// <summary>
    /// Creates a shift assignment from an accepted job application.
    /// </summary>
    public class CreateShiftAssignmentCommand :
        CommandBase<int>
    {
        #region Properties

        /// <summary>
        /// Token hash expected during QR check-in.
        /// </summary>
        public string CheckInTokenHash { get; set; }

        /// <summary>
        /// Accepted job application identifier.
        /// </summary>
        public int JobApplicationId { get; set; }

        #endregion Properties
    }

    internal class CreateShiftAssignmentCommandValidator : IRequestValidator<CreateShiftAssignmentCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(CreateShiftAssignmentCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.JobApplicationId <= 0)
            {
                failures.Add(ApplicationValidationCodes.CreateShiftAssignmentJobApplicationId.ForField(nameof(CreateShiftAssignmentCommand.JobApplicationId)));
            }

            if (request.CheckInTokenHash.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.CreateShiftAssignmentCheckInTokenHashRequired.ForField(nameof(CreateShiftAssignmentCommand.CheckInTokenHash)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class CreateShiftAssignmentCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<CreateShiftAssignmentCommand, int>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<int> HandleAsync(CreateShiftAssignmentCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            JobApplication? application = await UnitOfWork
                .GetRepository<JobApplication>()
                .Filter(x => x.Id == command.JobApplicationId)
                .Include(x => x.JobPosting)
                .FirstOrDefaultAsync(cancellationToken);
            application = application.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            (application.Status == JobApplicationStatus.Accepted)
                .ThrowIfFalse(ApplicationValidationCodes.CreateShiftAssignmentApplicationNotAccepted);
            (application.JobPosting.EmployerId == employerId)
                .ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            ShiftAssignment? existing = await UnitOfWork
                .GetRepository<ShiftAssignment>()
                .Filter(x => x.JobApplicationId == command.JobApplicationId)
                .FirstOrDefaultAsync(cancellationToken);
            if (existing is not null)
            {
                return existing.Id;
            }

            var assignment = new ShiftAssignment(
                application.JobPostingId,
                application.Id,
                application.WorkerId,
                command.CheckInTokenHash);
            UnitOfWork.Add(assignment);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.ShiftAssignmentAllDependency(),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.JobPostingAllDependency(),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.WorkerDependency(application.WorkerId),
                cancellationToken);

            return assignment.Id;
        }

        #endregion Utils
    }
}
