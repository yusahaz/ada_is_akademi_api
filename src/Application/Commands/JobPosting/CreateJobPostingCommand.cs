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
    using System;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Creates a draft job posting owned by an employer via the aggregate root.
    /// Handler result is the persisted <see cref="JobPosting"/> primary key.
    /// </summary>
    public class CreateJobPostingCommand :
        CommandBase<int>
    {
        #region Properties
        /// <summary>
        /// Full description shown to applicants.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Location where the shift takes place.
        /// </summary>
        public int EmployerLocationId { get; set; }

        /// <summary>
        /// Number of positions requested for the shift.
        /// </summary>
        public int HeadCount { get; set; }

        /// <summary>
        /// Job category identifier.
        /// </summary>
        public int JobCategoryId { get; set; }

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

    internal class CreateJobPostingCommandValidator : IRequestValidator<CreateJobPostingCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(CreateJobPostingCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerLocationId <= 0)
            {
                failures.Add(ApplicationValidationCodes.CreateJobPostingEmployerLocationId.ForField(nameof(request.EmployerLocationId)));
            }

            if (request.JobCategoryId <= 0)
            {
                failures.Add(ApplicationValidationCodes.CreateJobPostingJobCategoryId.ForField(nameof(request.JobCategoryId)));
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                failures.Add(ApplicationValidationCodes.CreateJobPostingTitleRequired.ForField(nameof(request.Title)));
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                failures.Add(ApplicationValidationCodes.CreateJobPostingDescriptionRequired.ForField(nameof(request.Description)));
            }

            if (request.HeadCount <= 0)
            {
                failures.Add(ApplicationValidationCodes.CreateJobPostingHeadCountPositive.ForField(nameof(request.HeadCount)));
            }

            if (string.IsNullOrWhiteSpace(request.WageCurrency))
            {
                failures.Add(ApplicationValidationCodes.CreateJobPostingWageCurrencyRequired.ForField(nameof(request.WageCurrency)));
            }

            if (request.WageAmount <= 0)
            {
                failures.Add(ApplicationValidationCodes.CreateJobPostingWageAmountPositive.ForField(nameof(request.WageAmount)));
            }

            if (request.ShiftEndTime <= request.ShiftStartTime)
            {
                failures.Add(ApplicationValidationCodes.CreateJobPostingShiftEndAfterStart.ForField(nameof(request.ShiftEndTime)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class CreateJobPostingCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<CreateJobPostingCommand, int>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<int> HandleAsync(CreateJobPostingCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            (await UnitOfWork
                .GetRepository<JobCategory>()
                .GetByIdAsync(command.JobCategoryId, cancellationToken))
                .ThrowIfNull(AzoxiaErrorCodes.NotFound);

            Employer? employer = await UnitOfWork
                .GetRepository<Employer>()
                .Filter(x => x.Id == employerId)
                .Include(x => x.Locations)
                .FirstOrDefaultAsync(cancellationToken);
            employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            JobPosting posting = employer.AddJobPosting(
                command.EmployerLocationId,
                command.JobCategoryId,
                command.Title,
                command.Description,
                command.ShiftDate,
                command.ShiftStartTime,
                command.ShiftEndTime,
                new Money(command.WageAmount, command.WageCurrency),
                command.HeadCount);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await AdaIsReadModelCacheInvalidation.InvalidateJobPostingReadModelsAsync(
                CacheService,
                posting.Id,
                employerId,
                cancellationToken,
                includeApplicationScopes: false);

            return posting.Id;
        }

        #endregion Utils
    }
}
