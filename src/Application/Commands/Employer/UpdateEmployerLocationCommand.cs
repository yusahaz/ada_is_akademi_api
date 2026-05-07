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

    /// <summary>
    /// Updates employer location metadata.
    /// </summary>
    public class UpdateEmployerLocationCommand :
        CommandBase
    {
        public string City { get; set; } = string.Empty;
        public int GeofenceRadiusMetres { get; set; }
        public double Latitude { get; set; }
        public string Line1 { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    internal class UpdateEmployerLocationCommandValidator : IRequestValidator<UpdateEmployerLocationCommand>
    {
        public ValidationResult Validate(UpdateEmployerLocationCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.LocationId <= 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(UpdateEmployerLocationCommand.LocationId)));
            }

            if (request.Name.IsNullOrWhiteSpace())
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(UpdateEmployerLocationCommand.Name)));
            }

            if (request.Line1.IsNullOrWhiteSpace())
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(UpdateEmployerLocationCommand.Line1)));
            }

            if (request.City.IsNullOrWhiteSpace())
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(UpdateEmployerLocationCommand.City)));
            }

            if (request.GeofenceRadiusMetres <= 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(UpdateEmployerLocationCommand.GeofenceRadiusMetres)));
            }

            return new ValidationResult(failures);
        }
    }

    internal class UpdateEmployerLocationCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<UpdateEmployerLocationCommand>(serviceProvider)
    {
        protected override async Task<Unit> HandleAsync(UpdateEmployerLocationCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            EmployerLocation? location = await UnitOfWork
                .GetRepository<EmployerLocation>()
                .Filter(x => x.Id == command.LocationId && x.EmployerId == employerId && !x.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
            location = location.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            location.UpdateEmployerLocation(command.Name);
            location.SetAddress(new Address(command.Line1, command.City, location.Address.Country));
            location.SetCoordinate(new GeoCoordinate(command.Latitude, command.Longitude));
            location.SetGeofenceRadiusMetres(command.GeofenceRadiusMetres);

            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.EmployerDependency(employerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.EmployerAllDependency(), cancellationToken);

            return Unit.Value;
        }
    }
}
