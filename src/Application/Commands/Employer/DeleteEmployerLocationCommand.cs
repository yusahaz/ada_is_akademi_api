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
    /// Soft-deletes an employer location.
    /// </summary>
    public class DeleteEmployerLocationCommand :
        CommandBase
    {
        public int LocationId { get; set; }
    }

    internal class DeleteEmployerLocationCommandValidator : IRequestValidator<DeleteEmployerLocationCommand>
    {
        public ValidationResult Validate(DeleteEmployerLocationCommand request)
            => request.LocationId > 0
                ? new ValidationResult([])
                : new ValidationResult([AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(DeleteEmployerLocationCommand.LocationId))]);
    }

    internal class DeleteEmployerLocationCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<DeleteEmployerLocationCommand>(serviceProvider)
    {
        protected override async Task<Unit> HandleAsync(DeleteEmployerLocationCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            EmployerLocation? location = await UnitOfWork
                .GetRepository<EmployerLocation>()
                .Filter(x => x.Id == command.LocationId && x.EmployerId == employerId && !x.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
            location = location.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            location.DeleteEmployerLocation();

            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.EmployerDependency(employerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.EmployerAllDependency(), cancellationToken);

            return Unit.Value;
        }
    }
}
