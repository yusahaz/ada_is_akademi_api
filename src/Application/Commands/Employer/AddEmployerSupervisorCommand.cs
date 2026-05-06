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
    /// Adds or returns existing supervisor under employer.
    /// </summary>
    public class AddEmployerSupervisorCommand : CommandBase<int>
    {
        public int? LocationId { get; set; }
        public int SystemUserId { get; set; }
    }

    internal class AddEmployerSupervisorCommandValidator : IRequestValidator<AddEmployerSupervisorCommand>
    {
        public ValidationResult Validate(AddEmployerSupervisorCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.SystemUserId <= 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.SystemUserId)));
            }

            if (request.LocationId is <= 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.LocationId)));
            }

            return new ValidationResult(failures);
        }
    }

    internal class AddEmployerSupervisorCommandHandler(IServiceProvider serviceProvider)
        : CommandHandlerBase<AddEmployerSupervisorCommand, int>(serviceProvider)
    {
        protected override async Task<int> HandleAsync(AddEmployerSupervisorCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            Employer? employer = await UnitOfWork.GetRepository<Employer>()
                .Filter(x => x.Id == employerId)
                .AsSplitQuery()
                .Include(x => x.Locations)
                .Include(x => x.Supervisors)
                .FirstOrDefaultAsync(cancellationToken);
            employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            SystemUser? systemUser = await UnitOfWork.GetRepository<SystemUser>().GetByIdAsync(command.SystemUserId, cancellationToken);
            systemUser = systemUser.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            if (command.LocationId.HasValue)
            {
                bool hasLocation = employer.Locations.Any(x => x.Id == command.LocationId.Value);
                hasLocation.ThrowIfFalse(AzoxiaErrorCodes.NotFound);
            }

            ShiftSupervisor supervisor = employer.AddShiftSupervisor(command.SystemUserId, command.LocationId);

            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.EmployerDependency(employerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.EmployerAllDependency(), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.SystemUserAllDependency(), cancellationToken);

            return supervisor.Id;
        }
    }
}
