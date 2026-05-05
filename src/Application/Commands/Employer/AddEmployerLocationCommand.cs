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
    /// Adds or returns existing employer location.
    /// </summary>
    public class AddEmployerLocationCommand : CommandBase<int>
    {
        public string City { get; set; }
        public string? Description { get; set; }
        public int GeofenceRadiusMetres { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public string Line1 { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; }
    }

    internal class AddEmployerLocationCommandValidator : IRequestValidator<AddEmployerLocationCommand>
    {
        public ValidationResult Validate(AddEmployerLocationCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.Name.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.Name)));
            if (request.Line1.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.Line1)));
            if (request.City.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.City)));
            if (request.Country.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.Country)));
            if (request.GeofenceRadiusMetres <= 0) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.GeofenceRadiusMetres)));
            return new ValidationResult(failures);
        }
    }

    internal class AddEmployerLocationCommandHandler(IServiceProvider serviceProvider)
        : CommandHandlerBase<AddEmployerLocationCommand, int>(serviceProvider)
    {
        protected override async Task<int> HandleAsync(AddEmployerLocationCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            Employer? employer = await UnitOfWork.GetRepository<Employer>()
                .Filter(x => x.Id == employerId)
                .Include(x => x.Locations)
                .FirstOrDefaultAsync(cancellationToken);
            employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            EmployerLocation location = employer.AddLocation(command.Name, command.Description);
            location.SetAddress(new Address(command.Line1, command.City, command.Country));
            location.SetCoordinate(new GeoCoordinate(command.Latitude, command.Longitude));
            location.SetGeofenceRadiusMetres(command.GeofenceRadiusMetres);

            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.EmployerDependency(employerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.EmployerAllDependency(), cancellationToken);

            return location.Id;
        }
    }
}
