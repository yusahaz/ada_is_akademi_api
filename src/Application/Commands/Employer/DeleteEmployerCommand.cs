namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using System.Linq;

    /// <summary>
    /// Soft deletes employer and all employer-scoped users.
    /// </summary>
    public class DeleteEmployerCommand : CommandBase
    {
        /// <summary>
        /// Identifier of the employer to delete.
        /// </summary>
        public int EmployerId { get; set; }
    }

    internal class DeleteEmployerCommandValidator : IRequestValidator<DeleteEmployerCommand>
    {
        /// <inheritdoc />
        public ValidationResult Validate(DeleteEmployerCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.EmployerId <= 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.EmployerId)));
            }

            return new ValidationResult(failures);
        }
    }

    internal class DeleteEmployerCommandHandler(IServiceProvider serviceProvider)
        : CommandHandlerBase<DeleteEmployerCommand>(serviceProvider)
    {
        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(DeleteEmployerCommand command, CancellationToken cancellationToken)
        {
            Employer? employer = await UnitOfWork.GetRepository<Employer>().GetByIdAsync(command.EmployerId, cancellationToken);
            employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            IEnumerable<int> employerBoundUserIds = await UnitOfWork.GetRepository<SystemUser>()
                .Filter(x => x.EmployerId == employer.Id)
                .AsNoTracking()
                .ToListAsync(x => x.Id, cancellationToken);

            IEnumerable<int> supervisorUserIdsRaw = await UnitOfWork.GetRepository<Supervisor>()
                .Filter(x => x.EmployerId == employer.Id)
                .AsNoTracking()
                .ToListAsync(x => x.SystemUserId, cancellationToken);

            IReadOnlyList<int> systemUserIds = employerBoundUserIds
                .Concat(supervisorUserIdsRaw)
                .Distinct()
                .ToList();

            if (systemUserIds.Count > 0)
            {
                IEnumerable<SystemUser> systemUsersRaw = await UnitOfWork.GetRepository<SystemUser>()
                    .Filter(x => systemUserIds.Contains(x.Id))
                    .ToListAsync(cancellationToken);
                List<SystemUser> systemUsers = systemUsersRaw.ToList();

                foreach (SystemUser systemUser in systemUsers)
                {
                    systemUser.DeleteSystemUser();
                }
            }

            employer.DeleteEmployer();

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.EmployerDependency(employer.Id), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.EmployerAllDependency(), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.SystemUserAllDependency(), cancellationToken);

            return Unit.Value;
        }
    }
}
