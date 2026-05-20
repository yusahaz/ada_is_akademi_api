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
    /// Performs worker check-out for a shift assignment.
    /// </summary>
    public class CheckOutShiftAssignmentCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Target assignment identifier.
        /// </summary>
        public int AssignmentId { get; set; }

        #endregion Properties
    }

    internal class CheckOutShiftAssignmentCommandValidator : IRequestValidator<CheckOutShiftAssignmentCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(CheckOutShiftAssignmentCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.AssignmentId <= 0)
            {
                failures.Add(ApplicationValidationCodes.CheckOutShiftAssignmentAssignmentId.ForField(nameof(CheckOutShiftAssignmentCommand.AssignmentId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class CheckOutShiftAssignmentCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<CheckOutShiftAssignmentCommand>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(CheckOutShiftAssignmentCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            ShiftAssignment? assignment = await UnitOfWork
                .GetRepository<ShiftAssignment>()
                .Filter(x => x.Id == command.AssignmentId)
                .FirstOrDefaultAsync(cancellationToken);
            assignment = assignment.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (assignment.WorkerId == workerId).ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            assignment.CheckOut();

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
