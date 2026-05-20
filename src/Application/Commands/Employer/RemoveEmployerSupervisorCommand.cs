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
    /// Deactivates supervisor under employer.
    /// </summary>
    public class RemoveEmployerSupervisorCommand : CommandBase
    {
        public int SystemUserId { get; set; }
    }

    internal class RemoveEmployerSupervisorCommandValidator : IRequestValidator<RemoveEmployerSupervisorCommand>
    {
        public ValidationResult Validate(RemoveEmployerSupervisorCommand request)
            => request.SystemUserId > 0
                ? new ValidationResult([])
                : new ValidationResult([AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.SystemUserId))]);
    }

    internal class RemoveEmployerSupervisorCommandHandler(IServiceProvider serviceProvider)
        : CommandHandlerBase<RemoveEmployerSupervisorCommand>(serviceProvider)
    {
        protected override async Task<Unit> HandleAsync(RemoveEmployerSupervisorCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            Employer? employer = await UnitOfWork.GetRepository<Employer>()
                .Filter(x => x.Id == employerId)
                .Include(x => x.Supervisors)
                .FirstOrDefaultAsync(cancellationToken);
            employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            employer.RemoveSupervisor(command.SystemUserId);

            SystemUser? supervisorUser = await UnitOfWork
                .GetRepository<SystemUser>()
                .GetByIdAsync(command.SystemUserId, cancellationToken);
            if (supervisorUser is not null && supervisorUser.Type == SystemUserType.Supervisor)
            {
                supervisorUser.RevokeEmployerSupervisorRole();
            }

            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await AdaIsReadModelCacheInvalidation.InvalidateEmployerReadModelsAsync(
                CacheService,
                employerId,
                cancellationToken);

            return Unit.Value;
        }
    }
}
