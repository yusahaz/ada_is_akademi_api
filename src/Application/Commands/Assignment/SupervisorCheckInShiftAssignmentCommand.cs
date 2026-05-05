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
    /// Performs supervisor-side QR confirmation for mutual check-in.
    /// </summary>
    public class SupervisorCheckInShiftAssignmentCommand :
        CommandBase
    {
        #region Properties

        public int AssignmentId { get; set; }
        public string SupervisorCheckInTokenHash { get; set; }

        #endregion Properties
    }

    internal class SupervisorCheckInShiftAssignmentCommandValidator : IRequestValidator<SupervisorCheckInShiftAssignmentCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(SupervisorCheckInShiftAssignmentCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.AssignmentId <= 0)
            {
                failures.Add(ApplicationValidationCodes.SupervisorCheckInShiftAssignmentAssignmentId.ForField(nameof(SupervisorCheckInShiftAssignmentCommand.AssignmentId)));
            }

            if (request.SupervisorCheckInTokenHash.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.SupervisorCheckInShiftAssignmentTokenHashRequired.ForField(nameof(SupervisorCheckInShiftAssignmentCommand.SupervisorCheckInTokenHash)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class SupervisorCheckInShiftAssignmentCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<SupervisorCheckInShiftAssignmentCommand>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(SupervisorCheckInShiftAssignmentCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            ShiftAssignment? assignment = await UnitOfWork
                .GetRepository<ShiftAssignment>()
                .Filter(x => x.Id == command.AssignmentId)
                .Include(x => x.JobPosting)
                .FirstOrDefaultAsync(cancellationToken);
            assignment = assignment.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (assignment.JobPosting.EmployerId == employerId).ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            assignment.SupervisorCheckIn(command.SupervisorCheckInTokenHash);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.ShiftAssignmentDependency(assignment.Id),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.ShiftAssignmentAllDependency(),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.WorkerDependency(assignment.WorkerId),
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
