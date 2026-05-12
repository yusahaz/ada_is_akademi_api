namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    public class AddWorkerExperienceCommand : CommandBase<int>
    {
        public string? Description { get; set; }
        public DateOnly? EndDate { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public DateOnly StartDate { get; set; }
    }

    internal class AddWorkerExperienceCommandValidator : IRequestValidator<AddWorkerExperienceCommand>
    {
        public ValidationResult Validate(AddWorkerExperienceCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.CompanyName.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.CompanyName)));
            if (request.Position.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.Position)));
            return new ValidationResult(failures);
        }
    }

    internal class AddWorkerExperienceCommandHandler(IServiceProvider serviceProvider)
        : WorkerCollectionCommandHandlerBase<AddWorkerExperienceCommand>(serviceProvider)
    {
        protected override async Task<int> HandleAsync(AddWorkerExperienceCommand command, CancellationToken cancellationToken)
        {
            (int workerId, Worker worker) = await GetActorWorkerAsync(cancellationToken);
            WorkerExperience item = worker.AddExperience(command.CompanyName, command.Position, command.StartDate, command.EndDate, command.Description);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
return item.Id;
        }
    }
}
