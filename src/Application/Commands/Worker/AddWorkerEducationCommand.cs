namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    public class AddWorkerEducationCommand : CommandBase<int>
    {
        public string Department { get; set; }
        public EducationType EducationType { get; set; }
        public int? EndYear { get; set; }
        public bool IsOngoing { get; set; }
        public string School { get; set; }
        public int StartYear { get; set; }
    }

    internal class AddWorkerEducationCommandValidator : IRequestValidator<AddWorkerEducationCommand>
    {
        public ValidationResult Validate(AddWorkerEducationCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.School.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.School)));
            if (request.Department.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.Department)));
            return new ValidationResult(failures);
        }
    }

    internal class AddWorkerEducationCommandHandler(IServiceProvider serviceProvider)
        : WorkerCollectionCommandHandlerBase<AddWorkerEducationCommand>(serviceProvider)
    {
        protected override async Task<int> HandleAsync(AddWorkerEducationCommand command, CancellationToken cancellationToken)
        {
            (int workerId, Worker worker) = await GetActorWorkerAsync(cancellationToken);
            WorkerEducation item = worker.AddEducation(command.School, command.Department, command.EducationType, command.StartYear, command.EndYear, command.IsOngoing);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
return item.Id;
        }
    }
}
