namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// Persists an employer-scoped worker profile view counter when the parties share a job application (deduped per UTC day).
    /// </summary>
    public sealed class RecordEmployerWorkerProfileViewCommand :
        CommandBase<RecordEmployerWorkerProfileViewResultModel>
    {
        #region Properties

        /// <summary>
        /// Worker primary key whose profile was opened.
        /// </summary>
        public int WorkerId { get; set; }

        #endregion Properties
    }

    internal sealed class RecordEmployerWorkerProfileViewCommandValidator :
        IRequestValidator<RecordEmployerWorkerProfileViewCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(RecordEmployerWorkerProfileViewCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.WorkerId <= 0)
            {
                failures.Add(
                    ApplicationValidationCodes.GetWorkerByIdWorkerId.ForField(nameof(RecordEmployerWorkerProfileViewCommand.WorkerId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal sealed class RecordEmployerWorkerProfileViewCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<RecordEmployerWorkerProfileViewCommand, RecordEmployerWorkerProfileViewResultModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<RecordEmployerWorkerProfileViewResultModel> HandleAsync(
            RecordEmployerWorkerProfileViewCommand command,
            CancellationToken cancellationToken)
        {
            IWorkerEmployerProfileAccess workerEmployerProfileAccess =
                ServiceProvider.GetRequiredService<IWorkerEmployerProfileAccess>();
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            await workerEmployerProfileAccess.EnsureEmployerSharesJobApplicationWithWorkerAsync(
                UnitOfWork,
                employerId,
                command.WorkerId,
                cancellationToken);

            EmployerWorkerProfileViewStat? stat = await UnitOfWork
                .GetRepository<EmployerWorkerProfileViewStat>()
                .Filter(x => x.EmployerId == employerId && x.WorkerId == command.WorkerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (stat is null)
            {
                stat = new EmployerWorkerProfileViewStat(employerId, command.WorkerId);
                UnitOfWork.Add(stat);
            }

            bool counted = stat.TryRecordView(DateTimeOffset.UtcNow);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.EmployerWorkerProfileViewStatDependency(employerId, command.WorkerId),
                cancellationToken);

            return new RecordEmployerWorkerProfileViewResultModel(counted, stat.TotalViews);
        }

        #endregion Utils
    }
}
