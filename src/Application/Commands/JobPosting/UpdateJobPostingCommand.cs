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
    using Azoxia.Core.ValueTypes;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// Updates editable fields on a draft job posting.
    /// </summary>
    public class UpdateJobPostingCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Full description shown to applicants.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Number of positions requested for the shift.
        /// </summary>
        public int HeadCount { get; set; }

        /// <summary>
        /// Identifier of the job posting to update.
        /// </summary>
        public int JobPostingId { get; set; }

        /// <summary>
        /// Calendar date for the shift.
        /// </summary>
        public DateOnly ShiftDate { get; set; }

        /// <summary>
        /// End time for the shift.
        /// </summary>
        public TimeOnly ShiftEndTime { get; set; }

        /// <summary>
        /// Start time for the shift.
        /// </summary>
        public TimeOnly ShiftStartTime { get; set; }

        /// <summary>
        /// Short title of the posting.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Monetary amount component of the wage.
        /// </summary>
        public decimal WageAmount { get; set; }

        /// <summary>
        /// ISO currency code for the wage.
        /// </summary>
        public string WageCurrency { get; set; }
        #endregion Properties
    }

    internal class UpdateJobPostingCommandValidator : IRequestValidator<UpdateJobPostingCommand>
    {
        #region Methods

        /// <summary>
        /// Validates identifiers, text fields, and wage inputs on the command.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(UpdateJobPostingCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.JobPostingId <= 0)
            {
                failures.Add(ApplicationValidationCodes.UpdateJobPostingJobPostingId.ForField(nameof(request.JobPostingId)));
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                failures.Add(ApplicationValidationCodes.UpdateJobPostingTitleRequired.ForField(nameof(request.Title)));
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                failures.Add(ApplicationValidationCodes.UpdateJobPostingDescriptionRequired.ForField(nameof(request.Description)));
            }

            if (request.HeadCount <= 0)
            {
                failures.Add(ApplicationValidationCodes.UpdateJobPostingHeadCountPositive.ForField(nameof(request.HeadCount)));
            }

            if (string.IsNullOrWhiteSpace(request.WageCurrency))
            {
                failures.Add(ApplicationValidationCodes.UpdateJobPostingWageCurrencyRequired.ForField(nameof(request.WageCurrency)));
            }

            if (request.WageAmount <= 0)
            {
                failures.Add(ApplicationValidationCodes.UpdateJobPostingWageAmountPositive.ForField(nameof(request.WageAmount)));
            }

            if (request.ShiftEndTime <= request.ShiftStartTime)
            {
                failures.Add(ApplicationValidationCodes.UpdateJobPostingShiftEndAfterStart.ForField(nameof(request.ShiftEndTime)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class UpdateJobPostingCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<UpdateJobPostingCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the posting, applies updates, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(UpdateJobPostingCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            JobPosting? entity = await UnitOfWork.GetRepository<JobPosting>().GetByIdAsync(command.JobPostingId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (entity.EmployerId == employerId).ThrowIfFalse(AzoxiaErrorCodes.NotFound);

            entity.Update(
                command.Title,
                command.Description,
                command.ShiftDate,
                command.ShiftStartTime,
                command.ShiftEndTime,
                new Money(command.WageAmount, command.WageCurrency),
                command.HeadCount);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.JobPostingDependency(command.JobPostingId),
                cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.EmployerJobPostingsSummaryDependency(entity.EmployerId),
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
