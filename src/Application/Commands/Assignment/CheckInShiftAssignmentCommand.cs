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
    /// Performs QR check-in for a shift assignment.
    /// </summary>
    public class CheckInShiftAssignmentCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Target assignment identifier.
        /// </summary>
        public int AssignmentId { get; set; }

        /// <summary>
        /// Token hash produced from scanned QR payload.
        /// </summary>
        public string CheckInTokenHash { get; set; }

        #endregion Properties
    }

    internal class CheckInShiftAssignmentCommandValidator : IRequestValidator<CheckInShiftAssignmentCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(CheckInShiftAssignmentCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.AssignmentId <= 0)
            {
                failures.Add(ApplicationValidationCodes.CheckInShiftAssignmentAssignmentId.ForField(nameof(CheckInShiftAssignmentCommand.AssignmentId)));
            }

            if (request.CheckInTokenHash.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.CheckInShiftAssignmentTokenHashRequired.ForField(nameof(CheckInShiftAssignmentCommand.CheckInTokenHash)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class CheckInShiftAssignmentCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<CheckInShiftAssignmentCommand>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(CheckInShiftAssignmentCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            ShiftAssignment? assignment = await UnitOfWork
                .GetRepository<ShiftAssignment>()
                .Filter(x => x.Id == command.AssignmentId)
                .FirstOrDefaultAsync(cancellationToken);
            assignment = assignment.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (assignment.WorkerId == workerId).ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            assignment.CheckIn(command.CheckInTokenHash);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await AdaIsReadModelCacheInvalidation.InvalidateShiftAssignmentReadModelsAsync(
                CacheService,
                assignment.Id,
                assignment.WorkerId,
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
